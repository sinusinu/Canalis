using System;

namespace CanalisLib;

class CanalisExample {
    /*
    Output should be something like this:

    Success
    Success
    22050 2 701512 S16
    Read 32 bytes: 0 0 0 0 -1 -1 -1 0 -1 0 0 0 0 1 0 -1
    Read 32 bytes: -1 0 -1 0 -1 0 0 -1 0 -1 -1 -1 -1 -1 -2 0
    Read 32 bytes: -2 -1 -1 0 -2 -1 -1 0 -1 0 -2 0 -1 0 -1 0
    Read 32 bytes: -1 0 -1 0 -1 0 -1 0 -2 0 -1 0 -1 1 -1 0
    Read 32 bytes: -1 0 -1 0 -2 0 -1 0 0 0 -1 0 0 0 -1 0
    Read 32 bytes: -1 0 -1 0 0 1 -1 0 -1 0 0 0 0 0 0 0
    Read 32 bytes: -1 -1 0 0 -1 1 0 0 0 -1 0 1 0 0 0 0
    Read 32 bytes: -1 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0
    Read 32 bytes: 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 1
    Read 32 bytes: 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
    Read 32 bytes: 0 0 0 0 0 0 0 0 1 -1 1 -1 0 -1 0 0
    Read 32 bytes: 1 0 1 0 0 0 0 0 1 1 1 0 0 0 1 0
    Read 32 bytes: 1 0 1 0 1 0 1 0 2 0 1 0 2 0 1 0
    Read 32 bytes: 1 0 2 0 2 0 2 0 2 0 1 0 1 0 2 0
    Read 32 bytes: 2 -1 2 0 2 0 1 0 1 -1 1 -1 2 -1 1 -1
    Read 32 bytes: -2 -1 -1 0 1 0 0 0 -1 -1 0 0 -1 -1 0 0
    Seeking...
    Read 32 bytes: -2 -1 -1 0 -1 0 -2 0 -1 0 -1 0 -1 0 -1 0
    Read 32 bytes: -1 0 -1 0 -2 0 -1 0 -1 1 -1 0 -1 0 -1 0
    Read 32 bytes: -2 0 -1 0 0 0 -1 0 0 0 -1 0 -1 0 -1 0
    Read 32 bytes: 0 1 -1 0 -1 0 0 0 0 0 0 0 -1 -1 0 0
    Read 32 bytes: -1 1 0 0 0 -1 0 1 0 0 0 0 -1 0 0 0
    Read 32 bytes: 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0
    Read 32 bytes: 1 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0
    Read 32 bytes: 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
    Read 32 bytes: 0 0 0 0 1 -1 1 -1 0 -1 0 0 1 0 1 0
    Read 32 bytes: 0 0 0 0 1 1 1 0 0 0 1 0 1 0 1 0
    Read 32 bytes: 1 0 1 0 2 0 1 0 2 0 1 0 1 0 2 0
    Read 32 bytes: 2 0 2 0 2 0 1 0 1 0 2 0 2 -1 2 0
    Read 32 bytes: 2 0 1 0 1 -1 1 -1 2 -1 1 -1 -2 -1 -1 0
    Read 32 bytes: 1 0 0 0 -1 -1 0 0 -1 -1 0 0 0 0 -1 0
    Read 32 bytes: 0 0 1 0 0 -1 1 0 -1 -1 0 1 -1 0 1 0
    Read 32 bytes: 0 0 0 0 1 0 0 0 0 0 0 -1 1 0 1 0
    */
    static void Main(string[] args) {
        IntPtr instance = Canalis.Create();
        Console.WriteLine(Canalis.GetLastError(instance));
        Canalis.LoadWav(instance, "thing.wav", Canalis.SampleFormat.S16);
        Console.WriteLine(Canalis.GetLastError(instance));

        Console.WriteLine(
            "{0} {1} {2} {3}",
            Canalis.GetSampleRate(instance),
            Canalis.GetChannels(instance),
            Canalis.GetByteCount(instance),
            Canalis.GetSampleFormat(instance)
        );

        // buffer type should match the sample format specified when loading file
        // use short for S16, int for S32, float for F32
        short[] buf = new short[16];
        int readBytes;
        
        for (int i = 0; i < 16; i++) {
            Canalis.Read(instance, buf, out readBytes);
            Console.Write("Read {0} bytes: ", readBytes);
            for (int j = 0; j < 16; j++) Console.Write(buf[j] + " ");
            Console.WriteLine();
        }

        Console.WriteLine("Seeking...");
        Canalis.SetPosition(instance, 72);

        for (int i = 0; i < 16; i++) {
            Canalis.Read(instance, buf, out readBytes);
            Console.Write("Read {0} bytes: ", readBytes);
            for (int j = 0; j < 16; j++) Console.Write(buf[j] + " ");
            Console.WriteLine();
        }

        Canalis.Free(instance);
    }
}