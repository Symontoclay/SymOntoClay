using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class SelectLogicalQueryOperatorHandler : BaseLoggedComponent, IUnaryOperatorHandler
    {
        public SelectLogicalQueryOperatorHandler(IEngineContext engineContext)
            : base(engineContext.Logger)
        {
            _engineContext = engineContext;

            var dataResolversFactory = engineContext.DataResolversFactory;

            _searcher = dataResolversFactory.GetLogicalSearchResolver();
        }

        private readonly IEngineContext _engineContext;
        private readonly LogicalSearchResolver _searcher;

        /// <inheritdoc/>
        public IndexedValue Call(IndexedValue operand, IndexedValue annotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            Log($"operand = {operand}");
            Log($"annotation = {annotation}");
#endif

            if(!operand.IsRuleInstanceValue)
            {
                throw new NotImplementedException();
            }

            var indexedQuery = operand.AsRuleInstanceValue.IndexedRuleInstance;

#if DEBUG
            Log($"indexedQuery = {indexedQuery}");
            Log($"query = {DebugHelperForIndexedRuleInstance.ToString(indexedQuery, _engineContext.Dictionary)}");
#endif

            var searchOptions = new LogicalSearchOptions();
            searchOptions.QueryExpression = indexedQuery;
            searchOptions.LocalCodeExecutionContext = localCodeExecutionContext;

#if DEBUG
            Log($"searchOptions = {searchOptions}");
#endif

            var searchResult = _searcher.Run(searchOptions);

#if DEBUG
            Log($"searchResult = {searchResult}");
            Log($"result = {DebugHelperForLogicalSearchResult.ToString(searchResult, _engineContext.Dictionary)}");
#endif

            return new LogicalSearchResultValue(searchResult).GetIndexed(_engineContext);
        }
    }
}
