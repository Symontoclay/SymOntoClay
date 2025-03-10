/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
using SymOntoClay.Common.DebugHelpers;
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
            _logger.Info("2079F60A-077A-4ED0-B2B1-A24896B220C4", "Begin");

            RunCase5();

            _logger.Info("2D704A7C-B3C2-4B96-87ED-1D79340F954C", "End");
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
            _logger.Info("0BF2668E-EFCC-450F-AEFF-C8E6FCEAFFEA", "Begin");

            var queryStr = string.Empty;

            queryStr = "{: male(#Tom) :}";
            ParseQueryStringAndGetLongHashCode(queryStr);

            queryStr = "{: >:{son($x, $y)} -> { male($x) & parent($y, $x)} :}";
            var a = ParseQueryStringAndGetLongHashCode(queryStr);

            queryStr = "{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}";
            var b = ParseQueryStringAndGetLongHashCode(queryStr);

            _logger.Info("026EA3EE-089E-47A9-B319-6036A7A5B7EA", $"a = {a}");
            _logger.Info("9CD7643A-1CCE-4013-A7CB-E268599920D5", $"b = {b}");

            if (a != b)
            {
                throw new NotImplementedException("D5673AE8-BD52-4E39-AB7A-09386082BDAA");
            }

            queryStr = "{: { love($x, $y) } -> { help($x, $y) } :}";
            ParseQueryStringAndGetLongHashCode(queryStr);

            queryStr = "{: ?x(?y, ?z) :}";
            ParseQueryStringAndGetLongHashCode(queryStr);

            _logger.Info("3161FF2D-2215-426E-B51A-DE07707D4222", "End");
        }

        private void RunGettingInheritanceInformation()
        {
            _logger.Info("80693A80-1BBD-4487-BE6B-DCEB44089305", "Begin");

            var queryStr = string.Empty;

            queryStr = "{: male(#Tom) :}";
            ParseQueryStringAndGetInheritanceInformation(queryStr);

            _logger.Info("A887DA09-6BF8-4E39-848A-CB6338B6882F", "End");
        }

        private void RunCase2()
        {
            _logger.Info("CC894FFB-2CE6-4811-9F90-9958152770A0", "Begin");

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

            _logger.Info("C474D1D5-EEC6-4931-80FF-615793F70256", "End");
        }

        private void RunCase1()
        {
            _logger.Info("87492D1F-7FED-4A19-90BC-6DA023321B37", "Begin");

            var queryStr = string.Empty;

            queryStr = "{: male(#Tom) :}";
            ParseQueryString(queryStr);

            queryStr = "{: parent(#Peter, #Tom) :}";
            ParseQueryString(queryStr);

            queryStr = "{: { son($x, $y) } -> { male($x) & parent($y, $x)} :}";
            ParseQueryString(queryStr);



            queryStr = "{: son(#Tom, #Peter) :}";
            Search(queryStr);



            _logger.Info("E321D751-A987-4CFB-95D7-D4CD5764E0E2", "End");
        }

        private void ParseFacts()
        {
            _logger.Info("C256D297-17FD-4133-A8CD-33293FF971C8", "Begin");

            var queryStr = string.Empty;


            queryStr = "{: male(#124) :}";
            ParseQueryString(queryStr);





            _logger.Info("E8889FC0-0D6C-44CD-B3DB-D31265ADBA44", "End");
        }

        private ulong ParseQueryStringAndGetLongHashCode(string queryStr)
        {
            _logger.Info("55112DDC-30F9-4AA2-BBF8-F10C191B3163", $"queryStr = {queryStr}");

            var result = new CodeFile();
            result.IsMain = false;
            result.FileName = "Hi!";

            var internalParserContext = new InternalParserContext(queryStr, result, _context);

            var globalStorage = _context.Storage.GlobalStorage;
            internalParserContext.SetCurrentDefaultSetings(globalStorage.DefaultSettingsOfCodeEntity);

            var parser = new LogicalQueryParser(internalParserContext);
            parser.Run();

            var parsedQuery = parser.Result;

            _logger.Info("B460E306-84B6-4E6A-89AC-B5835057A529", $"parsedQuery = {parsedQuery}");

            _logger.Info("4CA71172-B9E7-46D5-A13E-996A03C107ED", $"DebugHelperForRuleInstance.ToString(parsedQuery) = {DebugHelperForRuleInstance.ToString(parsedQuery)}");


            parsedQuery.CheckDirty();

            var longConditionalHashCode = parsedQuery.GetLongConditionalHashCode();

            _logger.Info("3BF42181-EA52-4E2D-9750-48BFDF617276", $"longConditionalHashCode = {longConditionalHashCode}");

            var longHashCode = parsedQuery.GetLongHashCode();

            _logger.Info("090D14E5-63E7-4EA3-B7BD-880285C6417B", $"longHashCode = {longHashCode}");

            return longHashCode;
        }

        private void ParseQueryStringAndGetInheritanceInformation(string queryStr)
        {
            _logger.Info("DF353C44-C299-40D1-A68B-85F5A0422661", $"queryStr = {queryStr}");

            var result = new CodeFile();
            result.IsMain = false;
            result.FileName = "Hi!";

            var internalParserContext = new InternalParserContext(queryStr, result, _context);

            var globalStorage = _context.Storage.GlobalStorage;
            internalParserContext.SetCurrentDefaultSetings(globalStorage.DefaultSettingsOfCodeEntity);

            var parser = new LogicalQueryParser(internalParserContext);
            parser.Run();

            var parsedQuery = parser.Result;

            _logger.Info("90B7C219-2209-458E-84EE-C3475864AB54", $"parsedQuery = {parsedQuery}");

            var inheritanceRelationsList = parsedQuery.GetInheritanceRelations(_logger);


            var inheritanceItemsList = new List<InheritanceItem>();

            var ruleInstanceName = parsedQuery.Name;

            foreach (var inheritanceRelation in inheritanceRelationsList)
            {
                _logger.Info("8A1B667D-D8B3-48EF-BB7D-C662AF2D1D96", $"inheritanceRelation = {inheritanceRelation}");

                var inheritanceItem = new InheritanceItem();

                inheritanceItem.SuperType = inheritanceRelation.Name;
                inheritanceItem.SubType = inheritanceRelation.ParamsList.Single().Name;
                inheritanceItem.Rank = new LogicalValue(1);

                inheritanceItem.KeysOfPrimaryRecords.Add(ruleInstanceName);

                inheritanceItemsList.Add(inheritanceItem);
            }

            _logger.Info("08F618A1-A6A9-4E7F-97DE-EC4639627534", $"inheritanceItemsList = {inheritanceItemsList.WriteListToString()}");
        }

        private void ParseQueryString(string queryStr)
        {
            _logger.Info("978095AB-306F-4D5E-8821-BBE3CA4E9EAD", $"queryStr = {queryStr}");

            var result = new CodeFile();
            result.IsMain = false;
            result.FileName = "Hi!";

            var internalParserContext = new InternalParserContext(queryStr, result, _context);

            var globalStorage = _context.Storage.GlobalStorage;
            internalParserContext.SetCurrentDefaultSetings(globalStorage.DefaultSettingsOfCodeEntity);

            var parser = new LogicalQueryParser(internalParserContext);
            parser.Run();

            var parsedQuery = parser.Result;


            _logger.Info("B1C83582-4CB8-467E-AAFE-D9D1BC0BCF3C", $"DebugHelperForRuleInstance.ToString(parsedQuery) = {DebugHelperForRuleInstance.ToString(parsedQuery)}");




            _context.Storage.GlobalStorage.LogicalStorage.Append(_logger, parsedQuery);

        }

        private void Search(string queryStr)
        {
            _logger.Info("24CD3A09-50A1-4EC2-A4F9-0CEB438AFF9D", $"queryStr = {queryStr}");

            var result = new CodeFile();
            result.IsMain = false;
            result.FileName = "Hi!";

            var internalParserContext = new InternalParserContext(queryStr, result, _context);

            var globalStorage = _context.Storage.GlobalStorage;
            internalParserContext.SetCurrentDefaultSetings(globalStorage.DefaultSettingsOfCodeEntity);

            var parser = new LogicalQueryParser(internalParserContext);
            parser.Run();

            var parsedQuery = parser.Result;


            _logger.Info("B07A0AE1-F9E2-4691-B7FD-251B62E98913", $"DebugHelperForRuleInstance.ToString(parsedQuery) = {DebugHelperForRuleInstance.ToString(parsedQuery)}");




            var searcher = _context.DataResolversFactory.GetLogicalSearchResolver();

            var searchOptions = new LogicalSearchOptions();
            searchOptions.QueryExpression = parsedQuery;

            var localCodeExecutionContext = new LocalCodeExecutionContext();
            searchOptions.LocalCodeExecutionContext = localCodeExecutionContext;
            localCodeExecutionContext.Storage = _context.Storage.GlobalStorage;
            localCodeExecutionContext.Holder = _context.CommonNamesStorage.DefaultHolder;


            var searchResult = searcher.Run(_logger, searchOptions);


            _logger.Info("BF5C1994-CE03-49B0-9D2F-B0F83E6D7AFF", $"searchResult.IsSuccess = {searchResult.IsSuccess}");
            _logger.Info("647788EC-5163-4777-BFBF-82154C2A6F09", $"searchResult.Items.Count = {searchResult.Items.Count}");

            _logger.Info("61E1D35E-F669-4CD0-AB22-724BAD33A20D", DebugHelperForLogicalSearchResult.ToString(searchResult));

            foreach (var item in searchResult.Items)
            {
                _logger.Info("9B0640EC-A3B1-4ED3-B0BF-4B42A74185F8", $"item.ResultOfVarOfQueryToRelationList.Count = {item.ResultOfVarOfQueryToRelationList.Count}");

                foreach (var resultOfVarOfQueryToRelation in item.ResultOfVarOfQueryToRelationList)
                {
                    var varName = resultOfVarOfQueryToRelation.NameOfVar;

                    _logger.Info("CC02C7DE-6066-4369-A113-21B5E21C7ABD", $"varName = {varName}");

                    var foundNode = resultOfVarOfQueryToRelation.FoundExpression;

                    _logger.Info("6A34DDC5-2078-46DA-AE73-7EFAD2BEAD93", $"DebugHelperForRuleInstance.ToString(foundNode) = {DebugHelperForRuleInstance.ToString(foundNode)}");
                }

            }
        }
    }
}
