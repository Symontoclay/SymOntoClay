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
            _logger.Info("9FEC1CBD-A953-4BB6-B23B-F0DF41B1962E", "Begin");

            var platformListener = new TstPlatformHostListener();

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.HostListener = platformListener;

            CreateMainNPC(factorySettings);

            _world.Start();

            Thread.Sleep(100);

            _logger.Info("4192C7E4-CD73-4005-814C-2DA5AB6A3A7B", "|||||||||||||");

            _npc.Logger.Output("FDC9FAC4-4442-453F-ADF9-0DD7558339F3", "|||||||||||||");


            Thread.Sleep(100);

            var factStr = "{: $x = {: act(M16, shoot) :} & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}";
            _npc.EngineContext.Storage.InsertListenedFact(_logger, factStr);













            Thread.Sleep(50000);

            _logger.Info("0FEF686A-A891-4BE5-93BF-59F00BFE8E0E", "End");
        }

        private IAddFactOrRuleResult LogicalStorage_OnAddingFact(RuleInstance ruleInstance)
        {
            _logger.Info("F13D2C8D-DF73-42B2-8A88-1CA39E33CEAE", $"ruleInstance = {ruleInstance.ToHumanizedString()}");

            return new AddFactOrRuleResult() { 
                KindOfResult = KindOfAddFactOrRuleResult.Accept,
                MutablePart = new MutablePartOfRuleInstance() { ObligationModality = LogicalValue.TrueValue }
            };
        }
    }
}
