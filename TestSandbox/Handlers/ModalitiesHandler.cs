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
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSandbox.Helpers;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class ModalitiesHandler
    {
        public ModalitiesHandler()
        {
            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.UseDefaultNLPSettings = false;
            factorySettings.UseDefaultAppFiles = false;
            _engineContext = TstEngineContextHelper.CreateAndInitContext(factorySettings).EngineContext;
        }

        private readonly IEngineContext _engineContext;
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        public void Run()
        {
            _logger.Info("DD43DA27-6B8E-4E29-94BC-B8BACE5DFB8E", "Begin");

            Case2();

            _logger.Info("59622775-A4B6-4568-93DC-592C7D3FCF96", "End");
        }

        private void Case2()
        {
            var factStr = "{: parent(#Piter, #Tom) o: very middle so: 0.5 :}";

            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            _logger.Info("A0C7F1DC-341B-4560-87EF-C8A4A869AAD2", $"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case1()
        {
            var factStr = "{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } :}";

            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            _logger.Info("A9D29B32-DEBF-40C8-BD79-74CD20ADCD1C", $"fact (before) = {DebugHelperForRuleInstance.ToString(fact)}");

            fact.ObligationModality = LogicalValue.TrueValue;
            fact.SelfObligationModality = LogicalValue.FalseValue;


            _logger.Info("E48442B1-F36F-4EB3-8B79-1D1C65398ED3", $"fact (after) = {DebugHelperForRuleInstance.ToString(fact)}");
        }
    }
}
