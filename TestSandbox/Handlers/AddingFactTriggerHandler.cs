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
using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Storage.LogicalStoraging;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestSandbox.CoreHostListener;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class AddingFactTriggerHandler : BaseGeneralStartHandler
    {
        public AddingFactTriggerHandler()
            : base(new UnityTestEngineContextFactorySettings()
            {
                UseDefaultNLPSettings = false
            })
        { 
        }

        public void Run()
        {
            _logger.Log("Begin");

            var platformListener = new TstPlatformHostListener();

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.HostListener = platformListener;

            CreateMainNPC(factorySettings);

            _world.Start();

            Thread.Sleep(100);

            _logger.Log("|||||||||||||");

            _npc.Logger.LogChannel("|||||||||||||");


            Thread.Sleep(100);

            var factStr = "{: $x = {: act(M16, shoot) :} & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}";
            _npc.EngineContext.Storage.InsertListenedFact(factStr);













            Thread.Sleep(50000);

            _logger.Log("End");
        }

        private IAddFactOrRuleResult LogicalStorage_OnAddingFact(RuleInstance ruleInstance)
        {
            _logger.Log($"ruleInstance = {ruleInstance.ToHumanizedString()}");

            return new AddFactOrRuleResult() { 
                KindOfResult = KindOfAddFactOrRuleResult.Accept,
                MutablePart = new MutablePartOfRuleInstance() { ObligationModality = LogicalValue.TrueValue }
            };
        }
    }
}
