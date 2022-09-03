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
    public int Source;
    public int Context;
    public UserAutomationEventPayload Payload;
}

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct AutomationCommandParameters
{
    [FieldOffset(0)]
    public AutomationAppendPropertyCommand AppendToProperty;
    [FieldOffset(sizeof(AutomationAppendPropertyCommand))]
    public AutomationClearPropertyCommand ClearProperty;
    [FieldOffset(sizeof(AutomationAppendPropertyCommand) + sizeof(AutomationClearPropertyCommand))]
    public AutomationSendUserEventCommand SendUserEvent;
}

public struct automationCommandData
{
    public int Target;
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

public struct AutomationPoint
{
    public int InterpolationType;
    public Span<double> Values;
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

public struct SineBankConfig
{
    public Span<SineBankWave> Waves;
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

public delegate void UserDataFreeCallback(Span<byte> data);
public delegate int StreamReadCallback(out ulong read, ulong requested, out Span<byte> destination, Span<byte>? userdata, out string errmsg);
public delegate int StreamSeekCallback(ulong pos, Span<byte>? userdata, out string errmsg);
public delegate int StreamCloseCallback(Span<byte>? userdata, out string errmsg);
public delegate int StreamDestroyCallback(Span<byte>? userdata);
public delegate int StreamOpenCallback(CustomStreamDefinition callbacks, string protocol, string path, Span<byte>? param, Span<byte>? userdata, out string errmsg);

public struct CustomStreamDefinition
{
    public StreamReadCallback Read;
    public StreamSeekCallback Seek;
    public StreamCloseCallback Close;
    public StreamDestroyCallback Destroy;
    public long length;
    public Span<byte>? userdata;
}

public interface IRawSynthizer : IDisposable
{
    void syz_getVersion(out ulong major, out ulong minor, out ulong patch);
    void syz_eventDeinit(ref AutomationEvent evt);
    void syz_libraryConfigSetDefaults(out LibraryConfig config);
    int syz_initialize();
    int syz_initializeWithConfig(ref LibraryConfig config);
    int syz_shutdown();
    int syz_getLastErrorCode();
    string syz_getLastErrorMessage();
    int syz_handleIncRef(int handle);
    int syz_handleDecRef(int handle);
    void syz_initDeleteBehaviorConfig(out DeleteBehaviorConfig config);
    int syz_configDeleteBehavior(int obj, ref DeleteBehaviorConfig cfg);
    int syz_handleGetObjectType(out ObjectType type, int handle);
    int syz_handleGetUserdata(out Span<byte> data, int handle);
    int syz_handleSetUserdata(int handle, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_pause(int handle);
    int syz_play(int handle);
    int syz_getI(out int value, int handle, Property property);
    int syz_setI(int target, Property property, int value);
    int syz_getD(out double value, int handle, Property property);
    int syz_setD(int target, Property property, double value);
    int syz_setO(int target, Property property, int value);
    int syz_getD3(out double x, out double y, out double z, int handle, Property property);
    int syz_setD3(int handle, Property property, double x, double y, double z);
    int syz_getD6(out double x, out double y, out double z, out double x2, out double y2, out double z2, int handle, Property property);
    int syz_setD6(int handle, Property property, double x, double y, double z, double x2, double y2, double z2);
    int syz_getBiquad(out BiquadConfig filter, int handle, Property property);
    int syz_setBiquad(int handle, Property property, ref BiquadConfig filter);
    int syz_biquadDesignIdentity(out BiquadConfig filter);
    int syz_biquadDesignLowpass(out BiquadConfig filter, double frequency, double q);
    int syz_biquadDesignHighpass(out BiquadConfig filter, double frequency, double q);
    int syz_biquadDesignBandpass(out BiquadConfig filter, double frequency, double bw);
    int syz_createContext(out int handle, Span<byte>? userdata, UserDataFreeCallback? UserdataFreeCallback);
    int syz_createContextHeadless(out int handle, Span<byte>? userdata, UserDataFreeCallback? UserdataFreeCallback);
    int syz_contextGetBlock(int handle, out Memory<float> block);
    int syz_contextEnableEvents(int handle);
    int syz_contextGetNextEvent(out AutomationEvent evt, int context, ulong flags);
    int syz_registerStreamProtocol(string protocol, StreamOpenCallback callback, Span<byte>? userdata);
    int syz_createStreamHandleFromStreamParams(out int handle, string protocol, string path, Span<byte>? param, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_createStreamHandleFromMemory(out int handle, ulong len, Memory<byte> data, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_createStreamHandleFromFile(out int handle, string path, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_createStreamHandleFromCustomStream(out int handle, ref CustomStreamDefinition callbacks, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_createStreamingGeneratorFromStreamParams(out int handle, int context, string protocol, string path, Span<byte>? param, Span<byte>? config, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_createStreamingGeneratorFromFile(out int handle, int context, string path, Span<byte>? config, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_createStreamingGeneratorFromStreamHandle(out int handle, int context, int stream_handle, Span<byte>? config, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_createBufferFromStreamParams(out int handle, string protocol, string path, Span<byte>? param, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_createBufferFromEncodedData(out int handle, ulong len, Memory<byte> data, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_createBufferFromFloatArray(out int handle, uint sr, uint channels, ulong frames, Memory<float> data, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_createBufferFromFile(out int handle, string path, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_createBufferFromStreamHandle(out int handle, int stream, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_bufferGetChannels(out uint channels, int handle);
    int syz_bufferGetLengthInSamples(out uint samples, int handle);
    int syz_bufferGetLengthInSeconds(out double seconds, int handle);
    int syz_bufferGetSizeInBytes(out ulong size, int handle);
    int syz_createBufferGenerator(out int handle, int context, Span<byte>? config, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_sourceAddGenerator(int source, int generator);
    int syz_sourceRemoveGenerator(int source, int generator);
    int syz_createDirectSource(out int handle, int context, Span<byte>? config, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_createAngularPannedSource(out int handle, int context, PannerStrategy strategy, double azimuth, double elevation, Span<byte>? config, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_createScalarPannedSource(out int handle, int context, PannerStrategy strategy, double scalar, Span<byte>? config, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_createSource3D(out int handle, int context, PannerStrategy strategy, double x, double y, double z, Span<byte>? config, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_createNoiseGenerator(out int handle, int context, uint channels, Span<byte>? config, Span<byte>? userdata, UserDataFreeCallback? callback);
    void syz_initSineBankConfig(out SineBankConfig cfg);
    int syz_createFastSineBankGenerator(out int handle, int context, ref SineBankConfig cfg, Span<byte>? config, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_createFastSineBankGeneratorSine(out int handle, int context, double initial_frequency, Span<byte>? config, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_createFastSineBankGeneratorTriangle(out int handle, int context, double initial_frequency, uint partials, Span<byte>? config, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_createFastSineBankGeneratorSquare(out int handle, int context, double initial_frequency, uint partials, Span<byte>? config, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_createFastSineBankGeneratorSaw(out int handle, int context, double initial_frequency, uint partials, Span<byte>? config, Span<byte>? userdata, UserDataFreeCallback? callback);
    void syz_initRouteConfig(out RouteConfig cfg);
    int syz_routingConfigRoute(int context, int output, int input, ref RouteConfig config);
    int syz_routingRemoveRoute(int context, int output, int input, double fade_out);
    int syz_routingRemoveAllRoutes(int context, int output, double fade_out);
    int syz_effectReset(int handle);
    int syz_createGlobalEcho(out int handle, int context, Span<byte>? config, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_globalEchoSetTaps(int handle, uint len, Memory<EchoTapConfig> taps);
    int syz_createGlobalFdnReverb(out int handle, int context, Span<byte>? config, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_createAutomationBatch(out int handle, int context, Span<byte>? userdata, UserDataFreeCallback? callback);
    int syz_automationBatchAddCommands(int batch, ulong commands_count, Memory<AutomationCommandData> commands);
    int syz_automationBatchExecute(int batch);
}

