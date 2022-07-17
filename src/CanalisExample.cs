using System;

namespace Canalis;

class CanalisExample {
    /*
    Output should be something like this:

    22050 2 314272 S16
    Read 32 bytes: 0 0 -1 0 0 0 1 0 0 0 0 0 0 1 1 1
    Read 32 bytes: 0 1 1 1 0 1 1 1 0 1 1 2 0 2 0 2
    Read 32 bytes: 0 2 0 2 0 2 0 2 0 2 0 2 0 1 -1 1
    Read 32 bytes: -1 0 -1 0 -2 -1 -2 -1 -2 -2 -3 -3 -3 -3 -3 -4
    Read 32 bytes: -3 -4 -4 -5 -4 -6 -4 -6 -4 -6 -4 -6 -3 -6 -3 -6
    Read 32 bytes: -3 -6 -3 -6 -3 -6 -2 -5 -2 -4 -1 -4 -2 -3 -1 -2
    Read 32 bytes: -1 -1 0 -1 0 0 0 1 1 2 1 3 2 4 2 5
    Read 32 bytes: 4 6 4 7 5 8 6 8 7 9 7 9 8 9 8 9
    Read 32 bytes: 9 9 9 8 9 8 9 8 9 7 10 6 9 6 10 5
    Read 32 bytes: 10 4 9 3 7 2 6 2 5 0 4 -1 2 -2 0 -4
    Read 32 bytes: -2 -5 -4 -6 -5 -7 -7 -8 -10 -10 -11 -11 -12 -12 -14 -14
    Read 32 bytes: -14 -14 -16 -15 -16 -14 -18 -16 -18 -15 -18 -14 -19 -14 -18 -13
    Read 32 bytes: -18 -12 -17 -10 -16 -9 -14 -6 -12 -4 -10 -2 -7 0 -5 2
    Read 32 bytes: -2 4 0 6 3 8 5 10 8 11 11 12 13 13 15 14
    Read 32 bytes: 17 15 18 14 19 15 20 14 20 13 20 13 19 12 19 11
    Read 32 bytes: 18 10 17 9 16 9 15 8 14 7 13 6 11 5 10 4
    Seeking...
    Read 32 bytes: -133 100 -115 122 -97 140 -79 158 -60 175 -41 190 -20 205 2 220
    Read 32 bytes: 25 235 50 250 71 264 93 274 112 281 129 281 140 276 149 259
    Read 32 bytes: 155 234 156 200 157 159 149 111 138 58 132 5 122 -49 111 -98
    Read 32 bytes: 100 -142 86 -180 73 -210 62 -232 50 -247 37 -255 26 -257 17 -255
    Read 32 bytes: 9 -249 -1 -240 -8 -226 -18 -211 -29 -188 -43 -165 -58 -137 -75 -106
    Read 32 bytes: -91 -73 -105 -39 -116 -4 -123 29 -125 60 -122 88 -116 112 -107 132
    Read 32 bytes: -98 148 -87 161 -77 170 -66 177 -55 181 -46 184 -35 185 -24 186
    Read 32 bytes: -12 187 1 186 14 187 27 187 40 187 54 183 65 176 74 166
    Read 32 bytes: 81 152 84 133 85 108 84 78 80 43 76 4 70 -37 62 -79
    Read 32 bytes: 56 -118 49 -154 43 -181 37 -203 31 -214 28 -219 24 -219 21 -213
    Read 32 bytes: 19 -206 16 -196 13 -186 10 -174 4 -161 -3 -145 -11 -128 -21 -107
    Read 32 bytes: -32 -85 -47 -57 -60 -30 -73 2 -85 32 -93 62 -98 91 -98 116
    Read 32 bytes: -95 139 -88 155 -79 167 -69 175 -56 179 -44 181 -32 180 -21 178
    Read 32 bytes: -9 175 0 172 10 169 18 166 28 164 36 161 44 158 51 151
    Read 32 bytes: 56 142 62 130 64 111 65 86 64 59 61 26 57 -12 52 -52
    */
    static void Main(string[] args) {
        CanalisInstance canalis = new CanalisInstance("thing.wav", CanalisInterop.SampleFormat.S16);

        Console.WriteLine(
            "{0} {1} {2} {3}",
            canalis.GetSampleRate(),
            canalis.GetChannels(),
            canalis.GetByteCount(),
            canalis.GetSampleFormat()
        );

        // buffer type should match the sample format specified when loading file
        // use short for S16, int for S32, float for F32
        short[] buf = new short[16];
        int readBytes;
        
        for (int i = 0; i < 16; i++) {
            canalis.Read(buf, out readBytes);
            Console.Write("Read {0} bytes: ", readBytes);
            for (int j = 0; j < 16; j++) Console.Write(buf[j] + " ");
            Console.WriteLine();
        }

        Console.WriteLine("Seeking...");
        canalis.SetPosition(16384);

        for (int i = 0; i < 16; i++) {
            canalis.Read(buf, out readBytes);
            Console.Write("Read {0} bytes: ", readBytes);
            for (int j = 0; j < 16; j++) Console.Write(buf[j] + " ");
            Console.WriteLine();
        }

        canalis.Dispose();
    }
}