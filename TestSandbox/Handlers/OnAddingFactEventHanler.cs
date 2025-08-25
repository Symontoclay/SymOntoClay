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

using SymOntoClay.BaseTestLib;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Storage.LogicalStoraging;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using TestSandbox.Helpers;

namespace TestSandbox.Handlers
{
    public class OnAddingFactEventHandler
    {
        public OnAddingFactEventHandler()
        {
            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.UseDefaultAppFiles = false;

            _engineContext = TstEngineContextHelper.CreateAndInitContext(factorySettings).EngineContext;
        }

        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImplementation();
        private readonly IEngineContext _engineContext;

        //event Func<RuleInstance, IAddFactOrRuleResult> OnAddingFact;

        public void Run()
        {
            _logger.Info("77931337-9D84-4E40-83C7-6C3D2164EB2F", "Begin");

            Case1();

            _logger.Info("3FBA1ACC-7A21-4A8D-B914-BD4813E35057", "End");
        }

        private void Case1()
        {
            //OnAddingFact += Handler1;
            //OnAddingFact += Handler2;
            //OnAddingFact += Handler3;

            //var factStr = "{: >: { like(i,#@{: >: { possess(i,$_) & cat($_) } :}) } :}";
            //var ruleInstance = Parse(factStr);

            //var fuzzyLogicResolver = _engineContext.DataResolversFactory.GetFuzzyLogicResolver();

            //var localCodeExecutionContext = new LocalCodeExecutionContext();
            //localCodeExecutionContext.Storage = _engineContext.Storage.GlobalStorage;
            //localCodeExecutionContext.Holder = NameHelper.CreateName(_engineContext.Id);

            //var result = AddingFactHelper.CallEvent(_logger, OnAddingFact, ruleInstance, fuzzyLogicResolver, localCodeExecutionContext);

            //_logger.Info("2DA20890-14D0-4755-BB63-C23648366632", $"result = {result}");
        }

        private IAddFactOrRuleResult Handler1(RuleInstance ruleInstance)
        {
            _logger.Info("8B8A3C16-7587-4530-862C-A8A8C50F5017", $"ruleInstance = {ruleInstance.ToHumanizedString()}");

            return new AddFactOrRuleResult()
            {
                KindOfResult = KindOfAddFactOrRuleResult.Accept,
                ChangedRuleInstance = ruleInstance
            };
        }

        private IAddFactOrRuleResult Handler2(RuleInstance ruleInstance)
        {
            _logger.Info("FCA5D675-84B4-44D3-82F7-C0DE50E373E5", $"ruleInstance = {ruleInstance.ToHumanizedString()}");

            return new AddFactOrRuleResult()
            {
                KindOfResult = KindOfAddFactOrRuleResult.Accept,
                ChangedRuleInstance = ruleInstance
            };
        }

        private IAddFactOrRuleResult Handler3(RuleInstance ruleInstance)
        {
            _logger.Info("E475C481-E0D5-4862-B0A7-49F93DE8D58E", $"ruleInstance = {ruleInstance.ToHumanizedString()}");

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
