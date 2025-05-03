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

            Case3();
            //Case2();
            //Case1();

            _logger.Info("75A8C4DA-0D97-496F-AF78-258D4B8A5043", "End");
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
