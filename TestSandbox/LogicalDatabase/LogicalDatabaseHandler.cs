using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Parsing.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
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

            //RunGettingLongHashCode();
            //RunGettingInheritanceInformation();
            RunCase2();
            //RunCase1();
            //ParseFacts();

            _logger.Log("End");
        }

        private void RunGettingLongHashCode()
        {
            _logger.Log("Begin");

            var queryStr = string.Empty;

            queryStr = "{: male(#Tom) :}";
            ParseQueryStringAndGetLongHashCode(queryStr);

            queryStr = "{: >:{son($x, $y)} -> { male($x) & parent($y, $x)} :}";
            var a = ParseQueryStringAndGetLongHashCode(queryStr);

            queryStr = "{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}";
            var b = ParseQueryStringAndGetLongHashCode(queryStr);

            _logger.Log($"a = {a}");
            _logger.Log($"b = {b}");

            if (a != b)
            {
                throw new NotImplementedException();
            }

            queryStr = "{: { love($x, $y) } -> { help($x, $y) } :}";
            ParseQueryStringAndGetLongHashCode(queryStr);

            queryStr = "{: ?x(?y, ?z) :}";
            ParseQueryStringAndGetLongHashCode(queryStr);

            _logger.Log("End");
        }

        private void RunGettingInheritanceInformation()
        {
            _logger.Log("Begin");

            var queryStr = string.Empty;

            queryStr = "{: male(#Tom) :}";
            ParseQueryStringAndGetInheritanceInformation(queryStr);

            _logger.Log("End");
        }

        //This inheritance
        private void RunCase2()
        {
            _logger.Log("Begin");

            var queryStr = string.Empty;

            queryStr = "{: can(bird, fly) :}";
            ParseQueryString(queryStr);

            queryStr = "{: bird(#Alisa_12) :}";
            ParseQueryString(queryStr);

            queryStr = "{: can(#Alisa_12, ?x) :}";
            Search(queryStr);

            _logger.Log("End");
        }

        private void RunCase1()
        {
            _logger.Log("Begin");

            var queryStr = string.Empty;

            queryStr = "{: male(#Tom) :}";
            ParseQueryString(queryStr);

            queryStr = "{: parent(#Piter, #Tom) :}";
            ParseQueryString(queryStr);

            queryStr = "{: { son($x, $y) } -> { male($x) & parent($y, $x)} :}";
            ParseQueryString(queryStr);

            //queryStr = "{: son(?x, ?y) :}";
            //Search(queryStr);

            //queryStr = "{: ?z(?x, ?y) :}";
            //Search(queryStr);

            queryStr = "{: son(#Tom, #Piter) :}";
            Search(queryStr);

            //queryStr = "{: male(#Tom) :}";
            //Search(queryStr);

            //queryStr = "{: male(#Mary) :}";
            //Search(queryStr);

            _logger.Log("End");
        }

        private void ParseFacts()
        {
            _logger.Log("Begin");

            var queryStr = string.Empty;

            //queryStr = "{: >:{ male(#124) } :}";
            //ParseQueryString(queryStr);

            queryStr = "{: male(#124) :}";
            ParseQueryString(queryStr);

            //queryStr = "{: >:{son($x, $y)} -> { male($x) & parent($y, $x)} :}";
            //ParseQueryString(queryStr);

            //queryStr = "{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}";
            //ParseQueryString(queryStr);

            //queryStr = "{: { love($x, $y) } -> { help($x, $y) } :}";
            //ParseQueryString(queryStr);

            //queryStr = "{: ?x(?y, ?z) :}";
            //Search(queryStr);

            _logger.Log("End");
        }

        private ulong ParseQueryStringAndGetLongHashCode(string queryStr)
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

            var longConditionalHashCode = indexedQuery.GetLongConditionalHashCode();

            _logger.Log($"longConditionalHashCode = {longConditionalHashCode}");

            var longHashCode = indexedQuery.GetLongHashCode();

            _logger.Log($"longHashCode = {longHashCode}");

            return longHashCode;
        }

        private void ParseQueryStringAndGetInheritanceInformation(string queryStr)
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

            var inheritanceRelationsList = parsedQuery.GetInheritanceRelations();

            //_logger.Log($"inheritanceRelationsList = {inheritanceRelationsList.WriteListToString()}");

            var inheritanceItemsList = new List<InheritanceItem>();

            var ruleInstanceName = parsedQuery.Name.NameValue;

            foreach (var inheritanceRelation in inheritanceRelationsList)
            {
                _logger.Log($"inheritanceRelation = {inheritanceRelation}");

                var inheritanceItem = new InheritanceItem();

                inheritanceItem.SuperName = inheritanceRelation.Name;
                inheritanceItem.SubName = inheritanceRelation.ParamsList.Single().Name;
                inheritanceItem.Rank = new LogicalValue(1);

                inheritanceItem.KeysOfPrimaryRecords.Add(ruleInstanceName);

                inheritanceItemsList.Add(inheritanceItem);
            }

            _logger.Log($"inheritanceItemsList = {inheritanceItemsList.WriteListToString()}");
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

            var dictionary = _context.Dictionary;

            var parsedQuery = parser.Result;

            _logger.Log($"parsedQuery = {parsedQuery}");

            _logger.Log($"DebugHelperForRuleInstance.ToString(parsedQuery) = {DebugHelperForRuleInstance.ToString(parsedQuery)}");

            var indexedQuery = parsedQuery.GetIndexed(_context);

            _logger.Log($"indexedQuery = {indexedQuery}");

            _logger.Log($"DebugHelperForIndexedRuleInstance.ToString(indexedQuery) = {DebugHelperForIndexedRuleInstance.ToString(indexedQuery, dictionary)}");

            var searcher = _context.DataResolversFactory.GetLogicalSearchResolver();

            var searchOptions = new LogicalSearchOptions();
            searchOptions.QueryExpression = indexedQuery;

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            searchOptions.LocalCodeExecutionContext = localCodeExecutionContext;
            localCodeExecutionContext.Storage = _context.Storage.GlobalStorage;
            localCodeExecutionContext.Holder = _context.CommonNamesStorage.IndexedDefaultHolder;

            //_logger.Log($"searchOptions = {searchOptions}");

            var searchResult = searcher.Run(searchOptions);

            //_logger.Log($"searchResult = {searchResult}");

            _logger.Log($"searchResult.IsSuccess = {searchResult.IsSuccess}");
            _logger.Log($"searchResult.Items.Count = {searchResult.Items.Count}");

            _logger.Log(DebugHelperForLogicalSearchResult.ToString(searchResult, dictionary));

            foreach (var item in searchResult.Items)
            {
                _logger.Log($"item.ResultOfVarOfQueryToRelationList.Count = {item.ResultOfVarOfQueryToRelationList.Count}");

                foreach (var resultOfVarOfQueryToRelation in item.ResultOfVarOfQueryToRelationList)
                {
                    var varName = dictionary.GetName(resultOfVarOfQueryToRelation.KeyOfVar);

                    _logger.Log($"varName = {varName}");

                    var foundNode = resultOfVarOfQueryToRelation.FoundExpression;

                    _logger.Log($"DebugHelperForRuleInstance.ToString(foundNode) = {DebugHelperForIndexedRuleInstance.ToString(foundNode, dictionary)}");
                }

                //_logger.Log($" = {}");
            }
        }
    }
}
