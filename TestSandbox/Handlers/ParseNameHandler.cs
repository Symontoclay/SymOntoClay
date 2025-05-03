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

            Case7();
            //Case6();
            //Case5();
            //Case4();
            //Case3();
            //Case2();
            //Case1();

            _logger.Info("75A8C4DA-0D97-496F-AF78-258D4B8A5043", "End");
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
