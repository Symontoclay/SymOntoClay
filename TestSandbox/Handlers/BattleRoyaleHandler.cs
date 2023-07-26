/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

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

            var platformListener = new BattleRoyaleSilentHostListener();
            //var platformListener = new TstBattleRoyaleHostListener();
            //var platformListener = new TstBattleRoyaleHostListener2();
            //var platformListener = new BattleRoyaleHostListener();
            //var platformListener = new VeryLongMehod_HostListener();
            //var platformListener = new VeryShortMehod_HostListener();

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.HostListener = platformListener;

            CreateMainNPC(factorySettings);

            var standardFactsBuilder = new StandardFactsBuilder();

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

            var wasHide = false;

            platformListener.AddOnEndPointEnterSyncHandler("Go", () =>
            {
                _logger.Log($"OnEndPointEnterSyncHandler: On Enter Go wasHide = {wasHide}");

                if(!wasHide)
                {
                    //_npc.InsertFact(standardFactsBuilder.BuildAliveFactString(enemyId));
                    //_npc.InsertFact(standardFactsBuilder.BuildDefaultInheritanceFactString(enemyId, "soldier"));
                    //_npc.InsertFact(standardFactsBuilder.BuildSeeFactString(enemyId));
                }
            });

            _world.Start();

            Thread.Sleep(1000);

            _npc.InsertFact(standardFactsBuilder.BuildAliveFactString(enemyId));
            _npc.InsertFact("{: is(#enemy1,soldier,1) :}");
            _npc.InsertFact(standardFactsBuilder.BuildDefaultInheritanceFactString(enemyId, "soldier"));
            var seeFactId = _npc.InsertFact(standardFactsBuilder.BuildSeeFactString(enemyId));

            Thread.Sleep(5000);
            _logger.Log("||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");

            _npc.RemoveFact(seeFactId);

            wasHide = true;

            _logger.Log("__________________________________________________________");


            Thread.Sleep(100000);

            _logger.Log("End");
        }
    }
}
