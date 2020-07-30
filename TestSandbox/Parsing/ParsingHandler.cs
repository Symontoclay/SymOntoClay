using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Parsing.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using TestSandbox.Helpers;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Parsing
{
    public class ParsingHandler
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            _logger.Info("Begin");

            TstParser2();
            //TstLexer2();
            //TstParser1();
            //TstLexer1();

            _logger.Info("End");
        }

        private void TstParser2()
        {
            _logger.Info("Begin");

            var text = @"app Enemy
{
    on Init => {
	     'Hello world!' >> @>log;

    }
}";

            var parserContext = new TstMainStorageContext();

            var codeFile = new CodeFile();

            var internalParserContext = new InternalParserContext(text, codeFile, parserContext);

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

        private void TstLexer2()
        {
            _logger.Info("Begin");

            var logger = new LoggerImpementation();

            var text = @"app Enemy
{
    on Init => {
	     'Hello world!' >> @>log;

    }
}";

            var lexer = new Lexer(text, logger);

            Token token = null;

            while ((token = lexer.GetToken()) != null)
            {
                _logger.Info($"token = {token}");
            }

            _logger.Info("End");
        }

        private void TstParser1()
        {
            _logger.Info("Begin");

            var text = @"app Enemy
{
}";

            var parserContext = new TstMainStorageContext();

            var codeFile = new CodeFile();

            var internalParserContext = new InternalParserContext(text, codeFile, parserContext);

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

        private void TstLexer1()
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
