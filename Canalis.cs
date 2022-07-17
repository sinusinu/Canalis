using System;
using System.Runtime.InteropServices;

namespace CanalisLib;

public class Canalis : IDisposable {
    private const string dllName = "canalis-native.dll";

    public enum Error {
        Success,
        GenericError,
        InvalidState,
        InvalidParam,
        Unsupported,
        OutOfMemory,
        CannotOpenFile,
        IndexOutOfBounds,
    };

    public enum State {
        Init,
        Ready,
        Error,
    };

    public enum SampleFormat {
        Undefined,
        S16,
        S32,
        F32,
    };

    [DllImport(dllName, EntryPoint = "canalis_create")]
    private static extern IntPtr Create();

    [DllImport(dllName, EntryPoint = "canalis_load_wav")]
    private static extern void LoadWav(IntPtr instance, string path, SampleFormat format);

    [DllImport(dllName, EntryPoint = "canalis_load_mp3")]
    private static extern void LoadMp3(IntPtr instance, string path, SampleFormat format);

    [DllImport(dllName, EntryPoint = "canalis_load_vorbis")]
    private static extern void LoadVorbis(IntPtr instance, string path, SampleFormat format);

    [DllImport(dllName, EntryPoint = "canalis_get_last_error")]
    private static extern Error GetLastError(IntPtr instance);

    [DllImport(dllName, EntryPoint = "canalis_get_state")]
    private static extern State GetState(IntPtr instance);

    [DllImport(dllName, EntryPoint = "canalis_get_sample_rate")]
    private static extern int GetSampleRate(IntPtr instance);

    [DllImport(dllName, EntryPoint = "canalis_get_sample_format")]
    private static extern SampleFormat GetSampleFormat(IntPtr instance);

    [DllImport(dllName, EntryPoint = "canalis_get_channels")]
    private static extern int GetChannels(IntPtr instance);
    
    [DllImport(dllName, EntryPoint = "canalis_get_byte_count")]
    private static extern uint GetByteCount(IntPtr instance);

    [DllImport(dllName, EntryPoint = "canalis_get_position")]
    private static extern uint GetPosition(IntPtr instance);

    [DllImport(dllName, EntryPoint = "canalis_set_position")]
    private static extern void SetPosition(IntPtr instance, uint position);

    [DllImport(dllName, EntryPoint = "canalis_read")]
    private static unsafe extern void Read(IntPtr instance, void* buf, int sizeInBytes, out int readBytes);

    [DllImport(dllName, EntryPoint = "canalis_read")]
    private static unsafe extern void Read(IntPtr instance, void* buf, int sizeInBytes, IntPtr nul);

    private static void Read(IntPtr instance, short[] buf, out int readBytes) {
        unsafe {
            fixed (void* pbuf = buf) {
                Read(instance, pbuf, buf.Length * 2, out readBytes);
            }
        }
    }

    private static void Read(IntPtr instance, short[] buf) {
        unsafe {
            fixed (void* pbuf = buf) {
                Read(instance, pbuf, buf.Length * 2, IntPtr.Zero);
            }
        }
    }

    private static void Read(IntPtr instance, int[] buf, out int readBytes) {
        unsafe {
            fixed (void* pbuf = buf) {
                Read(instance, pbuf, buf.Length * 4, out readBytes);
            }
        }
    }

    private static void Read(IntPtr instance, int[] buf) {
        unsafe {
            fixed (void* pbuf = buf) {
                Read(instance, pbuf, buf.Length * 4, IntPtr.Zero);
            }
        }
    }

    private static void Read(IntPtr instance, float[] buf, out int readBytes) {
        unsafe {
            fixed (void* pbuf = buf) {
                Read(instance, pbuf, buf.Length * 4, out readBytes);
            }
        }
    }

    private static void Read(IntPtr instance, float[] buf) {
        unsafe {
            fixed (void* pbuf = buf) {
                Read(instance, pbuf, buf.Length * 4, IntPtr.Zero);
            }
        }
    }
    
    [DllImport(dllName, EntryPoint = "canalis_free")]
    private static extern void Free(IntPtr instance);

    IntPtr instance;

    bool _disposed;

    public Canalis() {
        instance = Create();
    }

    public Canalis(string path, SampleFormat format) {
        instance = Create();
        Load(path, format);
    }

    ~Canalis() => Dispose(false);

    public void Load(string path, SampleFormat format) {
        string extension = path.ToLower().Substring(path.Length - 3);
        switch (extension) {
            case "wav":
                LoadWav(instance, path, format);
                break;
            case "mp3":
                LoadMp3(instance, path, format);
                break;
            case "ogg":
                LoadVorbis(instance, path, format);
                break;
            default:
                throw new ArgumentException("path has invalid extension, must be one of wav/mp3/ogg");
        }

        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to load file: " + GetLastError(instance));
    }

    public int GetSampleRate() {
        int sampleRate = GetSampleRate(instance);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to get sample rate: " + GetLastError(instance));
        return sampleRate;
    }

    public SampleFormat GetSampleFormat() {
        SampleFormat sampleFormat = GetSampleFormat(instance);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to get sample format: " + GetLastError(instance));
        return sampleFormat;
    }

    public int GetChannels() {
        int channels = GetChannels(instance);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to get channels: " + GetLastError(instance));
        return channels;
    }

    public uint GetByteCount() {
        uint byteCount = GetByteCount(instance);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to get byte count: " + GetLastError(instance));
        return byteCount;
    }

    public uint GetPosition() {
        uint position = GetPosition(instance);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to get position: " + GetLastError(instance));
        return position;
    }

    public void SetPosition(uint position) {
        SetPosition(instance, position);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to set position: " + GetLastError(instance));
    }

    public void Read(short[] buf) {
        SampleFormat sampleFormat = GetSampleFormat(instance);
        if (sampleFormat != SampleFormat.S16) throw new ArgumentException("Sample format mismatch, buf should be short[]");

        Read(instance, buf);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to read: " + GetLastError(instance));
    }

    public void Read(short[] buf, out int readBytes) {
        SampleFormat sampleFormat = GetSampleFormat(instance);
        if (sampleFormat != SampleFormat.S16) throw new ArgumentException("Sample format mismatch, buf should be short[]");

        Read(instance, buf, out readBytes);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to read: " + GetLastError(instance));
    }

    public void Read(int[] buf) {
        SampleFormat sampleFormat = GetSampleFormat(instance);
        if (sampleFormat != SampleFormat.S32) throw new ArgumentException("Sample format mismatch, buf should be int[]");

        Read(instance, buf);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to read: " + GetLastError(instance));
    }

    public void Read(int[] buf, out int readBytes) {
        SampleFormat sampleFormat = GetSampleFormat(instance);
        if (sampleFormat != SampleFormat.S32) throw new ArgumentException("Sample format mismatch, buf should be int[]");

        Read(instance, buf, out readBytes);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to read: " + GetLastError(instance));
    }

    public void Read(float[] buf) {
        SampleFormat sampleFormat = GetSampleFormat(instance);
        if (sampleFormat != SampleFormat.F32) throw new ArgumentException("Sample format mismatch, buf should be float[]");

        Read(instance, buf);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to read: " + GetLastError(instance));
    }

    public void Read(float[] buf, out int readBytes) {
        SampleFormat sampleFormat = GetSampleFormat(instance);
        if (sampleFormat != SampleFormat.F32) throw new ArgumentException("Sample format mismatch, buf should be float[]");

        Read(instance, buf, out readBytes);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to read: " + GetLastError(instance));
    }

    protected virtual void Dispose(bool disposing) {
        if (_disposed) return;
        Free(instance);
        _disposed = true;
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
