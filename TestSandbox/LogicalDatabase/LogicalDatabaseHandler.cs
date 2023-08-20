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

using SymOntoClay.BaseTestLib;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Parsing.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
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
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();
        private readonly ComplexTestEngineContext _genericContext;
        private readonly IEngineContext _context;

        public LogicalDatabaseHandler()
        {
            _genericContext = TstEngineContextHelper.CreateAndInitContext();
            _context = _genericContext.EngineContext;
        }

        public void Run()
        {
            _logger.Info("Begin");

            RunCase5();

            _logger.Info("End");
        }

        private void RunCase5()
        {
            var queryStr = string.Empty;

            queryStr = "{: enemy(#q) :}";
            ParseQueryString(queryStr);

            queryStr = "{: see(I, #q) :}";
            ParseQueryString(queryStr);

            queryStr = "{: see(I, ?x) & enemy(?x) :}";
            Search(queryStr);
        }

        private void RunCase4()
        {
            var queryStr = string.Empty;

            queryStr = "{: >: { $x = distance(I, enemy) & value($x, 30) } :}";
            ParseQueryString(queryStr);
        }

        private void RunCase3()
        {
            var queryStr = string.Empty;


            queryStr = "{: weight(#Tom, NULL) :}";
            ParseQueryString(queryStr);

            queryStr = "{: >: { weight(#Tom, ?x) } :}";
            Search(queryStr);
        }

        private void RunGettingLongHashCode()
        {
            _logger.Info("Begin");

            var queryStr = string.Empty;

            queryStr = "{: male(#Tom) :}";
            ParseQueryStringAndGetLongHashCode(queryStr);

            queryStr = "{: >:{son($x, $y)} -> { male($x) & parent($y, $x)} :}";
            var a = ParseQueryStringAndGetLongHashCode(queryStr);

            queryStr = "{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}";
            var b = ParseQueryStringAndGetLongHashCode(queryStr);

            _logger.Info($"a = {a}");
            _logger.Info($"b = {b}");

            if (a != b)
            {
                throw new NotImplementedException();
            }

            queryStr = "{: { love($x, $y) } -> { help($x, $y) } :}";
            ParseQueryStringAndGetLongHashCode(queryStr);

            queryStr = "{: ?x(?y, ?z) :}";
            ParseQueryStringAndGetLongHashCode(queryStr);

            _logger.Info("End");
        }

        private void RunGettingInheritanceInformation()
        {
            _logger.Info("Begin");

            var queryStr = string.Empty;

            queryStr = "{: male(#Tom) :}";
            ParseQueryStringAndGetInheritanceInformation(queryStr);

            _logger.Info("End");
        }

        private void RunCase2()
        {
            _logger.Info("Begin");

            var queryStr = string.Empty;

            queryStr = "{: >: { can(bird, fly) } :}";
            ParseQueryString(queryStr);

            queryStr = "{: bird(#Alisa_12) :}";
            ParseQueryString(queryStr);

            queryStr = "{: >: { can(#Alisa_12, ?x) } :}";
            Search(queryStr);

            queryStr = "{: can(#Alisa_12, fly) :}";
            Search(queryStr);

            queryStr = "{: ?z(#Alisa_12, ?x) :}";
            Search(queryStr);

            _logger.Info("End");
        }

        private void RunCase1()
        {
            _logger.Info("Begin");

            var queryStr = string.Empty;

            queryStr = "{: male(#Tom) :}";
            ParseQueryString(queryStr);

            queryStr = "{: parent(#Piter, #Tom) :}";
            ParseQueryString(queryStr);

            queryStr = "{: { son($x, $y) } -> { male($x) & parent($y, $x)} :}";
            ParseQueryString(queryStr);



            queryStr = "{: son(#Tom, #Piter) :}";
            Search(queryStr);



            _logger.Info("End");
        }

        private void ParseFacts()
        {
            _logger.Info("Begin");

            var queryStr = string.Empty;


            queryStr = "{: male(#124) :}";
            ParseQueryString(queryStr);





            _logger.Info("End");
        }

        private ulong ParseQueryStringAndGetLongHashCode(string queryStr)
        {
            _logger.Info($"queryStr = {queryStr}");

            var result = new CodeFile();
            result.IsMain = false;
            result.FileName = "Hi!";

            var internalParserContext = new InternalParserContext(queryStr, result, _context);

            var globalStorage = _context.Storage.GlobalStorage;
            internalParserContext.SetCurrentDefaultSetings(globalStorage.DefaultSettingsOfCodeEntity);

            var parser = new LogicalQueryParser(internalParserContext);
            parser.Run();

            var parsedQuery = parser.Result;

            _logger.Info($"parsedQuery = {parsedQuery}");

            _logger.Info($"DebugHelperForRuleInstance.ToString(parsedQuery) = {DebugHelperForRuleInstance.ToString(parsedQuery)}");


            parsedQuery.CheckDirty();

            var longConditionalHashCode = parsedQuery.GetLongConditionalHashCode();

            _logger.Info($"longConditionalHashCode = {longConditionalHashCode}");

            var longHashCode = parsedQuery.GetLongHashCode();

            _logger.Info($"longHashCode = {longHashCode}");

            return longHashCode;
        }

        private void ParseQueryStringAndGetInheritanceInformation(string queryStr)
        {
            _logger.Info($"queryStr = {queryStr}");

            var result = new CodeFile();
            result.IsMain = false;
            result.FileName = "Hi!";

            var internalParserContext = new InternalParserContext(queryStr, result, _context);

            var globalStorage = _context.Storage.GlobalStorage;
            internalParserContext.SetCurrentDefaultSetings(globalStorage.DefaultSettingsOfCodeEntity);

            var parser = new LogicalQueryParser(internalParserContext);
            parser.Run();

            var parsedQuery = parser.Result;

            _logger.Info($"parsedQuery = {parsedQuery}");

            var inheritanceRelationsList = parsedQuery.GetInheritanceRelations();


            var inheritanceItemsList = new List<InheritanceItem>();

            var ruleInstanceName = parsedQuery.Name;

            foreach (var inheritanceRelation in inheritanceRelationsList)
            {
                _logger.Info($"inheritanceRelation = {inheritanceRelation}");

                var inheritanceItem = new InheritanceItem();

                inheritanceItem.SuperName = inheritanceRelation.Name;
                inheritanceItem.SubName = inheritanceRelation.ParamsList.Single().Name;
                inheritanceItem.Rank = new LogicalValue(1);

                inheritanceItem.KeysOfPrimaryRecords.Add(ruleInstanceName);

                inheritanceItemsList.Add(inheritanceItem);
            }

            _logger.Info($"inheritanceItemsList = {inheritanceItemsList.WriteListToString()}");
        }

        private void ParseQueryString(string queryStr)
        {
            _logger.Info($"queryStr = {queryStr}");

            var result = new CodeFile();
            result.IsMain = false;
            result.FileName = "Hi!";

            var internalParserContext = new InternalParserContext(queryStr, result, _context);

            var globalStorage = _context.Storage.GlobalStorage;
            internalParserContext.SetCurrentDefaultSetings(globalStorage.DefaultSettingsOfCodeEntity);

            var parser = new LogicalQueryParser(internalParserContext);
            parser.Run();

            var parsedQuery = parser.Result;


            _logger.Info($"DebugHelperForRuleInstance.ToString(parsedQuery) = {DebugHelperForRuleInstance.ToString(parsedQuery)}");




            _context.Storage.GlobalStorage.LogicalStorage.Append(parsedQuery);

        }

        private void Search(string queryStr)
        {
            _logger.Info($"queryStr = {queryStr}");

            var result = new CodeFile();
            result.IsMain = false;
            result.FileName = "Hi!";

            var internalParserContext = new InternalParserContext(queryStr, result, _context);

            var globalStorage = _context.Storage.GlobalStorage;
            internalParserContext.SetCurrentDefaultSetings(globalStorage.DefaultSettingsOfCodeEntity);

            var parser = new LogicalQueryParser(internalParserContext);
            parser.Run();

            var parsedQuery = parser.Result;


            _logger.Info($"DebugHelperForRuleInstance.ToString(parsedQuery) = {DebugHelperForRuleInstance.ToString(parsedQuery)}");




            var searcher = _context.DataResolversFactory.GetLogicalSearchResolver();

            var searchOptions = new LogicalSearchOptions();
            searchOptions.QueryExpression = parsedQuery;

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            searchOptions.LocalCodeExecutionContext = localCodeExecutionContext;
            localCodeExecutionContext.Storage = _context.Storage.GlobalStorage;
            localCodeExecutionContext.Holder = _context.CommonNamesStorage.DefaultHolder;


            var searchResult = searcher.Run(searchOptions);


            _logger.Info($"searchResult.IsSuccess = {searchResult.IsSuccess}");
            _logger.Info($"searchResult.Items.Count = {searchResult.Items.Count}");

            _logger.Info(DebugHelperForLogicalSearchResult.ToString(searchResult));

            foreach (var item in searchResult.Items)
            {
                _logger.Info($"item.ResultOfVarOfQueryToRelationList.Count = {item.ResultOfVarOfQueryToRelationList.Count}");

                foreach (var resultOfVarOfQueryToRelation in item.ResultOfVarOfQueryToRelationList)
                {
                    var varName = resultOfVarOfQueryToRelation.NameOfVar;

                    _logger.Info($"varName = {varName}");

                    var foundNode = resultOfVarOfQueryToRelation.FoundExpression;

                    _logger.Info($"DebugHelperForRuleInstance.ToString(foundNode) = {DebugHelperForRuleInstance.ToString(foundNode)}");
                }

            }
        }
    }
}
