using AdvancedDLSupport;
using System;
using System.Runtime.InteropServices;

namespace Synthizer.FFI;
public struct UserAutomationEvent
{
    public ulong Param;
}

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct UserAutomationEventPayload
{
    [FieldOffset(0)]
    public UserAutomationEvent UserAutomation;
}

public struct AutomationEvent
{
    public EventType Type;
    public ulong Source;
    public ulong Context;
    public UserAutomationEventPayload Payload;
}

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct AutomationCommandParameters
{
    [FieldOffset(0)]
    public AutomationAppendPropertyCommand AppendToProperty;
    [FieldOffset(0)]
    public AutomationClearPropertyCommand ClearProperty;
    [FieldOffset(0)]
    public AutomationSendUserEventCommand SendUserEvent;
}

public struct AutomationCommandData
{
    public ulong Target;
    public double Time;
    public AutomationCommand Command;
    public uint Flags;
    public AutomationCommandParameters Parameters;
}

public enum LoggingBackend
{
    None,
    Stderr
}

public enum LogLevel
{
    Error,
    Warn = 10,
    Info = 20,
    Debug = 30
}

public struct LibraryConfig
{
    public LogLevel LogLevel;
    public LoggingBackend LoggingBackend;
    [MarshalAs(UnmanagedType.LPStr)]
    public string LibsndfilePath;
}

public struct DeleteBehaviorConfig
{
    public int Linger;
    public double LingerTimeout;
}

public struct BiquadConfig
{
    public double B0;
    public double B1;
    public double B2;
    public double A0;
    public double A1;
    public double Gain;
    public bool IsWire;
}

public unsafe struct AutomationPoint
{
    public InterpolationType InterpolationType;
    public fixed double Values[6];
    public ulong Flags;
}

public struct AutomationAppendPropertyCommand
{
    public Property Property;
    public AutomationPoint Point;
}

public struct AutomationClearPropertyCommand
{
    public Property Property;
}

public struct AutomationSendUserEventCommand
{
    public ulong Param;
}

public struct SineBankWave
{
    public double FrequencyMultiplier;
    public double Phase;
    public double Gain;
}

public unsafe struct SineBankConfig
{
    public SineBankWave* Waves;
    public ulong WaveCount;
    public double InitialFrequency;
}

public struct RouteConfig
{
    public double Gain;
    public double FadeTime;
    public BiquadConfig Filter;
}

public struct EchoTapConfig
{
    public double Delay;
    public double GainLeft;
    public double GainRight;
}

public enum ObjectType
{
    Context,
    Buffer,
    BufferGenerator,
    StreamingGenerator,
    NoiseGenerator,
    DirectSource,
    AngularPannedSource,
    ScalarPannedSource,
    Source3D,
    GlobalEcho,
    GlobalFDNReverb,
    StreamHandle,
    AutomationBatch,
    FastSineBankGenerator
}

public enum PannerStrategy
{
    Delegate,
    HRTF,
    Stereo
}

public enum DistanceModel
{
    None,
    Linear,
    Exponential,
    Inverse
}

public enum NoiseType
{
    Uniform,
    VM,
    FilteredBrown
}

public enum Property
{
    Azimuth,
    Buffer,
    Elevation,
    Gain,
    DefaultPannerStrategy,
    PanningScalar,
    PlaybackPosition,
    Position,
    Orientation,
    ClosenessBoost,
    ClosenessBoostDistance,
    DistanceMax,
    DistanceModel,
    DistanceRef,
    Rolloff,
    DefaultClosenessBoost,
    DefaultClosenessBoostDistance,
    DefaultDistanceMax,
    DefaultDistanceModel,
    DefaultDistanceRef,
    DefaultRolloff,
    Looping,
    NoiseType,
    PitchBend,
    InputFilterEnabled,
    InputFilterCutoff,
    MeanFreePath,
    T60,
    LateReflectionsLowFrequencyRolloff,
    LateReflectionsLowFrequencyReference,
    LateReflectionsHighFrequencyRolloff,
    LateReflectionsHighFrequencyReference,
    LateReflectionsDiffusion,
    LateReflectionsModulationDepth,
    LateReflectionsModulationFrequency,
    LateReflectionsDelay,
    Filter,
    FilterDirect,
    FilterEffects,
    FilterInput,
    CurrentTime,
    SuggestedAutomationTime,
    Frequency
}

public enum EventType
{
    Invalid,
    Looped,
    Finished,
    UserAutomation
}

public enum InterpolationType
{
    None,
    Linear
}

public enum AutomationCommand
{
    AppendProperty,
    SendUserEvent,
    ClearProperty,
    ClearEvents,
    ClearAllProperties
}

public unsafe delegate void UserDataFreeCallback(void* data);
public unsafe delegate int StreamReadCallback(out ulong read, ulong requested, out Span<byte> destination, void* userdata, [MarshalAs(UnmanagedType.LPStr)] out string errmsg);
public unsafe delegate int StreamSeekCallback(ulong pos, void* userdata, out string errmsg);
public unsafe delegate int StreamCloseCallback(void* userdata, [MarshalAs(UnmanagedType.LPStr)] out string errmsg);
public unsafe delegate int StreamDestroyCallback(void* userdata);
public unsafe delegate int StreamOpenCallback(CustomStreamDefinition callbacks, [MarshalAs(UnmanagedType.LPStr)] string protocol, [MarshalAs(UnmanagedType.LPStr)] string path, void* param, void* userdata, [MarshalAs(UnmanagedType.LPStr)] out string errmsg);

public unsafe struct CustomStreamDefinition
{
    public StreamReadCallback Read;
    public StreamSeekCallback Seek;
    public StreamCloseCallback Close;
    public StreamDestroyCallback Destroy;
    public long length;
    public void* userdata;
}

public interface IRawSynthizer : IDisposable
{
    void syz_getVersion(out ulong major, out ulong minor, out ulong patch);
    void syz_eventDeinit(ref AutomationEvent evt);
    void syz_libraryConfigSetDefaults(out LibraryConfig config);
    nint syz_initialize();
    nint syz_initializeWithConfig(ref LibraryConfig config);
    nint syz_shutdown();
    nint syz_getLastErrorCode();
    [return: MarshalAs(UnmanagedType.LPStr)]
    string syz_getLastErrorMessage();
    nint syz_handleIncRef(ulong handle);
    nint syz_handleDecRef(ulong handle);
    void syz_initDeleteBehaviorConfig(out DeleteBehaviorConfig config);
    nint syz_configDeleteBehavior(ulong obj, ref DeleteBehaviorConfig cfg);
    nint syz_handleGetObjectType(out ObjectType type, ulong handle);
    unsafe nint syz_handleGetUserdata(out void* data, ulong handle);
    unsafe nint syz_handleSetUserdata(ulong handle, void* userdata, UserDataFreeCallback? callback);
    nint syz_pause(ulong handle);
    nint syz_play(ulong handle);
    nint syz_getI(out nint value, ulong handle, Property property);
    nint syz_setI(ulong target, Property property, nint value);
    nint syz_getD(out double value, ulong handle, Property property);
    nint syz_setD(ulong target, Property property, double value);
    nint syz_setO(ulong target, Property property, ulong value);
    nint syz_getD3(out double x, out double y, out double z, ulong handle, Property property);
    nint syz_setD3(ulong handle, Property property, double x, double y, double z);
    nint syz_getD6(out double x, out double y, out double z, out double x2, out double y2, out double z2, ulong handle, Property property);
    nint syz_setD6(ulong handle, Property property, double x, double y, double z, double x2, double y2, double z2);
    nint syz_getBiquad(out BiquadConfig filter, ulong handle, Property property);
    nint syz_setBiquad(ulong handle, Property property, ref BiquadConfig filter);
    nint syz_biquadDesignIdentity(out BiquadConfig filter);
    nint syz_biquadDesignLowpass(out BiquadConfig filter, double frequency, double q);
    nint syz_biquadDesignHighpass(out BiquadConfig filter, double frequency, double q);
    nint syz_biquadDesignBandpass(out BiquadConfig filter, double frequency, double bw);
    unsafe nint syz_createContext(out ulong handle, void* userdata, UserDataFreeCallback? UserdataFreeCallback);
    unsafe nint syz_createContextHeadless(out ulong handle, void* userdata, UserDataFreeCallback? UserdataFreeCallback);
    nint syz_contextGetBlock(ulong handle, out Memory<float> block);
    nint syz_contextEnableEvents(ulong handle);
    nint syz_contextGetNextEvent(out AutomationEvent evt, ulong context, ulong flags);
    unsafe nint syz_registerStreamProtocol([MarshalAs(UnmanagedType.LPStr)] string protocol, StreamOpenCallback callback, void* userdata);
    unsafe nint syz_createStreamHandleFromStreamParams(out ulong handle, [MarshalAs(UnmanagedType.LPStr)] string protocol, [MarshalAs(UnmanagedType.LPStr)] string path, void* param, void* userdata, UserDataFreeCallback? callback);
    unsafe nint syz_createStreamHandleFromMemory(out ulong handle, ulong len, Memory<byte> data, void* userdata, UserDataFreeCallback? callback);
    unsafe nint syz_createStreamHandleFromFile(out ulong handle, [MarshalAs(UnmanagedType.LPStr)] string path, void* userdata, UserDataFreeCallback? callback);
    unsafe nint syz_createStreamHandleFromCustomStream(out ulong handle, ref CustomStreamDefinition callbacks, void* userdata, UserDataFreeCallback? callback);
    unsafe nint syz_createStreamingGeneratorFromStreamParams(out ulong handle, ulong context, [MarshalAs(UnmanagedType.LPStr)] string protocol, [MarshalAs(UnmanagedType.LPStr)] string path, void* param, void* config, void* userdata, UserDataFreeCallback? callback);
    unsafe nint syz_createStreamingGeneratorFromFile(out ulong handle, ulong context, [MarshalAs(UnmanagedType.LPStr)] string path, void* config, void* userdata, UserDataFreeCallback? callback);
    unsafe nint syz_createStreamingGeneratorFromStreamHandle(out ulong handle, ulong context, ulong stream_handle, void* config, void* userdata, UserDataFreeCallback? callback);
    unsafe nint syz_createBufferFromStreamParams(out ulong handle, [MarshalAs(UnmanagedType.LPStr)] string protocol, [MarshalAs(UnmanagedType.LPStr)] string path, void* param, void* userdata, UserDataFreeCallback? callback);
    unsafe nint syz_createBufferFromEncodedData(out ulong handle, ulong len, Memory<byte> data, void* userdata, UserDataFreeCallback? callback);
    unsafe nint syz_createBufferFromFloatArray(out ulong handle, nuint sr, nuint channels, ulong frames, Memory<float> data, void* userdata, UserDataFreeCallback? callback);
    unsafe nint syz_createBufferFromFile(out ulong handle, [MarshalAs(UnmanagedType.LPStr)] string path, void* userdata, UserDataFreeCallback? callback);
    unsafe nint syz_createBufferFromStreamHandle(out ulong handle, ulong stream, void* userdata, UserDataFreeCallback? callback);
    nint syz_bufferGetChannels(out uint channels, ulong handle);
    nint syz_bufferGetLengthInSamples(out uint samples, ulong handle);
    nint syz_bufferGetLengthInSeconds(out double seconds, ulong handle);
    nint syz_bufferGetSizeInBytes(out ulong size, ulong handle);
    unsafe nint syz_createBufferGenerator(out ulong handle, ulong context, void* config, void* userdata, UserDataFreeCallback? callback);
    nint syz_sourceAddGenerator(ulong source, ulong generator);
    nint syz_sourceRemoveGenerator(ulong source, ulong generator);
    unsafe nint syz_createDirectSource(out ulong handle, ulong context, void* config, void* userdata, UserDataFreeCallback? callback);
    unsafe nint syz_createAngularPannedSource(out ulong handle, ulong context, PannerStrategy strategy, double azimuth, double elevation, void* config, void* userdata, UserDataFreeCallback? callback);
    unsafe nint syz_createScalarPannedSource(out ulong handle, ulong context, PannerStrategy strategy, double scalar, void* config, void* userdata, UserDataFreeCallback? callback);
    unsafe nint syz_createSource3D(out ulong handle, ulong context, PannerStrategy strategy, double x, double y, double z, void* config, void* userdata, UserDataFreeCallback? callback);
    unsafe nint syz_createNoiseGenerator(out ulong handle, ulong context, nuint channels, void* config, void* userdata, UserDataFreeCallback? callback);
    void syz_initSineBankConfig(out SineBankConfig cfg);
    unsafe nint syz_createFastSineBankGenerator(out ulong handle, ulong context, ref SineBankConfig cfg, void* config, void* userdata, UserDataFreeCallback? callback);
    unsafe nint syz_createFastSineBankGeneratorSine(out ulong handle, ulong context, double initial_frequency, void* config, void* userdata, UserDataFreeCallback? callback);
    unsafe nint syz_createFastSineBankGeneratorTriangle(out ulong handle, ulong context, double initial_frequency, nuint partials, void* config, void* userdata, UserDataFreeCallback? callback);
    unsafe nint syz_createFastSineBankGeneratorSquare(out ulong handle, ulong context, double initial_frequency, uint partials, void* config, void* userdata, UserDataFreeCallback? callback);
    unsafe nint syz_createFastSineBankGeneratorSaw(out ulong handle, ulong context, double initial_frequency, uint partials, void* config, void* userdata, UserDataFreeCallback? callback);
    void syz_initRouteConfig(out RouteConfig cfg);
    nint syz_routingConfigRoute(ulong context, ulong output, ulong input, ref RouteConfig config);
    nint syz_routingRemoveRoute(ulong context, ulong output, ulong input, double fade_out);
    nint syz_routingRemoveAllRoutes(ulong context, ulong output, double fade_out);
    nint syz_effectReset(ulong handle);
    unsafe nint syz_createGlobalEcho(out ulong handle, ulong context, void* config, void* userdata, UserDataFreeCallback? callback);
    nint syz_globalEchoSetTaps(ulong handle, nuint len, Memory<EchoTapConfig> taps);
    unsafe nint syz_createGlobalFdnReverb(out ulong handle, ulong context, void* config, void* userdata, UserDataFreeCallback? callback);
    unsafe nint syz_createAutomationBatch(out ulong handle, ulong context, void* userdata, UserDataFreeCallback? callback);
    nint syz_automationBatchAddCommands(ulong batch, ulong commands_count, Memory<AutomationCommandData> commands);
    nint syz_automationBatchExecute(ulong batch);
}

