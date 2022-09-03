namespace SynthizerSharp.Tests;

public class UnitTest1
{
    // OS platforms were found in NUnits source code: https://raw.githubusercontent.com/nunit/nunit/master/src/NUnitFramework/framework/Internal/PlatformHelper.cs
    [Platform("Linux,Unix")]
    [Test]
    public void TestUnixLibraryActivation()
    {
        Assert.DoesNotThrow(() =>
        {
            var options = ImplementationOptions.UseLazyBinding | ImplementationOptions.GenerateDisposalChecks | ImplementationOptions.UseIndirectCalls;
            var activator = new NativeLibraryBuilder(options);
            var library = activator.ActivateInterface<IRawSynthizer>("libsynthizer.so");
        });
        Assert.Pass();
    }

    [Platform("MacOSX")]
    [Test]
    public void TestAppleLibraryActivation()
    {
        Assert.DoesNotThrow(() =>
        {
            var options = ImplementationOptions.UseLazyBinding | ImplementationOptions.GenerateDisposalChecks | ImplementationOptions.UseIndirectCalls;
            var activator = new NativeLibraryBuilder(options);
            var library = activator.ActivateInterface<IRawSynthizer>("synthizer.dylib");
        });
        Assert.Pass();
    }

    [Platform("Win7,Windows7,Win8,Windows8,Win8.1,Windows8.1,Windows10,Win10,Xbox")]
    [Test]
    public void TestWin32LibraryActivation()
    {
        Assert.DoesNotThrow(() =>
        {
            var options = ImplementationOptions.UseLazyBinding | ImplementationOptions.GenerateDisposalChecks | ImplementationOptions.UseIndirectCalls;
            var activator = new NativeLibraryBuilder(options);
            var library = activator.ActivateInterface<IRawSynthizer>("synthizer.dll");
        });
        Assert.Pass();
    }

    [Test]
    public void TestDoubleRefcount()
    {
        var library = FFIActivator.ActivateFFIInterface();
        int h;
        Assert.That(library.syz_initialize(), Is.EqualTo(0));
        Assert.That(library.syz_createContext(out h, null, null), Is.EqualTo(0));
        Assert.That(library.syz_handleIncRef(ref h), Is.EqualTo(0));
        Assert.That(library.syz_handleDecRef(ref h), Is.EqualTo(0));
        Assert.That(library.syz_handleDecRef(ref h), Is.EqualTo(0));
        Assert.That(library.syz_shutdown(), Is.EqualTo(0));
        Assert.Pass();
    }

    [Test]
    public void TestEffectConnections()
    {
        var library = FFIActivator.ActivateFFIInterface();
        int ctx;
        var sources = new int[3];
        var reverbs = new int[2];
        RouteConfig cfg = new();
        cfg.Gain = 1.0;
        cfg.FadeTime = 0.1;
        Assert.That(library.syz_initialize(), Is.EqualTo(0));
        Assert.That(library.syz_createContext(out ctx, null, null), Is.EqualTo(0));
        for (int i = 0; i < sources.Length; ++i)
        {
            Assert.That(library.syz_createSource3D(out sources[i], ctx, PannerStrategy.Delegate, 0.0, 0.0, 0.0, null, null, null), Is.EqualTo(0));
        }
        for (int i = 0; i < reverbs.Length; ++i)
        {
            Assert.That(library.syz_createGlobalFdnReverb(out reverbs[i], ctx, null, null, null), Is.EqualTo(0));
        }
        foreach (var source in sources)
        {
            foreach (var reverb in reverbs)
            {
                Assert.That(library.syz_routingConfigRoute(ctx, source, reverb, ref cfg), Is.EqualTo(0));
            }
        }
        foreach (var source in sources)
        {
            foreach (var reverb in reverbs)
            {
                Assert.That(library.syz_routingRemoveRoute(ctx, source, reverb, 0.01), Is.EqualTo(0));
            }
        }
        foreach (var source in sources)
        {
            foreach (var reverb in reverbs)
            {
                Assert.That(library.syz_routingConfigRoute(ctx, source, reverb, ref cfg), Is.EqualTo(0));
                Assert.That(library.syz_routingRemoveRoute(ctx, source, reverb, 0.05), Is.EqualTo(0));
            }
        }
        for (int i = 0; i < sources.Length; ++i)
        {
            Assert.That(library.syz_handleDecRef(ref sources[i]), Is.EqualTo(0));
        }
        for (int i = 0; i < reverbs.Length; ++i)
        {
            Assert.That(library.syz_handleDecRef(ref reverbs[i]), Is.EqualTo(0));
        }
        Assert.That(library.syz_shutdown(), Is.EqualTo(0));
        Assert.Pass();
    }

    // Significantly extended (in quantity) version of the above test to stress-test effect connections
    [Test]
    public void TestEffectConnectionsExtended()
    {
        var library = FFIActivator.ActivateFFIInterface();
        int ctx;
        var sources = new int[250000000];
        var reverbs = new int[250000000];
        RouteConfig cfg = new();
        cfg.Gain = 1.0;
        cfg.FadeTime = 0.1;
        Assert.That(library.syz_initialize(), Is.EqualTo(0));
        Assert.That(library.syz_createContext(out ctx, null, null), Is.EqualTo(0));
        for (int i = 0; i < sources.Length; ++i)
        {
            Assert.That(library.syz_createSource3D(out sources[i], ctx, PannerStrategy.Delegate, 0.0, 0.0, 0.0, null, null, null), Is.EqualTo(0));
        }
        for (int i = 0; i < reverbs.Length; ++i)
        {
            Assert.That(library.syz_createGlobalFdnReverb(out reverbs[i], ctx, null, null, null), Is.EqualTo(0));
        }
        foreach (var source in sources)
        {
            foreach (var reverb in reverbs)
            {
                Assert.That(library.syz_routingConfigRoute(ctx, source, reverb, ref cfg), Is.EqualTo(0));
            }
        }
        foreach (var source in sources)
        {
            foreach (var reverb in reverbs)
            {
                Assert.That(library.syz_routingRemoveRoute(ctx, source, reverb, 0.01), Is.EqualTo(0));
            }
        }
        foreach (var source in sources)
        {
            foreach (var reverb in reverbs)
            {
                Assert.That(library.syz_routingConfigRoute(ctx, source, reverb, ref cfg), Is.EqualTo(0));
                Assert.That(library.syz_routingRemoveRoute(ctx, source, reverb, 0.05), Is.EqualTo(0));
            }
        }
        for (int i = 0; i < sources.Length; ++i)
        {
            Assert.That(library.syz_handleDecRef(ref sources[i]), Is.EqualTo(0));
        }
        for (int i = 0; i < reverbs.Length; ++i)
        {
            Assert.That(library.syz_handleDecRef(ref reverbs[i]), Is.EqualTo(0));
        }
        Assert.That(library.syz_shutdown(), Is.EqualTo(0));
        Assert.Pass();
    }
}
