using NLog;
using SymOntoClay.Core.Internal.Parsing.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
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

            TstParser();
            //TstLexer();

            _logger.Info("End");
        }

        private void TstParser()
        {
            _logger.Info("Begin");

            var text = @"app Enemy
{
};";

            var parserContext = new TstParserContext();

            var internalParserContext = new InternalParserContext(text, parserContext);

            var parser = new SourceCodeParser(internalParserContext);
            parser.Run();

            var result = parser.Result;

            _logger.Info($"parsedFileInfoList = {result.WriteListToString()}");

            //Token token = null;

            //while ((token = internalParserContext.GetToken()) != null)
            //{
            //    _logger.Info($"token = {token}");
            //}

            _logger.Info("End");
        }

        private void TstLexer()
        {
            _logger.Info("Begin");

            var logger = new LoggerImpementation();

            var text = @"app Enemy
{
};";

            var lexer = new Lexer(text, logger);

            Token token = null;

            while ((token = lexer.GetToken()) != null)
            {
                _logger.Info($"token = {token}");
            }

            _logger.Info("End");
        }
    }
}
