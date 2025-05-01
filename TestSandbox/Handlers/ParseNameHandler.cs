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

            Case2();
            //Case1();

            _logger.Info("75A8C4DA-0D97-496F-AF78-258D4B8A5043", "End");
        }

        private void Case2()
        {
            var str = "#`dog`";

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

            var lexer = new Lexer(str, _logger, LexerMode.StrongIdentifier);

            Token token = null;

            while ((token = lexer.GetToken()) != null)
            {
                _logger.Info("4E050FF3-E1CB-4362-B626-B5EAF07C9F4F", $"token = {token}");
            }
        }
    }
}
