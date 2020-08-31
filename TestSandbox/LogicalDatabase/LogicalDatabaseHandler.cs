using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Parsing.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using TestSandbox.Helpers;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.LogicalDatabase
{
    public class LogicalDatabaseHandler
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();
        private readonly TstComplexContext _genericContext;
        private readonly EngineContext _context;

        public LogicalDatabaseHandler()
        {
            _genericContext = TstEngineContextHelper.CreateAndInitContext();
            _context = _genericContext.EngineContext;
        }

        public void Run()
        {
            _logger.Log("Begin");

            ParseFacts();

            _logger.Log("End");
        }

        private void ParseFacts()
        {
            _logger.Log("Begin");

            var queryStr = "{: >:{ male(#124) } :}";
            ParseQueryString(queryStr);

            _logger.Log("End");
        }

        private void ParseQueryString(string queryStr)
        {
            _logger.Log($"queryStr = {queryStr}");

            var result = new CodeFile();
            result.IsMain = false;
            result.FileName = "Hi!";

            var internalParserContext = new InternalParserContext(queryStr, result, _context);

            var globalStorage = _context.Storage.GlobalStorage;
            internalParserContext.SetCurrentDefaultSetings(globalStorage.DefaultSettingsOfCodeEntity);

            var parser = new LogicalQueryParser(internalParserContext);
            parser.Run();

            var parsedQuery = parser.Result;

            _logger.Log($"parsedQuery = {parsedQuery}");

            throw new NotImplementedException();
        }
    }
}
