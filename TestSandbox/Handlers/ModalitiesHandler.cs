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
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            Case2();
            //Case1();

            _logger.Log("End");
        }

        private void Case2()
        {
            var factStr = "{: parent(#Piter, #Tom) o: very middle so: 0.5 :}";
            //var factStr = "{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } so: 0.5 o: 1  :}";
            //var factStr = "{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } o: 1 :}";

            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            _logger.Log($"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case1()
        {
            var factStr = "{: >: { direction($x1,#@{: >: { color($_,$x1) & place($_) & green($x1) } :}) & $x1 = go(someone,self) } :}";

            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            _logger.Log($"fact (before) = {DebugHelperForRuleInstance.ToString(fact)}");

            fact.ObligationModality = LogicalValue.TrueValue;
            fact.SelfObligationModality = LogicalValue.FalseValue;

            //_logger.Log($"fact = {fact}");

            _logger.Log($"fact (after) = {DebugHelperForRuleInstance.ToString(fact)}");
        }
    }
}
