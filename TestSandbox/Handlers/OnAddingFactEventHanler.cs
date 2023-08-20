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
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Storage.LogicalStoraging;
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
    public class OnAddingFactEventHanler
    {
        public OnAddingFactEventHanler()
        {
            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.UseDefaultAppFiles = false;

            _engineContext = TstEngineContextHelper.CreateAndInitContext(factorySettings).EngineContext;
        }

        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();
        private readonly IEngineContext _engineContext;

        event Func<RuleInstance, IAddFactOrRuleResult> OnAddingFact;

        public void Run()
        {
            _logger.Info("Begin");

            Case1();

            _logger.Info("End");
        }

        private void Case1()
        {
            OnAddingFact += Handler1;
            OnAddingFact += Handler2;
            OnAddingFact += Handler3;

            var factStr = "{: >: { like(i,#@{: >: { possess(i,$_) & cat($_) } :}) } :}";
            var ruleInstance = Parse(factStr);

            var fuzzyLogicResolver = _engineContext.DataResolversFactory.GetFuzzyLogicResolver();

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            localCodeExecutionContext.Storage = _engineContext.Storage.GlobalStorage;
            localCodeExecutionContext.Holder = NameHelper.CreateName(_engineContext.Id);

            var result = AddingFactHelper.CallEvent(OnAddingFact, ruleInstance, fuzzyLogicResolver, localCodeExecutionContext, _logger);

            _logger.Info($"result = {result}");
        }

        private IAddFactOrRuleResult Handler1(RuleInstance ruleInstance)
        {
            _logger.Info($"ruleInstance = {ruleInstance.ToHumanizedString()}");

            return new AddFactOrRuleResult()
            {
                KindOfResult = KindOfAddFactOrRuleResult.Accept,
                MutablePart = new MutablePartOfRuleInstance() { ObligationModality = LogicalValue.TrueValue }
            };
        }

        private IAddFactOrRuleResult Handler2(RuleInstance ruleInstance)
        {
            _logger.Info($"ruleInstance = {ruleInstance.ToHumanizedString()}");

            return new AddFactOrRuleResult()
            {
                KindOfResult = KindOfAddFactOrRuleResult.Accept,
                MutablePart = new MutablePartOfRuleInstance() { ObligationModality = LogicalValue.FalseValue }
            };
        }

        private IAddFactOrRuleResult Handler3(RuleInstance ruleInstance)
        {
            _logger.Info($"ruleInstance = {ruleInstance.ToHumanizedString()}");

            return new AddFactOrRuleResult()
            {
                KindOfResult = KindOfAddFactOrRuleResult.Accept
            };
        }

        private RuleInstance Parse(string text)
        {
            return _engineContext.Parser.ParseRuleInstance(text);
        }
    }
}
