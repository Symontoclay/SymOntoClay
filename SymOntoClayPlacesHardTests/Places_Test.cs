using SymOntoClay.BaseTestLib;
using SymOntoClay.BaseTestLib.HostListeners;
using SymOntoClay.DefaultCLIEnvironment;
using SymOntoClay.UnityAsset.Core;

namespace SymOntoClayPlacesHardTests
{
    public class Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            using (var instance = new AdvancedBehaviorTestEngineInstance())
            {
                instance.CreateWorld((n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual("GoToImpl Begin", message);
                            break;

                        case 3:
                            Assert.AreEqual("ByEntity", message);
                            break;

                        case 4:
                            Assert.AreEqual("123", message);
                            break;

                        case 5:
                            Assert.AreEqual("#wp1", message);
                            break;

                        case 6:
                            Assert.AreEqual("<10, 10, 10>", message);
                            break;

                        case 7:
                            Assert.AreEqual("GoToImpl End", message);
                            break;

                        case 8:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, true);

                instance.WriteFile(@"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.go(to: #@(waypoint));
        'End' >> @>log;
    }
}");

                var hostListener = new Exec_Tests_HostListener4();

                instance.CreateNPC(hostListener);

                var settings = new PlaceSettings();
                settings.Id = "#WP1";
                settings.InstanceId = 123;
                settings.AllowPublicPosition = true;
                settings.UseStaticPosition = new System.Numerics.Vector3(5, 5, 5);

                settings.PlatformSupport = new PlatformSupportCLIStub();

                settings.Categories = new List<string>() { "waypoint" };
                settings.EnableCategories = true;

                var place = instance.GetPlace(settings);

                instance.StartWorld();

                Thread.Sleep(5000);
            }
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            using (var instance = new AdvancedBehaviorTestEngineInstance())
            {
                instance.CreateWorld((n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual("GoToImpl Begin", message);
                            break;

                        case 3:
                            Assert.AreEqual("ByEntity", message);
                            break;

                        case 4:
                            Assert.AreEqual("123", message);
                            break;

                        case 5:
                            Assert.AreEqual("#wp1", message);
                            break;

                        case 6:
                            Assert.AreEqual("<10, 10, 10>", message);
                            break;

                        case 7:
                            Assert.AreEqual("GoToImpl End", message);
                            break;

                        case 8:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, true);

                instance.WriteFile(@"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.go(to: #@(waypoint & random));
        'End' >> @>log;
    }
}");

                var hostListener = new Exec_Tests_HostListener4();

                instance.CreateNPC(hostListener);

                var settings = new PlaceSettings();
                settings.Id = "#WP1";
                settings.InstanceId = 123;
                settings.AllowPublicPosition = true;
                settings.UseStaticPosition = new System.Numerics.Vector3(5, 5, 5);

                settings.PlatformSupport = new PlatformSupportCLIStub();

                settings.Categories = new List<string>() { "waypoint" };
                settings.EnableCategories = true;

                var place = instance.GetPlace(settings);

                instance.StartWorld();

                Thread.Sleep(5000);
            }
        }

        [Test]
        [Parallelizable]
        public void Case2_a()
        {
            using (var instance = new AdvancedBehaviorTestEngineInstance())
            {
                instance.CreateWorld((n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual("GoToImpl Begin", message);
                            break;

                        case 3:
                            Assert.AreEqual("ByEntity", message);
                            break;

                        case 4:
                            Assert.AreEqual("123", message);
                            break;

                        case 5:
                            Assert.AreEqual("#wp1", message);
                            break;

                        case 6:
                            Assert.AreEqual("<10, 10, 10>", message);
                            break;

                        case 7:
                            Assert.AreEqual("GoToImpl End", message);
                            break;

                        case 8:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }, true);

                instance.WriteFile(@"app PeaceKeeper
{
    on Enter =>
    {
        'Begin' >> @>log;
        @@host.go(to: #@(waypoint & nearest));
        'End' >> @>log;
    }
}");

                var hostListener = new Exec_Tests_HostListener4();

                instance.CreateNPC(hostListener);

                var settings = new PlaceSettings();
                settings.Id = "#WP1";
                settings.InstanceId = 123;
                settings.AllowPublicPosition = true;
                settings.UseStaticPosition = new System.Numerics.Vector3(5, 5, 5);

                settings.PlatformSupport = new PlatformSupportCLIStub();

                settings.Categories = new List<string>() { "waypoint" };
                settings.EnableCategories = true;

                var place = instance.GetPlace(settings);

                instance.StartWorld();

                Thread.Sleep(5000);
            }
        }
    }
}