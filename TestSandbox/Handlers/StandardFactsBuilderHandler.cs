using Newtonsoft.Json;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.StandardFacts;
using SymOntoClay.UnityAsset.Core;
using SymOntoClayBaseTestLib.Helpers;
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

            Case2();
            //Case1();

            _logger.Log("End");
        }

        private void Case2()
        {
            var initialFactStr = "{: act(M16, shoot) :}";

            _logger.Log($"initialFactStr = {initialFactStr}");

            var initialFact = _engineContext.Parser.ParseRuleInstance(initialFactStr);

            _logger.Log($"initialFact = '{initialFact.ToHumanizedString()}'");
            //_logger.Log($"initialFact = {initialFact}");

            var fact = _standardFactsBuilder.BuildSayFactInstance("#123", initialFact);

            _logger.Log($"fact = '{fact.ToHumanizedString()}'");
            _logger.Log($"fact = {fact}");
        }

        private void Case1()
        {
            var factStr = _standardFactsBuilder.BuildSayFactString("#123", "{: act(M16, shoot) :}");

            _logger.Log($"factStr = {factStr}");

            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            _logger.Log($"fact = '{fact.ToHumanizedString()}'");
            _logger.Log($"fact = {fact}");

            _logger.Log($"fact.Normalized = '{fact.Normalized.ToHumanizedString()}'");
        }
    }
}
