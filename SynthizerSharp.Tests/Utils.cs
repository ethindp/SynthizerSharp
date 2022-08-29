using static System.Math;
namespace Synthizer.Tests.Utils;
public sealed class AudioGenerator {
public static ReadOnlyMemory<float> GenerateSineWaveArray(in float amplitude, in float samplerate, in float frequency, in UInt64 sample_count) {
var samples = new float[sample_count];
float phi = 0.0f;
float delta = 2.0f * (float)PI * frequency / samplerate;
for (int i = 0; i < samples.Length; i++) {
samples[i] = amplitude * (float)Sin(phi);
phi += delta;
}
return samples;
}

public static ReadOnlyMemory<float> GenerateSweptSineWaveArray(in float amplitude, in float samplerate, in float initial_frequency, in float final_frequency, in float sweep_duration, in UInt64 samples_count) {
var samples = new float[samples_count];
float phi = 0.0f;
float f = initial_frequency;
float delta = 2.0f * (float)PI * f / samplerate;
float f_delta = (final_frequency - initial_frequency) / (samplerate * sweep_duration);
for (int i = 0; i < samples.Length; i++) {
samples[i] = amplitude * (float)Sin(phi);
phi += delta;
f += f_delta;
delta = 2.0f * (float)PI * f / samplerate;
}
return samples;
}
}

public sealed class FFIActivator {
public static IRawSynthizer ActivateFFIInterface() {
var activator = new NativeLibraryBuilder();
if (OS.IsAndroid() || OS.IsFreeBSD() || OS.IsLinux()) {
var library = activator.ActivateInterface<IRawSynthizer>("libsynthizer.so");
return library;
} else if (OS.IsMacOS() || OS.IsIOS() || OS.IsMacCatalyst() || OS.IsTvOS() || OS.IsWatchOS()) {
var library = activator.ActivateInterface<IRawSynthizer>("synthizer.dylib");
return library;
} else if (OS.IsWindows()) {
var library = activator.ActivateInterface<IRawSynthizer>("synthizer.dll");
return library;
} else {
throw new NotSupportedException("OS not supported");
}
}
}

