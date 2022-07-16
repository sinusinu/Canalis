using System;
using System.Runtime.InteropServices;

namespace CanalisLib;

// TODO: refactor this to object-orient-thingy
public static class Canalis {
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
    public static extern IntPtr Create();

    [DllImport(dllName, EntryPoint = "canalis_load_wav")]
    public static extern void LoadWav(IntPtr instance, string path, SampleFormat format);

    [DllImport(dllName, EntryPoint = "canalis_load_mp3")]
    public static extern void LoadMp3(IntPtr instance, string path, SampleFormat format);

    [DllImport(dllName, EntryPoint = "canalis_load_vorbis")]
    public static extern void LoadVorbis(IntPtr instance, string path, SampleFormat format);

    [DllImport(dllName, EntryPoint = "canalis_get_last_error")]
    public static extern Error GetLastError(IntPtr instance);

    [DllImport(dllName, EntryPoint = "canalis_get_sample_rate")]
    public static extern int GetSampleRate(IntPtr instance);

    [DllImport(dllName, EntryPoint = "canalis_get_sample_format")]
    public static extern SampleFormat GetSampleFormat(IntPtr instance);

    [DllImport(dllName, EntryPoint = "canalis_get_channels")]
    public static extern int GetChannels(IntPtr instance);
    
    [DllImport(dllName, EntryPoint = "canalis_get_byte_count")]
    public static extern uint GetByteCount(IntPtr instance);

    [DllImport(dllName, EntryPoint = "canalis_get_position")]
    public static extern uint GetPosition(IntPtr instance);

    [DllImport(dllName, EntryPoint = "canalis_set_position")]
    public static extern void SetPosition(IntPtr instance, uint position);

    [DllImport(dllName, EntryPoint = "canalis_read")]
    private static unsafe extern void Read(IntPtr instance, void* buf, int sizeInBytes, out int readBytes);

    public static void Read(IntPtr instance, short[] buf, out int readBytes) {
        unsafe {
            fixed (void* pbuf = buf) {
                Read(instance, pbuf, buf.Length * 2, out readBytes);
            }
        }
    }

    public static void Read(IntPtr instance, int[] buf, out int readBytes) {
        unsafe {
            fixed (void* pbuf = buf) {
                Read(instance, pbuf, buf.Length * 4, out readBytes);
            }
        }
    }

    public static void Read(IntPtr instance, float[] buf, out int readBytes) {
        unsafe {
            fixed (void* pbuf = buf) {
                Read(instance, pbuf, buf.Length * 4, out readBytes);
            }
        }
    }
    
    [DllImport(dllName, EntryPoint = "canalis_free")]
    public static extern void Free(IntPtr instance);
}
