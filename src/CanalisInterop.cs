using System;
using System.Runtime.InteropServices;

namespace Canalis;

public class CanalisInterop {
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
    internal static extern IntPtr Create();

    [DllImport(dllName, EntryPoint = "canalis_load_wav")]
    internal static extern void LoadWav(IntPtr instance, string path, SampleFormat format);

    [DllImport(dllName, EntryPoint = "canalis_load_mp3")]
    internal static extern void LoadMp3(IntPtr instance, string path, SampleFormat format);

    [DllImport(dllName, EntryPoint = "canalis_load_vorbis")]
    internal static extern void LoadVorbis(IntPtr instance, string path, SampleFormat format);

    [DllImport(dllName, EntryPoint = "canalis_get_last_error")]
    internal static extern Error GetLastError(IntPtr instance);

    [DllImport(dllName, EntryPoint = "canalis_get_state")]
    internal static extern State GetState(IntPtr instance);

    [DllImport(dllName, EntryPoint = "canalis_get_sample_rate")]
    internal static extern int GetSampleRate(IntPtr instance);

    [DllImport(dllName, EntryPoint = "canalis_get_sample_format")]
    internal static extern SampleFormat GetSampleFormat(IntPtr instance);

    [DllImport(dllName, EntryPoint = "canalis_get_channels")]
    internal static extern int GetChannels(IntPtr instance);
    
    [DllImport(dllName, EntryPoint = "canalis_get_byte_count")]
    internal static extern uint GetByteCount(IntPtr instance);

    [DllImport(dllName, EntryPoint = "canalis_get_position")]
    internal static extern uint GetPosition(IntPtr instance);

    [DllImport(dllName, EntryPoint = "canalis_set_position")]
    internal static extern void SetPosition(IntPtr instance, uint position);

    [DllImport(dllName, EntryPoint = "canalis_read")]
    internal static unsafe extern void Read(IntPtr instance, void* buf, int sizeInBytes, out int readBytes);

    [DllImport(dllName, EntryPoint = "canalis_read")]
    internal static unsafe extern void Read(IntPtr instance, void* buf, int sizeInBytes, IntPtr nul);

    internal static void Read(IntPtr instance, short[] buf, out int readBytes) {
        unsafe {
            fixed (void* pbuf = buf) {
                Read(instance, pbuf, buf.Length * 2, out readBytes);
            }
        }
    }

    internal static void Read(IntPtr instance, short[] buf) {
        unsafe {
            fixed (void* pbuf = buf) {
                Read(instance, pbuf, buf.Length * 2, IntPtr.Zero);
            }
        }
    }

    internal static void Read(IntPtr instance, int[] buf, out int readBytes) {
        unsafe {
            fixed (void* pbuf = buf) {
                Read(instance, pbuf, buf.Length * 4, out readBytes);
            }
        }
    }

    internal static void Read(IntPtr instance, int[] buf) {
        unsafe {
            fixed (void* pbuf = buf) {
                Read(instance, pbuf, buf.Length * 4, IntPtr.Zero);
            }
        }
    }

    internal static void Read(IntPtr instance, float[] buf, out int readBytes) {
        unsafe {
            fixed (void* pbuf = buf) {
                Read(instance, pbuf, buf.Length * 4, out readBytes);
            }
        }
    }

    internal static void Read(IntPtr instance, float[] buf) {
        unsafe {
            fixed (void* pbuf = buf) {
                Read(instance, pbuf, buf.Length * 4, IntPtr.Zero);
            }
        }
    }
    
    [DllImport(dllName, EntryPoint = "canalis_free")]
    internal static extern void Free(IntPtr instance);
}
