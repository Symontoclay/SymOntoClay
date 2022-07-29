using Newtonsoft.Json;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class StandardFactsBuilderGetTargetVarNameHandler
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            Case8();
            //Case7();
            //Case6();
            //Case5();
            //Case4();
            //Case3();
            //Case2();
            //Case1();

            _logger.Log("End");
        }

        private void Case8()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$x1) & distance(i,$x2,15.588457107543945) & direction($x,12) & point($x,#@[15.588457107543945, 12]) & say(I, $y) } :}";

            _logger.Log($"factStr = '{factStr}'");

            var baseVarName = "$x";

            _logger.Log($"baseVarName = '{baseVarName}'");

            var targetVarName = RuleInstanceHelper.GetNewUniqueVarNameWithPrefixFromFactString(baseVarName, factStr);

            _logger.Log($"targetVarName = '{targetVarName}'");
        }

        private void Case7()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$x1) & distance(i,$x2,15.588457107543945) & direction($x,12) & point($x,#@[15.588457107543945, 12]) & say(I, $y) } :}";

            _logger.Log($"factStr = '{factStr}'");

            var baseVarName = "$x";

            _logger.Log($"baseVarName = '{baseVarName}'");

            var varNamesList = RuleInstanceHelper.GetUniqueVarNamesWithPrefixFromFactString(baseVarName, factStr);

            _logger.Log($"varNamesList = {JsonConvert.SerializeObject(varNamesList, Formatting.Indented)}");

            varNamesList = varNamesList.Select(p => p.Substring(2)).Where(p => string.IsNullOrWhiteSpace(p) || p.Any(x => char.IsDigit(x))).ToList();

            _logger.Log($"varNamesList (after) = {JsonConvert.SerializeObject(varNamesList, Formatting.Indented)}");

            var numberIndexes = varNamesList.Select(p => string.IsNullOrWhiteSpace(p) ? 0 : int.Parse(p)).ToList();

            _logger.Log($"numberIndexes = {JsonConvert.SerializeObject(numberIndexes, Formatting.Indented)}");

            var targetIndex = varNamesList.Select(p => string.IsNullOrWhiteSpace(p) ? 0 : int.Parse(p)).Max() + 1;

            _logger.Log($"targetIndex = {targetIndex}");

            var targetVarName = $"{baseVarName}{targetIndex}";

            _logger.Log($"targetVarName = '{targetVarName}'");
        }

        private void Case6()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$x1) & distance(i,$x2,15.588457107543945) & direction($x,12) & point($x,#@[15.588457107543945, 12]) & say(I, $y) } :}";

            _logger.Log($"factStr = '{factStr}'");

            var baseVarName = "$x";

            _logger.Log($"baseVarName = '{baseVarName}'");

            var varNamesList = RuleInstanceHelper.GetUniqueVarNamesWithPrefixFromFactString(baseVarName, factStr);

            _logger.Log($"varNamesList = {JsonConvert.SerializeObject(varNamesList, Formatting.Indented)}");
        }

        private void Case5()
        {
            var factStr = "{: >: { $x1 = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i, $x1) & distance(i,$x,15.588457107543945) & direction($x,12) & point($x,#@[15.588457107543945, 12]) & say(I, $y) } :}";

            _logger.Log($"factStr = '{factStr}'");

            var baseVarName = "$x";

            _logger.Log($"baseVarName = '{baseVarName}'");

            var pattern = $@"(\A|\b|\s|\W)(?<var>\{baseVarName}\d*)(\b|\Z|\s)";

            _logger.Log($"pattern = '{pattern}'");

            var regex = new Regex(pattern);

            MatchCollection matchesList = regex.Matches(factStr);

            foreach (Match match in matchesList)
            {
                _logger.Log($"match = {match}");

                if (!match.Success)
                {
                    break;
                }

                GroupCollection groups = match.Groups;

                foreach (var group in groups)
                {
                    _logger.Log($"group = {group}");
                }

                var gItem = groups["var"];

                _logger.Log($"gItem.Value = {gItem.Value}");
                _logger.Log($"gItem.Index = {gItem.Index}");
            }
        }

        private void Case4()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$x) & distance(i,$x,15.588457107543945) & direction($x,12) & point($x,#@[15.588457107543945, 12]) & say(I, $y) } :}";

            _logger.Log($"factStr = '{factStr}'");

            var varNamesList = RuleInstanceHelper.GetUniqueVarNamesFromFactString(factStr);

            _logger.Log($"varNamesList = {JsonConvert.SerializeObject(varNamesList, Formatting.Indented)}");
        }

        private void Case3()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$x) & distance(i,$x,15.588457107543945) & direction($x,12) & point($x,#@[15.588457107543945, 12]) & say(I, $y) } :}";

            _logger.Log($"factStr = '{factStr}'");

            var regex = new Regex(_logicalVarPattern);

            MatchCollection matchesList = regex.Matches(factStr);

            foreach (Match match in matchesList)
            {
                _logger.Log($"match = {match}");

                if (!match.Success)
                {
                    break;
                }

                GroupCollection groups = match.Groups;

                foreach (var group in groups)
                {
                    _logger.Log($"group = {group}");
                }

                var gItem = groups["var"];

                _logger.Log($"gItem.Value = {gItem.Value}");
                _logger.Log($"gItem.Index = {gItem.Index}");
            }
        }

        private void Case2()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$x) & distance(i,$x,15.588457107543945) & direction($x,12) & point($x,#@[15.588457107543945, 12]) & say(I, $y) } :}";

            _logger.Log($"factStr = '{factStr}'");

            var regex = new Regex(_logicalVarPattern);

            var pos = 0;

            while (true)
            {
                _logger.Log($"pos = {pos}");

                var match = regex.Match(factStr, pos);
                _logger.Log($"match = {match}");

                if (!match.Success)
                {
                    break;
                }

                GroupCollection groups = match.Groups;

                foreach (var group in groups)
                {
                    _logger.Log($"group = {group}");
                }

                var gItem = groups["var"];

                _logger.Log($"gItem.Value = {gItem.Value}");
                _logger.Log($"gItem.Index = {gItem.Index}");

                pos = gItem.Index + 1;
            }
        }

        private string _logicalVarPattern = @"(\A|\b|\s|\W)(?<var>\$(\w|_|\d)+)(\b|\Z|\s)";

        private void Case1()
        {
            var itemsList = new List<string>()
            {
                "m $x d",
                "m $x) d",
                "m $x1 m",
                "m $x_1 l",
                "m $`x` f",
                "m $`some {: weird var` g",
                "m $$x h"
            };

            var regex = new Regex(_logicalVarPattern);

            foreach (var item in itemsList)
            {
                _logger.Log($"item = '{item}'");

                _logger.Log($"isVar = {regex.IsMatch(item)}");

                var match = regex.Match(item);
                _logger.Log($"match = {match}");

                GroupCollection groups = match.Groups;

                foreach (var group in groups)
                {
                    _logger.Log($"group = {group}");
                }

                var gItem = groups["var"];

                _logger.Log($"gItem.Value = {gItem.Value}");
                _logger.Log($"gItem.Index = {gItem.Index}");
            }
        }
    }
}
