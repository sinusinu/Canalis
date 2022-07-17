using System;
using static Canalis.CanalisInterop;

namespace Canalis;

public class CanalisInstance : IDisposable {
    IntPtr instance;

    bool _disposed;

    public CanalisInstance() {
        instance = CanalisInterop.Create();
    }

    public CanalisInstance(string path, SampleFormat format) {
        instance = CanalisInterop.Create();
        Load(path, format);
    }

    ~CanalisInstance() => Dispose(false);

    public void Load(string path, SampleFormat format) {
        string extension = path.ToLower().Substring(path.Length - 3);
        switch (extension) {
            case "wav":
                CanalisInterop.LoadWav(instance, path, format);
                break;
            case "mp3":
                CanalisInterop.LoadMp3(instance, path, format);
                break;
            case "ogg":
                CanalisInterop.LoadVorbis(instance, path, format);
                break;
            default:
                throw new ArgumentException("path has invalid extension, must be one of wav/mp3/ogg");
        }

        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to load file: " + GetLastError(instance));
    }

    public int GetSampleRate() {
        int sampleRate = CanalisInterop.GetSampleRate(instance);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to get sample rate: " + GetLastError(instance));
        return sampleRate;
    }

    public SampleFormat GetSampleFormat() {
        SampleFormat sampleFormat = CanalisInterop.GetSampleFormat(instance);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to get sample format: " + GetLastError(instance));
        return sampleFormat;
    }

    public int GetChannels() {
        int channels = CanalisInterop.GetChannels(instance);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to get channels: " + GetLastError(instance));
        return channels;
    }

    public uint GetByteCount() {
        uint byteCount = CanalisInterop.GetByteCount(instance);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to get byte count: " + GetLastError(instance));
        return byteCount;
    }

    public uint GetPosition() {
        uint position = CanalisInterop.GetPosition(instance);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to get position: " + GetLastError(instance));
        return position;
    }

    public void SetPosition(uint position) {
        CanalisInterop.SetPosition(instance, position);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to set position: " + GetLastError(instance));
    }

    public void Read(short[] buf) {
        SampleFormat sampleFormat = CanalisInterop.GetSampleFormat(instance);
        if (sampleFormat != SampleFormat.S16) throw new ArgumentException("Sample format mismatch, buf should be " + sampleFormat);

        CanalisInterop.Read(instance, buf);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to read: " + GetLastError(instance));
    }

    public void Read(short[] buf, out int readBytes) {
        SampleFormat sampleFormat = CanalisInterop.GetSampleFormat(instance);
        if (sampleFormat != SampleFormat.S16) throw new ArgumentException("Sample format mismatch, buf should be " + sampleFormat);

        CanalisInterop.Read(instance, buf, out readBytes);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to read: " + GetLastError(instance));
    }

    public void Read(int[] buf) {
        SampleFormat sampleFormat = CanalisInterop.GetSampleFormat(instance);
        if (sampleFormat != SampleFormat.S32) throw new ArgumentException("Sample format mismatch, buf should be " + sampleFormat);

        CanalisInterop.Read(instance, buf);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to read: " + GetLastError(instance));
    }

    public void Read(int[] buf, out int readBytes) {
        SampleFormat sampleFormat = CanalisInterop.GetSampleFormat(instance);
        if (sampleFormat != SampleFormat.S32) throw new ArgumentException("Sample format mismatch, buf should be " + sampleFormat);

        CanalisInterop.Read(instance, buf, out readBytes);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to read: " + GetLastError(instance));
    }

    public void Read(float[] buf) {
        SampleFormat sampleFormat = CanalisInterop.GetSampleFormat(instance);
        if (sampleFormat != SampleFormat.F32) throw new ArgumentException("Sample format mismatch, buf should be " + sampleFormat);

        CanalisInterop.Read(instance, buf);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to read: " + GetLastError(instance));
    }

    public void Read(float[] buf, out int readBytes) {
        SampleFormat sampleFormat = CanalisInterop.GetSampleFormat(instance);
        if (sampleFormat != SampleFormat.F32) throw new ArgumentException("Sample format mismatch, buf should be " + sampleFormat);

        CanalisInterop.Read(instance, buf, out readBytes);
        if (GetLastError(instance) != Error.Success) throw new InvalidOperationException("Failed to read: " + GetLastError(instance));
    }

    protected virtual void Dispose(bool disposing) {
        if (_disposed) return;
        CanalisInterop.Free(instance);
        _disposed = true;
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}