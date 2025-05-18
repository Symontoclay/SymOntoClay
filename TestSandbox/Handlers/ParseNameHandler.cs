using Newtonsoft.Json.Linq;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Parsing.Internal;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using static System.Net.Mime.MediaTypeNames;

namespace TestSandbox.Handlers
{
    public class ParseNameHandler
    {
        public ParseNameHandler() 
        {
        }

        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        public void Run()
        {
            _logger.Info("5BF042DE-688A-48D8-8ECD-92233AF6D477", "Begin");

            Case19();
            //Case18();
            //Case17();
            //Case16();
            //Case15();
            //Case14();
            //Case13();
            //Case12();
            //Case12_a();
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

            _logger.Info("75A8C4DA-0D97-496F-AF78-258D4B8A5043", "End");
        }

        private void Case19()
        {
            var str = "number[∞]";

            _logger.Info("396F028A-2866-49CC-89F3-9B088F11B1ED", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("2C61516A-7AAF-47BE-84EB-BAFFC0211171", $"result = {result}");
            _logger.Info("86FE1940-1E03-4E23-A4A3-7C59E8728224", $"result = {result.ToHumanizedLabel()}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("FA56E3E9-E9B1-4DE3-8E8F-808F516FDEA1", $"token = {token}");
            //}
        }

        private void Case18()
        {
            var str = "number[*]";

            _logger.Info("2408E45E-6E97-4FC3-8683-89EFC3FC4C59", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("94B50052-56AE-4F7C-B359-2796455AA64B", $"result = {result}");
            _logger.Info("DDC6F23F-A54A-460C-9CF6-42BCBD920ADA", $"result = {result.ToHumanizedLabel()}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("2F410076-8A01-4B50-A836-0554D74CFA81", $"token = {token}");
            //}
        }

        private void Case17()
        {
            var str = "number[]";

            _logger.Info("2B5EADF7-293B-456E-A703-E9DC03F256F5", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("AFFB83E0-A5FF-4EEF-90B7-15D710211BC8", $"result = {result}");
            _logger.Info("A828FA55-1F22-4BAC-82BD-FEA871EDBF9A", $"result = {result.ToHumanizedLabel()}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("9165610D-5CEA-48D0-860E-8AA26A20EB6B", $"token = {token}");
            //}
        }

        private void Case16()
        {
            var str = "number[5]";

            _logger.Info("797C8BA7-79F1-4DB4-BC02-AA59B6AD5AB3", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("0B9D7CE5-265C-401E-8237-AFECFDE73D11", $"result = {result}");
            _logger.Info("D9B8B4B6-D4F7-4065-BD29-732AF846E299", $"result = {result.ToHumanizedLabel()}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("8C0FE1D8-D29D-40E1-86FD-ECF3593FE044", $"token = {token}");
            //}
        }

        private void Case15()
        {
            var str = "global(politics)::dog (animal (alive))";

            _logger.Info("B95CA8C8-7921-4C3B-BAF9-3A9AE6C7C5F1", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("F397E863-CF82-4E16-9C35-7B0C968BAC93", $"result = {result}");
            _logger.Info("F12A1488-B85D-4ACB-BEA0-4A329925C7E3", $"result = {result.ToHumanizedLabel()}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("D3999E58-C7DF-4BE6-8F1A-8CB30198111F", $"token = {token}");
            //}
        }

        private void Case14()
        {
            var str = "global::##Prop1";

            _logger.Info("E47B42AA-7EF4-4F10-A8DB-AAF232414610", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("7CDA1037-1000-4B8A-9B2B-D3000FBE9F57", $"result = {result}");
            _logger.Info("8AE917F3-E0DE-4785-8215-328108554674", $"result = {result.ToHumanizedLabel()}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("8689EB54-64DA-4462-B9B7-4BA51CEC4E10", $"token = {token}");
            //}
        }

        private void Case13()
        {
            var str = "global::Prop1";

            _logger.Info("9900B991-57DD-4963-A2BD-23BC257BA431", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("BB7E2D65-5947-4545-9A58-9B710D1AEB78", $"result = {result}");
            _logger.Info("D25A1045-84C8-475A-8CCB-624146A2DF62", $"result = {result.ToHumanizedLabel()}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("3E42C8BF-FA63-43E5-B58F-4EC15D10B479", $"token = {token}");
            //}
        }

        private void Case12()
        {
            var str = "@:`small`";

            _logger.Info("00BFEA04-11BC-4D7D-A734-67F00DBF274C", $"str = '{str}'");

            var oldIdentifier = NameHelper.CreateName(str);

            _logger.Info("CF1AFD01-3EB5-4FA0-AA6F-8660BDD03C7B", $"oldIdentifier = {oldIdentifier}");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("E982B6A6-8D9B-4ACB-A8BF-25A416F6CB20", $"result = {result}");
            _logger.Info("2A6B6313-4C9C-4658-B79E-A13EF1D92C1D", $"result = {result.ToHumanizedLabel()}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("F06803C2-0BA4-4786-BC40-D65B88194898", $"token = {token}");
            //}
        }

        private void Case12_a()
        {
            var str = "@:`small dog`";

            _logger.Info("F89FDC9A-422F-488B-9B12-4217050718C0", $"str = '{str}'");

            var oldIdentifier = NameHelper.CreateName(str);

            _logger.Info("9B0C0273-477C-4013-9570-634012B18B57", $"oldIdentifier = {oldIdentifier}");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info("43A60565-9579-412D-8D8C-13793E430D30", $"result = {result}");
            _logger.Info("1D327D44-1B0E-4B45-8C0F-F50C61DE5E2B", $"result = {result.ToHumanizedLabel()}");

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("6B4D33BA-4A82-45FE-ABC1-B87F6DAE21EF", $"token = {token}");
            //}
        }

        private void Case11()
        {
            var str = "@>`dog`";

            _logger.Info("DE752BF9-5F41-44B3-8665-16C103EF299C", $"str = '{str}'");

            var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            Token token = null;

            while ((token = lexer.GetToken()) != null)
            {
                _logger.Info("0DB9E800-39A1-4C76-ADB2-684C4F2E6D55", $"token = {token}");
            }
        }

        private void Case10()
        {
            var str = "@@`dog`";

            _logger.Info("74820A91-066D-4C66-9D71-7053103F4CA1", $"str = '{str}'");

            var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            Token token = null;

            while ((token = lexer.GetToken()) != null)
            {
                _logger.Info("9E1D43CC-68C1-48C9-B558-C00BBC0A0AA3", $"token = {token}");
            }
        }

        private void Case9()
        {
            var str = "@`dog`";

            _logger.Info("5979A9D4-1E06-42C3-B56E-A575D18DD1B2", $"str = '{str}'");

            var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            Token token = null;

            while ((token = lexer.GetToken()) != null)
            {
                _logger.Info("34FDEE2C-989D-433C-95BB-9A948853B373", $"token = {token}");
            }
        }

        private void Case8()
        {
            var str = "$`dog`";

            _logger.Info("5FD716CD-6A41-44EA-A40C-FCFFF266BBF3", $"str = '{str}'");

            var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            Token token = null;

            while ((token = lexer.GetToken()) != null)
            {
                _logger.Info("315947C3-DFA6-453F-8B29-B793F4B02E9B", $"token = {token}");
            }
        }

        private void Case7()
        {
            var str = "#|`dog`";

            _logger.Info("E3486139-EBBC-4885-ABD3-AE7D8AED38BE", $"str = '{str}'");

            var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            Token token = null;

            while ((token = lexer.GetToken()) != null)
            {
                _logger.Info("2C700970-0D98-4B14-B0BA-E1983EA8F6F3", $"token = {token}");
            }
        }

        private void Case6()
        {
            var str = "#^`dog`";

            _logger.Info("A8C58289-A166-4FBC-B94F-0ABC6DBFEBDA", $"str = '{str}'");

            var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            Token token = null;

            while ((token = lexer.GetToken()) != null)
            {
                _logger.Info("E7C6D716-0E31-49A8-B39B-A9687D567ECE", $"token = {token}");
            }
        }

        private void Case5()
        {
            var str = "##@`dog`";

            _logger.Info("9463EB7D-D873-4FAF-A560-396D285CB8A2", $"str = '{str}'");

            var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            Token token = null;

            while ((token = lexer.GetToken()) != null)
            {
                _logger.Info("E00149A2-1E95-45E6-9C95-4CC1007ADD7D", $"token = {token}");
            }
        }

        private void Case4()
        {
            var str = "##`dog`";

            _logger.Info("43718E7B-15FB-4B5D-B8E8-2E20498FF78B", $"str = '{str}'");

            var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            Token token = null;

            while ((token = lexer.GetToken()) != null)
            {
                _logger.Info("31425952-058B-4275-BB11-4D38ACAAAFA1", $"token = {token}");
            }
        }

        private void Case3()
        {
            var str = "#@`dog`";

            _logger.Info("61FF1248-2508-4927-8E78-CA3D0BB88AA1", $"str = '{str}'");

            var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            Token token = null;

            while ((token = lexer.GetToken()) != null)
            {
                _logger.Info("51C31E42-96F7-4BE7-82A9-07647470CDF8", $"token = {token}");
            }
        }

        private void Case2()
        {
            var str = "#`dog`";

            _logger.Info("3197872C-C2FC-4B9C-8BE8-A6C64CE913F3", $"str = '{str}'");

            var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            Token token = null;

            while((token = lexer.GetToken()) != null)
            {
                _logger.Info("4E050FF3-E1CB-4362-B626-B5EAF07C9F4F", $"token = {token}");
            }
        }

        private void Case1()
        {
            var str = "`dog`";

            _logger.Info("D00EF65D-519F-4C02-A61F-E082D198EE8F", $"str = '{str}'");

            var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            Token token = null;

            while ((token = lexer.GetToken()) != null)
            {
                _logger.Info("4E050FF3-E1CB-4362-B626-B5EAF07C9F4F", $"token = {token}");
            }
        }
    }
}
