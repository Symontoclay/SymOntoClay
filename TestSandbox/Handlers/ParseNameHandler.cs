using SymOntoClay.Core.Internal.Parsing.Internal;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;

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

            Case14();
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

            _logger.Info("75A8C4DA-0D97-496F-AF78-258D4B8A5043", "End");
        }

        private void Case14()
        {
            var str = "global::##Prop1";

            _logger.Info("E47B42AA-7EF4-4F10-A8DB-AAF232414610", $"str = '{str}'");

            var parserContext = new InternalParserCoreContext(str, _logger, LexerMode.StrongIdentifier);

            var parser = new StrongIdentifierValueParser(parserContext);
            parser.Run();

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

            //var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            //Token token = null;

            //while ((token = lexer.GetToken()) != null)
            //{
            //    _logger.Info("3E42C8BF-FA63-43E5-B58F-4EC15D10B479", $"token = {token}");
            //}
        }

        private void Case12()
        {
            var str = "@:`dog`";

            _logger.Info("00BFEA04-11BC-4D7D-A734-67F00DBF274C", $"str = '{str}'");

            var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            Token token = null;

            while ((token = lexer.GetToken()) != null)
            {
                _logger.Info("F06803C2-0BA4-4786-BC40-D65B88194898", $"token = {token}");
            }
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
