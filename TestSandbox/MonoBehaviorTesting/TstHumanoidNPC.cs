/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.DefaultCLIEnvironment;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using SymOntoClay.UnityAsset.Core;
using System.IO;
using TestSandbox.CoreHostListener;

namespace TestSandbox.MonoBehaviorTesting
{
    public class TstHumanoidNPC: TstMonoBehaviour
    {
        private readonly IMonitorLogger _logger = new MonitorLoggerNLogImplementation();

        private IHumanoidNPC _npc;
        private string _id;
        private TstRayScaner _tstRayScaner;

        public override void Awake()
        {
            _logger.Info("40CE89C8-E4E3-4FA1-BA28-D64D6B416C46", "Begin");

            _tstRayScaner = new TstRayScaner();

            var platformListener = new TstPlatformHostListener();

            _id = "#`Test 1`";

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = _id;
            npcSettings.InstanceId = 1;
            npcSettings.LogicFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Npcs\PeaceKeeper\PeaceKeeper.sobj");
            npcSettings.HostListener = platformListener;
            npcSettings.VisionProvider = _tstRayScaner;
            npcSettings.PlatformSupport = new PlatformSupportCLIStub();

            _logger.Info("468926C0-E364-4D57-9BE0-FA2BBA7386D3", $"npcSettings = {npcSettings}");

            _npc = WorldFactory.WorldInstance.GetHumanoidNPC(npcSettings);

            _logger.Info("816078D8-87DA-4215-AC74-30D83391651E", $"_npc == null = {_npc == null}");

            _tstRayScaner.SetNPC(_npc);

            _logger.Info("0BF6B946-3309-489B-A903-063F474C4442", "End");
        }

        public override void Start()
        {
            _logger.Info("98F76603-5F34-4F5C-801F-94E79E1DDE10", "Begin");

            _logger.Info("28EE84EC-9F1B-42F7-9C23-694261067170", "End");
        }

        //private bool _isFactUpdated;

        public override void Update()
        {







            _tstRayScaner.Scan();



        }

        public override void Stop()
        {
            _logger.Info("17A2B831-C23F-481D-A226-4CB3858308B3", "Begin");

            _npc.Dispose();

            _logger.Info("F3062F4C-22A5-4228-AA0B-0652E1FC4EAF", "End");
        }
    }
}
