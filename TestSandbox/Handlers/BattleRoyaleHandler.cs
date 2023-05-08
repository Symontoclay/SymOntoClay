using SymOntoClay.BaseTestLib;
using SymOntoClay.BaseTestLib.HostListeners;
using SymOntoClay.DefaultCLIEnvironment;
using SymOntoClay.StandardFacts;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestSandbox.CoreHostListener;

namespace TestSandbox.Handlers
{
    public class BattleRoyaleHandler : BaseGeneralStartHandler
    {
        public void Run()
        {
            _logger.Log("Begin");

            //var platformListener = new BattleRoyaleSilentHostListener();
            var platformListener = new TstBattleRoyaleHostListener();
            //var platformListener = new TstBattleRoyaleHostListener2();
            //var platformListener = new BattleRoyaleHostListener();
            //var platformListener = new VeryLongMehod_HostListener();
            //var platformListener = new VeryShortMehod_HostListener();

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.HostListener = platformListener;

            CreateMainNPC(factorySettings);

            var placeSettings = new PlaceSettings();
            placeSettings.Id = "#WP1";
            placeSettings.InstanceId = 123;
            placeSettings.AllowPublicPosition = true;
            placeSettings.UseStaticPosition = new System.Numerics.Vector3(5, 5, 5);

            placeSettings.PlatformSupport = new PlatformSupportCLIStub();

            placeSettings.Categories = new List<string>() { "waypoint" };
            placeSettings.EnableCategories = true;

            var place = _world.GetPlace(placeSettings);

            var enemyId = "#enemy1";

            var enemySettings = new HumanoidNPCSettings();
            enemySettings.Id = enemyId;
            enemySettings.InstanceId = 567;
            enemySettings.AllowPublicPosition = true;
            enemySettings.UseStaticPosition = new System.Numerics.Vector3(15, 15, 15);

            enemySettings.PlatformSupport = new PlatformSupportCLIStub();
            enemySettings.HostListener = new object();

            enemySettings.Categories = new List<string>() { "soldier" };
            enemySettings.EnableCategories = true;

            var enemy = _world.GetHumanoidNPC(enemySettings);

            var standardFactsBuilder = new StandardFactsBuilder();

            platformListener.AddOnEndPointEnterSyncHandler("Go", () =>
            {
                _logger.Log("Enter to Go");

                var factStr = standardFactsBuilder.BuildSeeFact(enemyId);

                _logger.Log($"factStr = {factStr}");

                var factId = _npc.InsertFact(factStr/*"{: see(I, #enemy1) :}"*/);
            });

            _world.Start();

            //Thread.Sleep(5000);

            //var factId = _npc.InsertFact("{: see(I, #enemy1) :}");

            Thread.Sleep(100000);

            _logger.Log("End");
        }
    }
}
