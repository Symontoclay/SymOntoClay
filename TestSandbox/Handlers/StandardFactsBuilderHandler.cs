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
using SymOntoClay.BaseTestLib;
using SymOntoClay.Core;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using SymOntoClay.StandardFacts;
using SymOntoClay.Threading;
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
            factorySettings.ThreadingSettings = new ThreadingSettings()
            {
                CodeExecution = new CustomThreadPoolSettings(),
                AsyncEvents = new CustomThreadPoolSettings()
            };
            _engineContext = TstEngineContextHelper.CreateAndInitContext(factorySettings).EngineContext;

            _standardFactsBuilder = new StandardFactsBuilder();
        }

        private readonly IEngineContext _engineContext;
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();
        private readonly IStandardFactsBuilder _standardFactsBuilder;

        public void Run()
        {
            _logger.Info("8DED6357-C08E-4FF4-80ED-5AC352FA45C9", "Begin");

            Case31();
            //Case30();
            //Case29_1();
            //Case29();
            //Case28();
            //Case27();
            //Case26();
            //Case25();
            //Case24();
            //Case23();
            //Case22();
            //Case21();
            //Case20();
            //Case19();
            //Case18();
            //Case17();
            //Case16();
            //Case15();
            //Case14();
            //Case13();
            //Case12();
            //Case11();
            //Case10();
            //Case9();
            //Case8();
            //Case7();
            //Case6();
            //Case5();
            //Case4();
            //Case3();
            //Case2();
            //Case1();

            _logger.Info("624BEA0D-792E-4DE2-8128-AF4547977A7F", "End");
        }

        private void Case31()
        {
            var fact = _standardFactsBuilder.BuildImplicitPropertyQueryInstance(NameHelper.CreateName("someprop"), NameHelper.CreateName("#123"));

            _logger.Info("5D63A028-F53B-4575-ACFE-E5E5D1353DF3", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("87B6F264-F522-4248-8DED-FDEFB52D47CB", $"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case30()
        {
            var fact = _standardFactsBuilder.BuildPropertyVirtualRelationInstance(NameHelper.CreateName("someprop"), NameHelper.CreateName("#123"), new NumberValue(16));

            _logger.Info("5D63A028-F53B-4575-ACFE-E5E5D1353DF3", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("87B6F264-F522-4248-8DED-FDEFB52D47CB", $"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case29_1()
        {
            var fact = _standardFactsBuilder.BuildDistanceFactInstance("#123", 12.6);

            _logger.Info("5D63A028-F53B-4575-ACFE-E5E5D1353DF3", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("87B6F264-F522-4248-8DED-FDEFB52D47CB", $"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");

            var factStr = _standardFactsBuilder.BuildDistanceFactString("#123", 12.6);

            _logger.Info("2675EC5A-AB19-4543-9B11-740F6A28FF62", $"factStr = '{factStr}'");
        }

        private void Case29()
        {
            var fact = _standardFactsBuilder.BuildDistanceFactInstance("#123", 12);

            _logger.Info("5D63A028-F53B-4575-ACFE-E5E5D1353DF3", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("87B6F264-F522-4248-8DED-FDEFB52D47CB", $"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");

            var factStr = _standardFactsBuilder.BuildDistanceFactString("#123", 12);

            _logger.Info("2675EC5A-AB19-4543-9B11-740F6A28FF62", $"factStr = '{factStr}'");
        }

        private void Case28()
        {
            var fact = _standardFactsBuilder.BuildFocusFactInstance("#123");

            _logger.Info("A48106BE-546B-4967-92D8-0904A73BCDA4", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("23FB361E-6CD7-4718-9557-280F2103541E", $"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");

            var factStr = _standardFactsBuilder.BuildFocusFactString("#123");

            _logger.Info("194D579B-D32F-42AF-9274-4AFFAF8EDBED", $"factStr = '{factStr}'");
        }

        private void Case27()
        {
            var fact = _standardFactsBuilder.BuildSeeFactInstance("#123");

            _logger.Info("5BA2388B-AEB8-40A0-AB7C-4F503890521A", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("6DCED42E-6A24-4FF6-95B4-CD9B3AD5A10E", $"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");

            var factStr = _standardFactsBuilder.BuildSeeFactString("#123");

            _logger.Info("287CD4B5-6544-4B5A-86D8-29B1C5ED0CCB", $"factStr = '{factStr}'");
        }

        private void Case26()
        {
            var fact = _standardFactsBuilder.BuildReadyForShootFactInstance("#123");

            _logger.Info("3BCDD76B-5A58-4938-98CB-B6E41ABA7F73", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("EBCE2199-70E4-427C-97FD-38DF1DEA22F1", $"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case25()
        {
            var factStr = _standardFactsBuilder.BuildReadyForShootFactString("#123");

            _logger.Info("1F465C88-273C-4C2D-8216-08115AC74576", $"factStr = '{factStr}'");
        }

        private void Case24()
        {
            var fact = _standardFactsBuilder.BuildShootSoundFactInstance();

            _logger.Info("64A19F7E-CE46-4081-8DB8-D86F33796552", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("DDBCF241-5B01-4F54-9CDB-70353D213F78", $"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case23()
        {
            var factStr = _standardFactsBuilder.BuildShootSoundFactString();

            _logger.Info("1B1F6777-3804-4EA0-8642-4D5E9BF4E5CA", $"factStr = '{factStr}'");
        }

        private void Case22()
        {
            var fact = _standardFactsBuilder.BuildShootFactInstance("#123");

            _logger.Info("414E93EE-187E-48AB-934E-241A606346C5", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("2E7CD67A-2A3C-44CE-9977-3D7589F78A18", $"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case21()
        {
            var factStr = _standardFactsBuilder.BuildShootFactString("#123");

            _logger.Info("8DD20902-4164-4844-AAEF-82DB7A961018", $"factStr = '{factStr}'");
        }

        private void Case20()
        {
            var fact = _standardFactsBuilder.BuildHoldFactInstance("#123", "#456");

            _logger.Info("EDBE5273-EB43-4CF4-A128-E9BED99004C2", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("A88848B7-71DD-4330-9A3C-6DB407A9CCC2", $"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case19()
        {
            var factStr = _standardFactsBuilder.BuildHoldFactString("#123", "#456");

            _logger.Info("64D7F746-28F1-41FE-ADAE-BB79C48039DA", $"factStr = '{factStr}'");
        }

        private void Case18()
        {
            var fact = _standardFactsBuilder.BuildRunSoundFactInstance();

            _logger.Info("9C53DC90-3637-464F-8720-677B0AC625C9", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("EC53EE83-F10F-4E51-9759-D0746E1EC3CD", $"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case17()
        {
            var factStr = _standardFactsBuilder.BuildRunSoundFactString();

            _logger.Info("A64B75F9-6E53-4B58-AF85-06BFEC93E8B5", $"factStr = '{factStr}'");
        }

        private void Case16()
        {
            var fact = _standardFactsBuilder.BuildRunFactInstance("#123");

            _logger.Info("73F32B89-278A-4AA1-9432-66D23CDB5BC8", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("C784DC3A-F649-42F0-9601-7CFD52EF8E27", $"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case15()
        {
            var factStr = _standardFactsBuilder.BuildRunFactString("#123");

            _logger.Info("88732F6A-D76F-43D2-A266-290CED5CA14B", $"factStr = '{factStr}'");
        }

        private void Case14()
        {
            var fact = _standardFactsBuilder.BuildWalkSoundFactInstance();

            _logger.Info("A5EB56EF-F88B-45F4-99F5-8277158090B6", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("28065763-4FDF-44BB-8273-B2977E223EB2", $"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case13()
        {
            var factStr = _standardFactsBuilder.BuildWalkSoundFactString();

            _logger.Info("D9C9F8C4-CBFF-4067-A251-BF43AC4EB15C", $"factStr = '{factStr}'");
        }

        private void Case12()
        {
            var fact = _standardFactsBuilder.BuildWalkFactInstance("#123");

            _logger.Info("2BFC39D9-36B0-4147-9280-376067D98450", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("2B1EC2DB-5CA1-46FE-82B3-80145041F622", $"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case11()
        {
            var factStr = _standardFactsBuilder.BuildWalkFactString("#123");

            _logger.Info("9CDAF508-3FE8-4FF4-955A-6E1B11872752", $"factStr = '{factStr}'");
        }

        private void Case10()
        {
            var fact = _standardFactsBuilder.BuildStopFactInstance("#123");

            _logger.Info("7B48027A-7B1C-4606-BE6F-6049BB5518AA", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("CB1F3592-5619-41A0-9032-08D0B98B0629", $"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case9()
        {
            var factStr = _standardFactsBuilder.BuildStopFactString("#123");

            _logger.Info("4FCFBE5E-896C-4AA7-AB44-EBFCDE733394", $"factStr = '{factStr}'");

            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            _logger.Info("88C53983-6668-44DD-B1C9-F1AAF272130D", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("BEF4D0B3-2B9F-47CE-A1A1-D492194E43EA", $"fact = {fact}");
        }

        private void Case8()
        {
            var fact = _standardFactsBuilder.BuildDeadFactInstance("#123");

            _logger.Info("EF5EDCCB-100C-468A-8EE3-AB90EC991300", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("3EA92B7B-5D3F-49A3-BB52-7C13B0BE5F2F", $"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case7()
        {
            var factStr = _standardFactsBuilder.BuildDeadFactString("#123");

            _logger.Info("77016B68-15F3-4CBF-922B-B004A176C449", $"factStr = '{factStr}'");

            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            _logger.Info("2BE74976-D969-4A4A-959C-C94B21494233", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("8259FD38-266A-402F-8A3A-7F64AF268A19", $"fact = {fact}");
        }

        private void Case6()
        {
            var fact = _standardFactsBuilder.BuildAliveFactInstance("#123");

            _logger.Info("1190832C-F6F0-4C05-98D9-4AE3E13A0FE4", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("9D18AB91-2733-46BF-AC91-8F8C5C4450BE", $"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case5()
        {
            var factStr = _standardFactsBuilder.BuildAliveFactString("#123");

            _logger.Info("2DFDA99B-06BE-4FDA-B616-C536E943F1DA", $"factStr = '{factStr}'");

            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            _logger.Info("8769F123-7E20-4C37-ABF1-C7FCDBBF821B", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("311604D3-6425-4D87-BE33-9323FE10B47A", $"fact = {fact}");
        }

        private void Case4()
        {
            var initialFactStr = "{: act(M16, shoot) :}";

            _logger.Info("049D222D-5CA2-4BD8-B5AA-1BFB3B6EE9B7", $"initialFactStr = {initialFactStr}");

            var initialFact = _engineContext.Parser.ParseRuleInstance(initialFactStr);

            _logger.Info("33BEC298-435A-4766-BDF0-ED609F30D089", $"initialFact = '{initialFact.ToHumanizedString()}'");

            var fact = _standardFactsBuilder.BuildSoundFactInstance(15.588457107543945, 12, initialFact);

            _logger.Info("50A282F8-4356-494B-84AA-0FE511408492", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("D7286FCD-8691-41DF-9518-796017D83224", $"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case3()
        {
            var factStr = _standardFactsBuilder.BuildSoundFactString(15.588457107543945, 12, "{: act(M16, shoot) :}");

            _logger.Info("40B21BFB-B30C-430B-B503-1B60A2213F93", $"factStr = '{factStr}'");
        }

        private void Case2()
        {
            var initialFactStr = "{: act(M16, shoot) :}";

            _logger.Info("0A63693A-0CE3-437D-8D10-2CA2251A6B4E", $"initialFactStr = {initialFactStr}");

            var initialFact = _engineContext.Parser.ParseRuleInstance(initialFactStr);

            _logger.Info("4D459069-CF69-413F-BEAD-F1BF2DB782DD", $"initialFact = '{initialFact.ToHumanizedString()}'");

            var fact = _standardFactsBuilder.BuildSayFactInstance("#123", initialFact);

            _logger.Info("DBEC6AA6-9B00-4A75-AA41-732F5E062EA8", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("7FB039C1-87EB-41F6-8FA0-930F85CD1372", $"fact = '{fact.ToHumanizedString(HumanizedOptions.ShowOnlyMainContent)}'");
        }

        private void Case1()
        {
            var factStr = _standardFactsBuilder.BuildSayFactString("#123", "{: act(M16, shoot) :}");

            _logger.Info("6B8D8320-D989-4276-B0D0-89CC41362036", $"factStr = '{factStr}'");

            var fact = _engineContext.Parser.ParseRuleInstance(factStr);

            _logger.Info("BF9085B0-7CF9-4F8B-9CEF-B7E978C55C11", $"fact = '{fact.ToHumanizedString()}'");
            _logger.Info("2993FB32-C3B7-4FFA-B260-403A86215359", $"fact = {fact}");

            _logger.Info("A236F56A-35EB-4A75-ADF1-316CD1C08935", $"fact.Normalized = '{fact.Normalized.ToHumanizedString()}'");
        }
    }
}
