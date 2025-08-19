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

using Newtonsoft.Json;
using NUnit.Framework;
using SymOntoClay.BaseTestLib;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using SymOntoClay.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TestSandbox.Helpers;

namespace TestSandbox.Handlers
{
    public class StandardFactsBuilderGetTargetVarNameHandler
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImplementation();

        public void Run()
        {
            _logger.Info("EADBE5DB-6FA7-4A47-B193-C44750676A37", "Begin");

            //Case1();
            //Case2();
            //Case3();
            //Case4();
            //Case5();
            //Case6();
            //Case7();
            //Case8();
            //Case9();
            //Case10();
            //Case11();
            //Case12();
            Case13();

            _logger.Info("B17E693C-0230-4F96-8025-D5252E037025", "End");
        }

        private void Case13()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$x1) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";

            _logger.Info("A68AEEF6-30B7-4F12-BC29-D111ED3E9D34", $"factStr = '{factStr}'");

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.UseDefaultNLPSettings = false;
            factorySettings.UseDefaultAppFiles = false;
            factorySettings.ThreadingSettings = ConfigureThreadingSettings();
            var engineContext = TstEngineContextHelper.CreateAndInitContext(factorySettings).EngineContext;
            var fact = engineContext.Parser.ParseRuleInstance(factStr);

            var baseVarName = "$`x`";
            var varNamesList = RuleInstanceHelper.GetUniqueVarNamesWithPrefix(baseVarName, fact);

            _logger.Info("79AB8679-0ECB-44C4-8E00-908041ACF912", $"varNamesList = {varNamesList.WritePODListToString()}");
        }

        private void Case12()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$y) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";

            _logger.Info("B2F43A3E-3D23-4EB6-814D-64BC92963283", $"factStr = '{factStr}'");

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.UseDefaultNLPSettings = false;
            factorySettings.UseDefaultAppFiles = false;
            factorySettings.ThreadingSettings = ConfigureThreadingSettings();
            var engineContext = TstEngineContextHelper.CreateAndInitContext(factorySettings).EngineContext;
            var fact = engineContext.Parser.ParseRuleInstance(factStr);

            var baseVarName = "$`x`";
            var varNamesList = RuleInstanceHelper.GetUniqueVarNamesWithPrefix(baseVarName, fact);

            _logger.Info("A3283719-3629-407B-9E43-75E0A68C6E46", $"varNamesList = {varNamesList.WritePODListToString()}");
        }

        private void Case11()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$x) & (distance(i,$x,15.588457107543945) & direction($x,very far)) & point($x,#@[15.588457107543945, 12]) & say(I, $y) } :}";

            _logger.Info("42F7D22E-4596-4A9D-9147-867A930DAD3F", $"factStr = '{factStr}'");

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.UseDefaultNLPSettings = false;
            factorySettings.UseDefaultAppFiles = false;
            factorySettings.ThreadingSettings = ConfigureThreadingSettings();
            var engineContext = TstEngineContextHelper.CreateAndInitContext(factorySettings).EngineContext;

            var fact = engineContext.Parser.ParseRuleInstance(factStr);

            _logger.Info("28AE46A9-4233-4487-A52F-B1826E82C485", $"fact = '{fact.ToHumanizedString()}'");

            var baseVarName = "$x";

            _logger.Info("F561D943-AB58-4EDA-A9AB-0D02A489EFAA", $"baseVarName = '{baseVarName}'");

            var targetVarName = RuleInstanceHelper.GetNewUniqueVarNameWithPrefix(baseVarName, fact);

            _logger.Info("7AFF42D9-7438-4E26-9B89-6CBFA14C79D1", $"targetVarName = '{targetVarName}'");
        }

        private void Case10()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$x) & (distance(i,$x,15.588457107543945) & direction($x,very far)) & point($x,#@[15.588457107543945, 12]) & say(I, $y) } :}";

            _logger.Info("862ECA66-5BA7-4D48-A4CF-A9B1380C3B9C", $"factStr = '{factStr}'");

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.UseDefaultNLPSettings = false;
            factorySettings.UseDefaultAppFiles = false;
            factorySettings.ThreadingSettings = ConfigureThreadingSettings();
            var engineContext = TstEngineContextHelper.CreateAndInitContext(factorySettings).EngineContext;

            var fact = engineContext.Parser.ParseRuleInstance(factStr);

            _logger.Info("0A864CCD-D2BE-4D75-9B6C-341047037AF0", $"fact = '{fact.ToHumanizedString()}'");

            var baseVarName = "$x";

            _logger.Info("6B55351F-CD67-4AA8-A84F-5FE2C323810D", $"baseVarName = '{baseVarName}'");

            var varNamesList = RuleInstanceHelper.GetUniqueVarNamesWithPrefix(baseVarName, fact);

            _logger.Info("30B65F22-9B7D-4D7D-BC8C-9ED50CA3FB69", $"varNamesList = {JsonConvert.SerializeObject(varNamesList, Formatting.Indented)}");
        }

        private void Case9()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$x) & (distance(i,$x,15.588457107543945) & direction($x,very far)) & point($x,#@[15.588457107543945, 12]) & say(I, $y) } :}";

            _logger.Info("C087C3EF-7DE5-475E-BEB4-409A56A965BC", $"factStr = '{factStr}'");

            var factorySettings = new UnityTestEngineContextFactorySettings();
            factorySettings.UseDefaultNLPSettings = false;
            factorySettings.UseDefaultAppFiles = false;
            factorySettings.ThreadingSettings = ConfigureThreadingSettings();
            var engineContext = TstEngineContextHelper.CreateAndInitContext(factorySettings).EngineContext;

            var fact = engineContext.Parser.ParseRuleInstance(factStr);

            _logger.Info("2536EE63-2111-488B-A6E3-3BB787BB430B", $"fact = '{fact.ToHumanizedString()}'");

            var varNamesList = RuleInstanceHelper.GetUniqueVarNames(fact);

            _logger.Info("787A3A02-21AF-4543-A3CF-5AC8E67D7B21", $"varNamesList = {JsonConvert.SerializeObject(varNamesList, Formatting.Indented)}");
        }

        private void Case8()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$x1) & distance(i,$x2,15.588457107543945) & direction($x,12) & point($x,#@[15.588457107543945, 12]) & say(I, $y) } :}";

            _logger.Info("5E500B79-D0B5-428E-B060-F683D8F92559", $"factStr = '{factStr}'");

            var baseVarName = "$x";

            _logger.Info("1CDDE956-B504-4987-8A44-2FBE93EB67AA", $"baseVarName = '{baseVarName}'");

            var targetVarName = RuleInstanceHelper.GetNewUniqueVarNameWithPrefix(baseVarName, factStr);

            _logger.Info("E802D354-91A3-43C3-A2C5-7A9F7FEF2E30", $"targetVarName = '{targetVarName}'");
        }

        private void Case7()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$x1) & distance(i,$x2,15.588457107543945) & direction($x,12) & point($x,#@[15.588457107543945, 12]) & say(I, $y) } :}";

            _logger.Info("BCED5DA5-1374-4EB8-BFBA-F90E0EFC9994", $"factStr = '{factStr}'");

            var baseVarName = "$x";

            _logger.Info("A71BF1FE-E172-4CA9-A5DA-83D01FFDDB12", $"baseVarName = '{baseVarName}'");

            var varNamesList = RuleInstanceHelper.GetUniqueVarNamesWithPrefix(baseVarName, factStr);

            _logger.Info("1FE3AE9F-0689-4327-83B7-D59E6B98629D", $"varNamesList = {JsonConvert.SerializeObject(varNamesList, Formatting.Indented)}");

            varNamesList = varNamesList.Select(p => p.Substring(2)).Where(p => string.IsNullOrWhiteSpace(p) || p.Any(x => char.IsDigit(x))).ToList();

            _logger.Info("BDDDA2CE-A26C-459C-B15C-7C96E7B70B97", $"varNamesList (after) = {JsonConvert.SerializeObject(varNamesList, Formatting.Indented)}");

            var numberIndexes = varNamesList.Select(p => string.IsNullOrWhiteSpace(p) ? 0 : int.Parse(p)).ToList();

            _logger.Info("87A5E002-0D5A-41E3-B99B-BCC14E8C3D62", $"numberIndexes = {JsonConvert.SerializeObject(numberIndexes, Formatting.Indented)}");

            var targetIndex = varNamesList.Select(p => string.IsNullOrWhiteSpace(p) ? 0 : int.Parse(p)).Max() + 1;

            _logger.Info("3B800353-6F5E-4C71-A3F9-3CE0856F107E", $"targetIndex = {targetIndex}");

            var targetVarName = $"{baseVarName}{targetIndex}";

            _logger.Info("B836D6BB-BA0E-47EC-B6CE-6C0DBBD471CF", $"targetVarName = '{targetVarName}'");
        }

        private void Case6()
        {
            var factStr = "{: >: { $y = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$y) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";

            _logger.Info("058C0237-24A9-4ABC-A27F-D8C071B79998", $"factStr = '{factStr}'");

            var baseVarName = "$x";

            _logger.Info("0F896288-4B5B-4E9F-9485-CB3D41F914A4", $"baseVarName = '{baseVarName}'");

            var varNamesList = RuleInstanceHelper.GetUniqueVarNamesWithPrefix(baseVarName, factStr);

            _logger.Info("6DCD5702-BA22-4EA7-AD38-1EBA6A01FEF7", $"varNamesList = {JsonConvert.SerializeObject(varNamesList, Formatting.Indented)}");
        }

        private void Case5()
        {
            var factStr = "{: >: { $x1 = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i, $x1) & distance(i,$x,15.588457107543945) & direction($x,12) & point($x,#@[15.588457107543945, 12]) & say(I, $y) } :}";

            _logger.Info("D39FA621-13AC-4780-BFCB-3800E9CE4EAF", $"factStr = '{factStr}'");

            var baseVarName = "$x";

            _logger.Info("506C4AA0-BE7E-4576-9B96-B8CB9F464D2C", $"baseVarName = '{baseVarName}'");

            var pattern = $@"(\A|\b|\s|\W)(?<var>\{baseVarName}\d*)(\b|\Z|\s)";

            _logger.Info("8461C8AD-8D4D-46AD-BC39-52BD8EC20738", $"pattern = '{pattern}'");

            var regex = new Regex(pattern);

            MatchCollection matchesList = regex.Matches(factStr);

            foreach (Match match in matchesList)
            {
                _logger.Info("0A4B171E-DAD7-422B-8448-3CEE9DD9C329", $"match = {match}");

                if (!match.Success)
                {
                    break;
                }

                GroupCollection groups = match.Groups;

                foreach (var group in groups)
                {
                    _logger.Info("28352A95-6CDE-4EE6-95C6-44C1FFC52C3C", $"group = {group}");
                }

                var gItem = groups["var"];

                _logger.Info("7FA3F7B7-135C-4B3F-8F50-E149A0CEE73A", $"gItem.Value = {gItem.Value}");
                _logger.Info("081773DD-7C82-47DF-A69F-B020BB34AB0C", $"gItem.Index = {gItem.Index}");
            }
        }

        private void Case4()
        {
            var factStr = "{: >: { $y = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$y) & distance(i,$y,15.588457107543945) & direction($y,12) & point($y,#@[15.588457107543945, 12]) & say(I, $y) } :}";

            _logger.Info("300B3B75-86FA-4D5D-A177-E5758FA9D38C", $"factStr = '{factStr}'");

            var varNamesList = RuleInstanceHelper.GetUniqueVarNames(factStr);

            _logger.Info("3F43E08F-CBC5-4F20-BF29-759F9A89A2B4", $"varNamesList = {JsonConvert.SerializeObject(varNamesList, Formatting.Indented)}");
        }

        private void Case3()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$x) & distance(i,$x,15.588457107543945) & direction($x,12) & point($x,#@[15.588457107543945, 12]) & say(I, $y) } :}";

            _logger.Info("78A66951-7E8C-4E09-8363-E876413A0602", $"factStr = '{factStr}'");

            var regex = new Regex(_logicalVarPattern);

            MatchCollection matchesList = regex.Matches(factStr);

            foreach (Match match in matchesList)
            {
                _logger.Info("DF1CC076-8706-4F63-8AA1-C42C362DEA34", $"match = {match}");

                if (!match.Success)
                {
                    break;
                }

                GroupCollection groups = match.Groups;

                foreach (var group in groups)
                {
                    _logger.Info("", $"group = {group}");
                }

                var gItem = groups["var"];

                _logger.Info("AF7B520A-CC94-403A-BC12-8C0E56D49438", $"gItem.Value = {gItem.Value}");
                _logger.Info("463D5972-F57E-4069-9E14-C87E373EBED9", $"gItem.Index = {gItem.Index}");
            }
        }

        private void Case2()
        {
            var factStr = "{: >: { $x = {: #^`fdb93ce8-6392-4583-a400-565ade676acd` >: { act(m16,shoot) } :} & hear(i,$x) & distance(i,$x,15.588457107543945) & direction($x,12) & point($x,#@[15.588457107543945, 12]) & say(I, $y) } :}";

            _logger.Info("4A56ED1A-AD19-419A-908E-D18C1FBCE763", $"factStr = '{factStr}'");

            var regex = new Regex(_logicalVarPattern);

            var pos = 0;

            while (true)
            {
                _logger.Info("E155C929-FF80-48B7-B1D9-22F16BFADBB3", $"pos = {pos}");

                var match = regex.Match(factStr, pos);
                _logger.Info("9FA465FF-66D4-4B63-BB0B-179AEADCA744", $"match = {match}");

                if (!match.Success)
                {
                    break;
                }

                GroupCollection groups = match.Groups;

                foreach (var group in groups)
                {
                    _logger.Info("CE02D640-4A1B-4F82-880F-B80C7FA7B81B", $"group = {group}");
                }

                var gItem = groups["var"];

                _logger.Info("BC2A5AA5-7E51-4A43-9D2B-5F35F9200F51", $"gItem.Value = {gItem.Value}");
                _logger.Info("F245B1DF-5E17-4718-BF43-082A4AD4532F", $"gItem.Index = {gItem.Index}");

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
                _logger.Info("CB26FF56-D654-4DCE-B71C-4E662CCC8617", $"item = '{item}'");

                _logger.Info("74192E7E-FE0F-4748-9D85-ABA3028044DC", $"isVar = {regex.IsMatch(item)}");

                var match = regex.Match(item);
                _logger.Info("E0BC12E0-B157-4928-A93B-594D318F22B4", $"match = {match}");

                GroupCollection groups = match.Groups;

                foreach (var group in groups)
                {
                    _logger.Info("80716A44-FAFA-42BD-94F0-090E4E6E0F89", $"group = {group}");
                }

                var gItem = groups["var"];

                _logger.Info("CDADA05F-018D-44A7-9DBD-1B64E59140F3", $"gItem.Value = {gItem.Value}");
                _logger.Info("86BD2C11-0D27-4A6E-A269-CB7B08F928C8", $"gItem.Index = {gItem.Index}");
            }
        }

        private ThreadingSettings ConfigureThreadingSettings()
        {
            return new ThreadingSettings
            {
                AsyncEvents = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 5
                },
                CodeExecution = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 5
                }
            };
        }
    }
}
