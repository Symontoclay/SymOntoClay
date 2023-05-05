using NLog;
using SymOntoClay.BaseTestLib;
using SymOntoClay.BaseTestLib.HostListeners;
using SymOntoClay.DefaultCLIEnvironment;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Tests
{
    public class Actions_HardTests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

            instance.CreateWorld((n, message) =>
            {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "Begin");
                        break;

                    case 2:
                        Assert.AreEqual(message, "Begin go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "Begin move and check");
                        break;

                    case 4:
                        Assert.AreEqual(message, "I see!!!");
                        break;

                    case 5:
                        Assert.AreEqual(message, "End go");
                        break;

                    case 6:
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

        go();

        'End' >> @>log;
    }

    fun go()
    {
        'Begin go' >> @>log;
        `move and check`(#@(waypoint & random));
        'End go' >> @>log;
    }
}

action `move and check`
{
    op (@target)
    {
        'Begin move and check' >> @>log;

        repeat
        {
            @@host.go(to: @target)[: timeout = 1200, on completed {'on completed move' >> @>log; complete action;} :];
            check();
        }

        'End move and check' >> @>log;
    }

    on {: see(I, $_) :} => 
    {
        'I see!!!' >> @>log;
        weak cancel action;
    }
}

action check
{
    op ()
    {
        'Begin check' >> @>log;
        @@host.`stop`();
        @@host.`rotate`(30);
        @@host.`rotate`(-60);
        @@host.`rotate`(30);
        'End check' >> @>log;
    }
}");

            var hostListener = new BattleRoyaleSilentHostListener();

            var npc = instance.CreateNPC(hostListener);

            var settings = new PlaceSettings();
            settings.Id = "#WP1";
            settings.InstanceId = 123;
            settings.AllowPublicPosition = true;
            settings.UseStaticPosition = new System.Numerics.Vector3(5, 5, 5);

            settings.PlatformSupport = new PlatformSupportCLIStub();

            settings.Categories = new List<string>() { "waypoint" };
            settings.EnableCategories = true;

            var place = instance.GetPlace(settings);

            var enemySettings = new HumanoidNPCSettings();
            enemySettings.Id = "#enemy1";
            enemySettings.InstanceId = 567;
            enemySettings.AllowPublicPosition = true;
            enemySettings.UseStaticPosition = new System.Numerics.Vector3(15, 15, 15);

            enemySettings.PlatformSupport = new PlatformSupportCLIStub();
            enemySettings.HostListener = new object();

            enemySettings.Categories = new List<string>() { "soldier" };
            enemySettings.EnableCategories = true;

            var enemy = instance.GetHumanoidNPC(enemySettings);

            hostListener.AddOnEndPointEnterSyncHandler("Go", () => {
                npc.InsertFact("{: see(I, #enemy1) :}");
            });

            instance.StartWorld();

            Thread.Sleep(2000);
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            using var instance = new AdvancedBehaviorTestEngineInstance();

            instance.CreateWorld((n, message) =>
            {
                switch (n)
                {
                    case 1:
                        Assert.AreEqual(message, "Begin");
                        break;

                    case 2:
                        Assert.AreEqual(message, "Begin go");
                        break;

                    case 3:
                        Assert.AreEqual(message, "Begin move and check");
                        break;

                    case 4:
                        Assert.AreEqual(message, "I see!!!");
                        break;

                    case 5:
                        Assert.AreEqual(message, "End go");
                        break;

                    case 6:
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

        go();

        'End' >> @>log;
    }

    fun go()
    {
        'Begin go' >> @>log;
        `move and check`(#@(waypoint & random));
        'End go' >> @>log;
    }
}

action `move and check`
{
    op (@target)
    {
        'Begin move and check' >> @>log;

        repeat
        {
            @@host.go(to: @target)[: timeout = 1200, on completed {'on completed move' >> @>log; complete action;} :];
            check();
        }

        'End move and check' >> @>log;
    }

    on {: see(I, $_) :} => 
    {
        'I see!!!' >> @>log;
        complete action;
    }
}

action check
{
    op ()
    {
        'Begin check' >> @>log;
        @@host.`stop`();
        @@host.`rotate`(30);
        @@host.`rotate`(-60);
        @@host.`rotate`(30);
        'End check' >> @>log;
    }
}");

            var hostListener = new BattleRoyaleSilentHostListener();

            var npc = instance.CreateNPC(hostListener);

            var settings = new PlaceSettings();
            settings.Id = "#WP1";
            settings.InstanceId = 123;
            settings.AllowPublicPosition = true;
            settings.UseStaticPosition = new System.Numerics.Vector3(5, 5, 5);

            settings.PlatformSupport = new PlatformSupportCLIStub();

            settings.Categories = new List<string>() { "waypoint" };
            settings.EnableCategories = true;

            var place = instance.GetPlace(settings);

            var enemySettings = new HumanoidNPCSettings();
            enemySettings.Id = "#enemy1";
            enemySettings.InstanceId = 567;
            enemySettings.AllowPublicPosition = true;
            enemySettings.UseStaticPosition = new System.Numerics.Vector3(15, 15, 15);

            enemySettings.PlatformSupport = new PlatformSupportCLIStub();
            enemySettings.HostListener = new object();

            enemySettings.Categories = new List<string>() { "soldier" };
            enemySettings.EnableCategories = true;

            var enemy = instance.GetHumanoidNPC(enemySettings);

            hostListener.AddOnEndPointEnterSyncHandler("Go", () => {
                npc.InsertFact("{: see(I, #enemy1) :}");
            });

            instance.StartWorld();

            Thread.Sleep(2000);
        }
    }
}
