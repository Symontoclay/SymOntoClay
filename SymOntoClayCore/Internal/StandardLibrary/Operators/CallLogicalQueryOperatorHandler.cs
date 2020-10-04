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
    public class CallLogicalQueryOperatorHandler : BaseLoggedComponent, IUnaryOperatorHandler
    {
        public CallLogicalQueryOperatorHandler(IEngineContext engineContext)
            : base(engineContext.Logger)
        {
            _engineContext = engineContext;

            var globalStorage = engineContext.Storage.GlobalStorage;
            _logicalStorage = globalStorage.LogicalStorage;

            var dataResolversFactory = engineContext.DataResolversFactory;

            _searcher = dataResolversFactory.GetLogicalSearchResolver();
        }

        private readonly IEngineContext _engineContext;
        private readonly LogicalSearchResolver _searcher;
        private readonly ILogicalStorage _logicalStorage;

        /// <inheritdoc/>
        public IndexedValue Call(IndexedValue operand, IndexedValue annotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
            if(!operand.IsLogicalQueryOperationValue)
            {
                throw new NotSupportedException();
            }

            var val = operand.AsLogicalQueryOperationValue;

            var kindOfLogicalQueryOperation = val.KindOfLogicalQueryOperation;

#if DEBUG
            //Log($"kindOfLogicalQueryOperation = {kindOfLogicalQueryOperation}");
#endif

            switch(kindOfLogicalQueryOperation)
            {
                case KindOfLogicalQueryOperation.Select:
                    return ProcessSelect(val, annotation, localCodeExecutionContext);

                case KindOfLogicalQueryOperation.Insert:
                    return ProcessInsert(val, annotation, localCodeExecutionContext);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfLogicalQueryOperation), kindOfLogicalQueryOperation, null);
            }
        }

        private IndexedValue ProcessSelect(IndexedLogicalQueryOperationValue operand, IndexedValue annotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"operand = {operand}");
            //Log($"annotation = {annotation}");
#endif

            if(operand.Target == null)
            {
                throw new NotImplementedException();
            }

            if(operand.Source != null)
            {
                throw new NotImplementedException();
            }

            if(operand.Dest != null)
            {
                throw new NotImplementedException();
            }

            var target = operand.Target;

            if (!target.IsRuleInstanceValue)
            {
                throw new NotImplementedException();
            }

            var indexedQuery = target.AsRuleInstanceValue.IndexedRuleInstance;

#if DEBUG
            //Log($"indexedQuery = {indexedQuery}");
            //Log($"query = {DebugHelperForIndexedRuleInstance.ToString(indexedQuery, _engineContext.Dictionary)}");
#endif

            var searchOptions = new LogicalSearchOptions();
            searchOptions.QueryExpression = indexedQuery;
            searchOptions.LocalCodeExecutionContext = localCodeExecutionContext;

#if DEBUG
            //Log($"searchOptions = {searchOptions}");
#endif

            var searchResult = _searcher.Run(searchOptions);

#if DEBUG
            //Log($"searchResult = {searchResult}");
            //Log($"result = {DebugHelperForLogicalSearchResult.ToString(searchResult, _engineContext.Dictionary)}");
#endif

            return new LogicalSearchResultValue(searchResult).GetIndexed(_engineContext);
        }

        private IndexedValue ProcessInsert(IndexedLogicalQueryOperationValue operand, IndexedValue annotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"operand = {operand}");
            //Log($"annotation = {annotation}");
#endif

            if (operand.Target == null)
            {
                throw new NotImplementedException();
            }

            if (operand.Source != null)
            {
                throw new NotImplementedException();
            }

            if (operand.Dest != null)
            {
                throw new NotImplementedException();
            }

            var target = operand.Target;

            if (!target.IsRuleInstanceValue)
            {
                throw new NotImplementedException();
            }

            var ruleInstance = target.AsRuleInstanceValue.RuleInstance;

#if DEBUG
            //Log($"ruleInstance = {DebugHelperForRuleInstance.ToString(ruleInstance)}");
#endif

            _logicalStorage.Append(ruleInstance);

            return target;
        }
    }
}
