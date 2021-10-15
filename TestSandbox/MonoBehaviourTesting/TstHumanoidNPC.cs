/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using Newtonsoft.Json;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core;
using SymOntoClay.DefaultCLIEnvironment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TestSandbox.CoreHostListener;
using TestSandbox.Helpers;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.MonoBehaviourTesting
{
    public class TstHumanoidNPC: TstMonoBehaviour
    {
        private readonly IEntityLogger _logger = new LoggerImpementation();

        private IHumanoidNPC _npc;
        private string _id;
        private TstRayScaner _tstRayScaner;

        public override void Awake()
        {
            _logger.Log("Begin");

            _tstRayScaner = new TstRayScaner();

            var platformListener = new TstPlatformHostListener();

            _id = "#`Test 1`";

            var npcSettings = new HumanoidNPCSettings();
            npcSettings.Id = _id;
            npcSettings.InstanceId = 1;
            //npcSettings.HostFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Hosts\PeaceKeeper\PeaceKeeper.host");
            npcSettings.LogicFile = Path.Combine(Directory.GetCurrentDirectory(), @"Source\Npcs\PeaceKeeper\PeaceKeeper.sobj");
            npcSettings.HostListener = platformListener;
            npcSettings.VisionProvider = _tstRayScaner;
            npcSettings.PlatformSupport = new PlatformSupportCLIStub();

            _logger.Log($"npcSettings = {npcSettings}");

            _npc = WorldFactory.WorldInstance.GetHumanoidNPC(npcSettings);

            _logger.Log($"_npc == null = {_npc == null}");

            _tstRayScaner.SetNPC(_npc);

            _logger.Log("End");
        }

        public override void Start()
        {
            _logger.Log("Begin");

            _logger.Log("End");
        }

        private bool _isFactUpated;

        public override void Update()
        {
            //_logger.Log("Begin");

            //if(!_isFactUpated)
            //{
            //    _isFactUpated = true;

            //    var factStr = $"act({_id}, go)";

            //    _logger.Log($"factStr = {factStr}");

            //    var factId = _npc.InsertPublicFact(factStr);

            //    _logger.Log($"factId = {factId}");

            //    _npc.RemovePublicFact(factId);
            //}

            _tstRayScaner.Scan();

            //var tmpVisibleItems = _tstRayScaner.GetCurrentVisibleItems();

            //_logger.Log($"tmpVisibleItems = {JsonConvert.SerializeObject(tmpVisibleItems, Formatting.Indented)}");

            //_logger.Log("End");
        }

        public override void Stop()
        {
            _logger.Log("Begin");

            _npc.Dispose();

            _logger.Log("End");
        }
    }
}
