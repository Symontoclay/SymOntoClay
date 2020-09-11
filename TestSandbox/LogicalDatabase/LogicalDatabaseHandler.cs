using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
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

            var queryStr = string.Empty;

            queryStr = "{: >:{ male(#124) } :}";
            ParseQueryString(queryStr);

            //queryStr = "{: male(#124) :}";
            //ParseQueryString(queryStr);

            queryStr = "{: >:{son($x, $y)} -> { male($x) & parent($y, $x)} :}";
            ParseQueryString(queryStr);

            //queryStr = "{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}";
            //ParseQueryString(queryStr);

            queryStr = "{: ?x(?y, ?z) :}";
            Search(queryStr);

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

            _logger.Log($"DebugHelperForRuleInstance.ToString(parsedQuery) = {DebugHelperForRuleInstance.ToString(parsedQuery)}");

            //var indexedQuery = parsedQuery.GetIndexed(_context);

            //_logger.Log($"indexedQuery = {indexedQuery}");

            //_logger.Log($"DebugHelperForIndexedRuleInstance.ToString(indexedQuery) = {DebugHelperForIndexedRuleInstance.ToString(indexedQuery, _context.Dictionary)}");

            _context.Storage.GlobalStorage.LogicalStorage.Append(parsedQuery);

            //throw new NotImplementedException();
        }

        private void Search(string queryStr)
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

            _logger.Log($"DebugHelperForRuleInstance.ToString(parsedQuery) = {DebugHelperForRuleInstance.ToString(parsedQuery)}");

            var indexedQuery = parsedQuery.GetIndexed(_context);

            _logger.Log($"indexedQuery = {indexedQuery}");

            _logger.Log($"DebugHelperForIndexedRuleInstance.ToString(indexedQuery) = {DebugHelperForIndexedRuleInstance.ToString(indexedQuery, _context.Dictionary)}");

            var searcher = _context.DataResolversFactory.GetLogicalSearchResolver();

            var searchOptions = new LogicalSearchOptions();
            searchOptions.QueryExpression = indexedQuery;

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            searchOptions.LocalCodeExecutionContext = localCodeExecutionContext;
            localCodeExecutionContext.Storage = _context.Storage.GlobalStorage;
            localCodeExecutionContext.Holder = _context.CommonNamesStorage.IndexedDefaultHolder;

            //_logger.Log($"searchOptions = {searchOptions}");

            var searchResult = searcher.Run(searchOptions);

            _logger.Log($"searchResult = {searchResult}");
        }
    }
}
