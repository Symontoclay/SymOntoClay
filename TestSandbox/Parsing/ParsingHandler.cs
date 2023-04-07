/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
