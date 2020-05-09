using NLog;
using SymOntoClay.Core.Internal.Parsing.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Parsing
{
    public class ParsingHandler
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            var logger = new LoggerImpementation();

            var text = @"app Enemy
{
};";

            var lexer = new Lexer(text, logger);

            Token token = null;

            while((token = lexer.GetToken()) != null)
            {
                _logger.Info($"token = {token}");
            }

            _logger.Info("End");
        }
    }
}
