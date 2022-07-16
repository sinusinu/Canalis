// -- Headers --

#include <stdint.h>         // for size_t
#include <memory.h>         // for malloc, memcpy, free

#define DR_WAV_IMPLEMENTATION
#include "dr_wav.h"

#define DR_MP3_IMPLEMENTATION
#include "dr_mp3.h"

#include "stb_vorbis.c"

// -- Macros --

#if defined(_MSC_VER)
#define canalis_export __declspec(dllexport)
#else
#define canalis_export
#endif

// -- Types and enums --

typedef enum {
    CANALIS_SUCCESS,
    CANALIS_ERROR_GENERIC,
    CANALIS_ERROR_INVALID_STATE,
    CANALIS_ERROR_INVALID_PARAM,
    CANALIS_ERROR_UNSUPPORTED,
    CANALIS_ERROR_OUT_OF_MEMORY,
    CANALIS_ERROR_CANNOT_OPEN_FILE,
    CANALIS_ERROR_INDEX_OUT_OF_BOUNDS,
} canalis_error;

typedef enum {
    CANALIS_STATE_INIT,
    CANALIS_STATE_READY,
    CANALIS_STATE_ERROR,    // this only indicates an irrecoverable error; trivial errors won't render instance as unusable.
} canalis_state;

typedef enum {
    CANALIS_HANDLER_TYPE_UNDEFINED,
    CANALIS_HANDLER_TYPE_WAV,
    CANALIS_HANDLER_TYPE_MP3,
    CANALIS_HANDLER_TYPE_VORBIS,
    CANALIS_HANDLER_TYPE_DUMMY,
} canalis_handler_type;

typedef enum {
    CANALIS_SAMPLE_FORMAT_UNDEFINED,
    CANALIS_SAMPLE_FORMAT_S16,
    CANALIS_SAMPLE_FORMAT_S32,
    CANALIS_SAMPLE_FORMAT_F32,
} canalis_sample_format;

// -- Structures --

typedef struct CanalisInstance {
    canalis_state state;
    canalis_error error;

    canalis_handler_type handler_type;
    canalis_sample_format sample_format;
    
    int32_t sample_rate;
    int32_t channels;

    uint32_t data_size_in_bytes;
    uint32_t seek_position_in_bytes;
    
    void* data;
} CanalisInstance;

// -- Function definitions --

canalis_export CanalisInstance* canalis_create();
canalis_export void canalis_load_wav(CanalisInstance* instance, const char* path, canalis_sample_format format);
canalis_export void canalis_load_mp3(CanalisInstance* instance, const char* path, canalis_sample_format format);
canalis_export void canalis_load_vorbis(CanalisInstance* instance, const char* path, canalis_sample_format format);
canalis_export canalis_error canalis_get_last_error(CanalisInstance* instance);
canalis_export int32_t canalis_get_sample_rate(CanalisInstance* instance);
canalis_export canalis_sample_format canalis_get_sample_format(CanalisInstance* instance);
canalis_export int32_t canalis_get_channels(CanalisInstance* instance);
canalis_export uint32_t canalis_get_byte_count(CanalisInstance* instance);
canalis_export uint32_t canalis_get_position(CanalisInstance* instance);
canalis_export void canalis_set_position(CanalisInstance* instance, uint32_t position);
canalis_export void canalis_read(CanalisInstance* instance, void* buf, int32_t size_in_bytes, int32_t* read_bytes);
canalis_export void canalis_free(CanalisInstance* instance);

// -- Function implementations --

canalis_export CanalisInstance* canalis_create(int* result) {
    CanalisInstance* instance = malloc(sizeof(CanalisInstance));
    memset(instance, 0, sizeof(CanalisInstance));
    
    instance->error = CANALIS_SUCCESS;
    return instance;
}

canalis_export void canalis_load_wav(CanalisInstance* instance, const char* path, canalis_sample_format format) {
    if (instance->state != CANALIS_STATE_INIT) {
        instance->error = CANALIS_ERROR_INVALID_STATE;
        return;
    }

    drwav* wav = malloc(sizeof(drwav));
    if (!drwav_init_file(wav, path, NULL)) {
        free(wav);
        instance->error = CANALIS_ERROR_CANNOT_OPEN_FILE;
        return;
    }

    instance->handler_type = CANALIS_HANDLER_TYPE_WAV;
    instance->channels = wav->channels;
    instance->sample_rate = wav->fmt.sampleRate;

    void* data;
    uint8_t sample_size;
    uint32_t decoded_samples;
    
    switch (format) {
        case CANALIS_SAMPLE_FORMAT_S16:
            data = malloc(wav->totalPCMFrameCount * wav->channels * sizeof(drwav_int16));
            decoded_samples = drwav_read_pcm_frames_s16(wav, wav->totalPCMFrameCount, data);
            sample_size = 2;
            break;
        case CANALIS_SAMPLE_FORMAT_S32:
            data = malloc(wav->totalPCMFrameCount * wav->channels * sizeof(drwav_int32));
            decoded_samples = drwav_read_pcm_frames_s32(wav, wav->totalPCMFrameCount, data);
            sample_size = 4;
            break;
        case CANALIS_SAMPLE_FORMAT_F32:
            data = malloc(wav->totalPCMFrameCount * wav->channels * sizeof(float));
            decoded_samples = drwav_read_pcm_frames_f32(wav, wav->totalPCMFrameCount, data);
            sample_size = 4;
            break;
    }

    drwav_uninit(wav);

    instance->data_size_in_bytes = wav->totalPCMFrameCount * wav->channels * sample_size;
    instance->sample_format = format;
    instance->data = data;
    instance->state = CANALIS_STATE_READY;
    instance->error = CANALIS_SUCCESS;
}

canalis_export void canalis_load_mp3(CanalisInstance* instance, const char* path, canalis_sample_format format) {
    if (instance->state != CANALIS_STATE_INIT) {
        instance->error = CANALIS_ERROR_INVALID_STATE;
        return;
    }

    drmp3* mp3 = malloc(sizeof(drmp3));
    if (!(drmp3_init_file(mp3, path, NULL))) {
        free(mp3);
        instance->error = CANALIS_ERROR_CANNOT_OPEN_FILE;
        return;
    }
    
    instance->handler_type = CANALIS_HANDLER_TYPE_MP3;
    instance->channels = mp3->channels;
    instance->sample_rate = mp3->sampleRate;

    void* data;
    uint8_t sample_size;
    uint32_t decoded_samples;
    int pcm_frame_count = drmp3_get_pcm_frame_count(mp3);

    int16_t* data16;
    int32_t* data32;

    switch (format) {
        case CANALIS_SAMPLE_FORMAT_S16:
            data = malloc(pcm_frame_count * mp3->channels * sizeof(drmp3_int16));
            decoded_samples = drmp3_read_pcm_frames_s16(mp3, pcm_frame_count, data);
            sample_size = 2;
            break;
        case CANALIS_SAMPLE_FORMAT_S32:
            data16 = malloc(pcm_frame_count * mp3->channels * sizeof(drmp3_int16));
            data32 = malloc(pcm_frame_count * mp3->channels * sizeof(drmp3_int32));
            decoded_samples = drmp3_read_pcm_frames_s16(mp3, pcm_frame_count, data16);
            for (int i = 0; i < pcm_frame_count * mp3->channels; i++) data32[i] = data16[i] << 16;
            free(data16);
            data = data32;
            sample_size = 4;
            break;
        case CANALIS_SAMPLE_FORMAT_F32:
            data = malloc(pcm_frame_count * mp3->channels * sizeof(float));
            decoded_samples = drmp3_read_pcm_frames_f32(mp3, pcm_frame_count, data);
            sample_size = 4;
            break;
    }

    drmp3_uninit(mp3);

    instance->data_size_in_bytes = pcm_frame_count * mp3->channels * sample_size;
    instance->sample_format = format;
    instance->data = data;
    instance->state = CANALIS_STATE_READY;
    instance->error = CANALIS_SUCCESS;
}

canalis_export void canalis_load_vorbis(CanalisInstance* instance, const char* path, canalis_sample_format format) {
    if (instance->state != CANALIS_STATE_INIT) {
        instance->error = CANALIS_ERROR_INVALID_STATE;
        return;
    }

    int16_t* data16;
    int channels, sample_rate, sample_count;
    sample_count = stb_vorbis_decode_filename(path, &channels, &sample_rate, &data16);
    if (!sample_count) {
        instance->error = CANALIS_ERROR_CANNOT_OPEN_FILE;
        return;
    }

    instance->channels = channels;
    instance->sample_rate = sample_rate;

    void* data;
    
    int32_t* data32;
    float* dataf;

    uint8_t sample_size;

    switch (format) {
        case CANALIS_SAMPLE_FORMAT_S16:
            data = data16;
            sample_size = 2;
            break;
        case CANALIS_SAMPLE_FORMAT_S32:
            data32 = malloc(sample_count * sizeof(int32_t));
            for (int i = 0; i < sample_count; i++) data32[i] = data16[i] << 16;
            free(data16);
            data = data32;
            sample_size = 4;
            break;
        case CANALIS_SAMPLE_FORMAT_F32:
            dataf = malloc(sample_count * sizeof(int32_t));
            for (int i = 0; i < sample_count; i++) dataf[i] = data16[i] * 0.000030517578125f;
            free(data16);
            data = dataf;
            sample_size = 4;
            break;
    }

    instance->data_size_in_bytes = sample_count * sample_size;
    instance->sample_format = format;
    instance->data = data;
    instance->state = CANALIS_STATE_READY;
    instance->error = CANALIS_SUCCESS;
}

canalis_export canalis_error canalis_get_last_error(CanalisInstance* instance) {
    return instance->error;
}

canalis_export int32_t canalis_get_sample_rate(CanalisInstance* instance) {
    if (instance->state != CANALIS_STATE_READY) {
        instance->error = CANALIS_ERROR_INVALID_STATE;
        return 0;
    }
    
    instance->error = CANALIS_SUCCESS;
    return instance->sample_rate;
}

canalis_export canalis_sample_format canalis_get_sample_format(CanalisInstance* instance) {
    if (instance->state != CANALIS_STATE_READY) {
        instance->error = CANALIS_ERROR_INVALID_STATE;
        return 0;
    }
    
    instance->error = CANALIS_SUCCESS;
    return instance->sample_format;
}

canalis_export int32_t canalis_get_channels(CanalisInstance* instance) {
    if (instance->state != CANALIS_STATE_READY) {
        instance->error = CANALIS_ERROR_INVALID_STATE;
        return 0;
    }
    
    instance->error = CANALIS_SUCCESS;
    return instance->channels;
}

canalis_export uint32_t canalis_get_byte_count(CanalisInstance* instance) {
    if (instance->state != CANALIS_STATE_READY) {
        instance->error = CANALIS_ERROR_INVALID_STATE;
        return 0;
    }

    instance->error = CANALIS_SUCCESS;
    return instance->data_size_in_bytes;
}

canalis_export uint32_t canalis_get_position(CanalisInstance* instance) {
    if (instance->state != CANALIS_STATE_READY) {
        instance->error = CANALIS_ERROR_INVALID_STATE;
        return -1;
    }
    
    instance->error = CANALIS_SUCCESS;
    return instance->seek_position_in_bytes;
}

canalis_export void canalis_set_position(CanalisInstance* instance, uint32_t position) {
    if (instance->state != CANALIS_STATE_READY) {
        instance->error = CANALIS_ERROR_INVALID_STATE;
        return;
    }

    if (position < 0 || position >= instance->data_size_in_bytes) {
        instance->error = CANALIS_ERROR_INDEX_OUT_OF_BOUNDS;
        return;
    }

    instance->seek_position_in_bytes = position;
    instance->error = CANALIS_SUCCESS;
}

canalis_export void canalis_read(CanalisInstance* instance, void* buf, int32_t size_in_bytes, int32_t* read_bytes) {
    if (instance->state != CANALIS_STATE_READY) {
        instance->error = CANALIS_ERROR_INVALID_STATE;
        return;
    }

    int32_t copy_size;
    if (instance->seek_position_in_bytes + size_in_bytes >= instance->data_size_in_bytes) {
        copy_size = instance->data_size_in_bytes - instance->seek_position_in_bytes;
    } else {
        copy_size = size_in_bytes;
    }

    memcpy(buf, instance->data + instance->seek_position_in_bytes, copy_size);

    if (read_bytes != NULL) *read_bytes = copy_size;
    instance->seek_position_in_bytes += copy_size;

    instance->error = CANALIS_SUCCESS;
}

canalis_export void canalis_free(CanalisInstance* instance) {
    if (instance->state == CANALIS_STATE_READY) free(instance->data);
    free(instance);
}
