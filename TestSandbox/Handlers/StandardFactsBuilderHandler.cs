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

using Newtonsoft.Json;
using SymOntoClay.BaseTestLib;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.StandardFacts;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestSandbox.Helpers;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class StandardFactsBuilderHandler
    {
        public StandardFactsBuilderHandler()
        {
            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.UseDefaultNLPSettings = false;
            factorySettings.UseDefaultAppFiles = false;
            _engineContext = TstEngineContextHelper.CreateAndInitContext(factorySettings).EngineContext;

            _standardFactsBuilder = new StandardFactsBuilder();
        }

        private readonly IEngineContext _engineContext;
        private static readonly IEntityLogger _logger = new LoggerImpementation();
        private readonly IStandardFactsBuilder _standardFactsBuilder;

        public void Run()
        {
            _logger.Log("Begin");

            Case26();
            Case25();

            _logger.Log("End");
        }

        private void Case26()
        {
            var fact = _standardFactsBuilder.BuildReadyForShootFactInstance("#123");

            _logger.Log($"fact = '{fact.ToHumanizedString()}'");
            _logger.Log($"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case25()
        {
            var factStr = _standardFactsBuilder.BuildReadyForShootFactString("#123");

            _logger.Log($"factStr = '{factStr}'");
        }

        private void Case24()
        {
            var fact = _standardFactsBuilder.BuildShootSoundFactInstance();

            _logger.Log($"fact = '{fact.ToHumanizedString()}'");
            _logger.Log($"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case23()
        {
            var factStr = _standardFactsBuilder.BuildShootSoundFactString();

            _logger.Log($"factStr = '{factStr}'");
        }

        private void Case22()
        {
            var fact = _standardFactsBuilder.BuildShootFactInstance("#123");

            _logger.Log($"fact = '{fact.ToHumanizedString()}'");
            _logger.Log($"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case21()
        {
            var factStr = _standardFactsBuilder.BuildShootFactString("#123");

            _logger.Log($"factStr = '{factStr}'");
        }

        private void Case20()
        {
            var fact = _standardFactsBuilder.BuildHoldFactInstance("#123", "#456");

            _logger.Log($"fact = '{fact.ToHumanizedString()}'");
            _logger.Log($"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case19()
        {
            var factStr = _standardFactsBuilder.BuildHoldFactString("#123", "#456");

            _logger.Log($"factStr = '{factStr}'");
        }

        private void Case18()
        {
            var fact = _standardFactsBuilder.BuildRunSoundFactInstance();

            _logger.Log($"fact = '{fact.ToHumanizedString()}'");
            _logger.Log($"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case17()
        {
            var factStr = _standardFactsBuilder.BuildRunSoundFactString();

            _logger.Log($"factStr = '{factStr}'");
        }

        private void Case16()
        {
            var fact = _standardFactsBuilder.BuildRunFactInstance("#123");

            _logger.Log($"fact = '{fact.ToHumanizedString()}'");
            _logger.Log($"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case15()
        {
            var factStr = _standardFactsBuilder.BuildRunFactString("#123");

            _logger.Log($"factStr = '{factStr}'");
        }

        private void Case14()
        {
            var fact = _standardFactsBuilder.BuildWalkSoundFactInstance();

            _logger.Log($"fact = '{fact.ToHumanizedString()}'");
            _logger.Log($"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case13()
        {
            var factStr = _standardFactsBuilder.BuildWalkSoundFactString();

            _logger.Log($"factStr = '{factStr}'");
        }

        private void Case12()
        {
            var fact = _standardFactsBuilder.BuildWalkFactInstance("#123");

            _logger.Log($"fact = '{fact.ToHumanizedString()}'");
            _logger.Log($"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case11()
        {
            var factStr = _standardFactsBuilder.BuildWalkFactString("#123");

            _logger.Log($"factStr = '{factStr}'");
        }

        private void Case10()
        {
            var fact = _standardFactsBuilder.BuildStopFactInstance("#123");

            _logger.Log($"fact = '{fact.ToHumanizedString()}'");
            _logger.Log($"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case9()
        {
            var factStr = _standardFactsBuilder.BuildStopFactString("#123");

            _logger.Log($"factStr = '{factStr}'");

            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            _logger.Log($"fact = '{fact.ToHumanizedString()}'");
            _logger.Log($"fact = {fact}");
        }

        private void Case8()
        {
            var fact = _standardFactsBuilder.BuildDeadFactInstance("#123");

            _logger.Log($"fact = '{fact.ToHumanizedString()}'");
            _logger.Log($"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case7()
        {
            var factStr = _standardFactsBuilder.BuildDeadFactString("#123");

            _logger.Log($"factStr = '{factStr}'");

            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            _logger.Log($"fact = '{fact.ToHumanizedString()}'");
            _logger.Log($"fact = {fact}");
        }

        private void Case6()
        {
            var fact = _standardFactsBuilder.BuildAliveFactInstance("#123");

            _logger.Log($"fact = '{fact.ToHumanizedString()}'");
            _logger.Log($"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case5()
        {
            var factStr = _standardFactsBuilder.BuildAliveFactString("#123");

            _logger.Log($"factStr = '{factStr}'");

            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            _logger.Log($"fact = '{fact.ToHumanizedString()}'");
            _logger.Log($"fact = {fact}");
        }

        private void Case4()
        {
            var initialFactStr = "{: act(M16, shoot) :}";

            _logger.Log($"initialFactStr = {initialFactStr}");

            var initialFact = _engineContext.Parser.ParseRuleInstance(initialFactStr);

            _logger.Log($"initialFact = '{initialFact.ToHumanizedString()}'");

            var fact = _standardFactsBuilder.BuildSoundFactInstance(15.588457107543945, 12, initialFact);

            _logger.Log($"fact = '{fact.ToHumanizedString()}'");
            _logger.Log($"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case3()
        {
            var factStr = _standardFactsBuilder.BuildSoundFactString(15.588457107543945, 12, "{: act(M16, shoot) :}");

            _logger.Log($"factStr = '{factStr}'");
        }

        private void Case2()
        {
            var initialFactStr = "{: act(M16, shoot) :}";

            _logger.Log($"initialFactStr = {initialFactStr}");

            var initialFact = _engineContext.Parser.ParseRuleInstance(initialFactStr);

            _logger.Log($"initialFact = '{initialFact.ToHumanizedString()}'");

            var fact = _standardFactsBuilder.BuildSayFactInstance("#123", initialFact);

            _logger.Log($"fact = '{fact.ToHumanizedString()}'");
            _logger.Log($"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case1()
        {
            var factStr = _standardFactsBuilder.BuildSayFactString("#123", "{: act(M16, shoot) :}");

            _logger.Log($"factStr = '{factStr}'");

            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            _logger.Log($"fact = '{fact.ToHumanizedString()}'");
            _logger.Log($"fact = {fact}");

            _logger.Log($"fact.Normalized = '{fact.Normalized.ToHumanizedString()}'");
        }
    }
}
