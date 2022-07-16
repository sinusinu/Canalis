# Canalis

Canalis is a public domain WAV/MP3/OGG decoder made for use in C#, powered by [`dr_wav`, `dr_mp3`](https://github.com/mackron/dr_libs), and [`stb_vorbis`](https://github.com/nothings/stb/) public domain C libraries.

This library does not provide any way to playback decoded samples. You must bring your own audio device.

# Disclaimer

Just like [Flora](https://github.com/sinusinu/Flora), Canalis is created mainly for my personal use. It may lack some basic functionalities you might expect.

# How to use

Canalis is consisted of two parts:
- Native code written in C
- C# glue of the native code

## Native code written in C

Inside `native` folder is a shared library written in C which is a dumb wrapper of `dr_wav`, `dr_mp3`, and `stb_vorbis`, designed with P/Invoke in mind.

While this could be used as a library for other C projects,

- this whole thing is extremely poorly written, and
- you have much better options if you are on C anyway

so I won't recommend using this on other C projects.

You can grab the prebuilt `canalis-native.dll` from `libs` folder or build one yourself, and take it to your C# project with:

## C# glue of the native code

`Canalis.cs` is the glue file that P/Invokes the `canalis-native.dll`. It uses unsafe blocks so that must be enabled if you want to include `Canalis.cs` file in your project directly.

If you prefer adding Canalis as a external reference (e.g. you don't want to enable unsafe blocks on your project), you can either:
- `dotnet add reference` this project, or
- `dotnet build` and reference the output `Canalis.dll` file.

Either way, put `canalis-native.dll` and glue project together in your project, and it should be ready to go!

Refer to `CanalisExample.cs` for example usage.

# Questions That Are Probably Floating On Your Mind

### Why P/Invoke? There already are fully-managed audio file decoding libraries!

I know and appreciate those libraries, but they were a bit too much for my specific use case. I wanted to have something minimal and specific, while avoiding pain of decoding complex compression of some audio formats by myself.

I could use part of these projects, but then I have to deal with license stuff. My laziness didn't want to deal with it.

### Two DLLs for single library sounds dumb.

ikr?

### Is this library reliable?

Probably. Probably not. I don't know. and I don't care about its reliability. It works for my specific use case; that is all I need.

# License

Canalis is distributed as a public domain software. Check out the LICENSE file for more info.

Canalis is powered by [`dr_wav`, `dr_mp3`](https://github.com/mackron/dr_libs), [`stb_vorbis`](https://github.com/nothings/stb/) public domain C libraries.