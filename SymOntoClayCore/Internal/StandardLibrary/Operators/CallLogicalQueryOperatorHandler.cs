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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Visitors;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class CallLogicalQueryOperatorHandler : BaseOperatorHandler, IUnaryOperatorHandler
    {
        public CallLogicalQueryOperatorHandler(IEngineContext engineContext)
            : base(engineContext)
        {
            _engineContext = engineContext;

            var globalStorage = engineContext.Storage.GlobalStorage;
            _globalLogicalStorage = globalStorage.LogicalStorage;

            var dataResolversFactory = engineContext.DataResolversFactory;

            _searcher = dataResolversFactory.GetLogicalSearchResolver();
            _varsResolver = dataResolversFactory.GetVarsResolver();
        }

        private readonly IEngineContext _engineContext;
        private readonly LogicalSearchResolver _searcher;
        private readonly VarsResolver _varsResolver;
        private readonly ILogicalStorage _globalLogicalStorage;        

        /// <inheritdoc/>
        public CallResult Call(IMonitorLogger logger, Value operand, Value annotation, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            operand = TryResolveFromVarOrExpr(logger, operand, localCodeExecutionContext);

            if (!operand.IsLogicalQueryOperationValue)
            {
                throw new NotSupportedException();
            }

            var val = operand.AsLogicalQueryOperationValue;

            var kindOfLogicalQueryOperation = val.KindOfLogicalQueryOperation;

            switch(kindOfLogicalQueryOperation)
            {
                case KindOfLogicalQueryOperation.Select:
                    return ProcessSelect(logger, val, annotation, localCodeExecutionContext);

                case KindOfLogicalQueryOperation.Insert:
                    return ProcessInsert(logger, val, annotation, localCodeExecutionContext);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfLogicalQueryOperation), kindOfLogicalQueryOperation, null);
            }
        }

        private CallResult ProcessSelect(IMonitorLogger logger, LogicalQueryOperationValue operand, Value annotation, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            if(operand.Target == null)
            {
                throw new NotImplementedException("434D3277-5D90-4BF7-B282-8F3604E3E8F5");
            }

            if(operand.Source != null)
            {
                throw new NotImplementedException("CDCFB545-EAAB-4546-8824-18BBD1D28F2E");
            }

            if(operand.Dest != null)
            {
                throw new NotImplementedException("882D4E1E-C493-4F78-B205-BCFA4D14C6F2");
            }

            var target = operand.Target;

            if (!target.IsRuleInstance)
            {
                throw new NotImplementedException("2C323E77-66D3-4602-A6BE-E9A10CBE8B17");
            }

            var query = target.AsRuleInstance;

            var searchOptions = new LogicalSearchOptions();
            searchOptions.ResolvingNotResultsStrategy = ResolvingNotResultsStrategy.InResolver;
            searchOptions.ReplacingNotResultsStrategy = ReplacingNotResultsStrategy.AllKindOfItems;
            searchOptions.QueryExpression = query;
            searchOptions.LocalCodeExecutionContext = localCodeExecutionContext;

            var searchResult = _searcher.Run(logger, searchOptions);

            return new CallResult(new LogicalSearchResultValue(searchResult));
        }

        private CallResult ProcessInsert(IMonitorLogger logger, LogicalQueryOperationValue operand, Value annotation, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            if (operand.Target == null)
            {
                throw new NotImplementedException("14D66638-BE1D-4038-9E94-C5C606EABBCB");
            }

            if (operand.Source != null)
            {
                throw new NotImplementedException("9AD3EA97-ADD1-44A3-A951-D6CFBD858659");
            }

            if (operand.Dest != null)
            {
                throw new NotImplementedException("0509CA5F-6324-4BF3-A698-71D8C5AC9D82");
            }

            var target = operand.Target;

            if (!target.IsRuleInstance)
            {
                throw new NotImplementedException("C67D9167-BAEB-471D-861B-699E48FFAB60");
            }

            var ruleInstance = target.AsRuleInstance;

            if (ruleInstance.Holder == null)
            {
                ruleInstance.Holder = localCodeExecutionContext.Holder;
            }

            if(ruleInstance.TypeOfAccess == TypeOfAccess.Local)
            {
                ruleInstance.TypeOfAccess = CodeItem.DefaultTypeOfAccess;
            }

            if (ruleInstance.IsParameterized)
            {
                ruleInstance = ruleInstance.Clone();

                var packedVarsResolver = new PackedVarsResolver(_varsResolver, localCodeExecutionContext);
                var resolveVariablesLogicalVisitor = new ResolveVariablesLogicalVisitor(_engineContext.Logger);
                resolveVariablesLogicalVisitor.Run(ruleInstance, packedVarsResolver);

                ruleInstance.CheckDirty();
            }            

            _globalLogicalStorage.Append(logger, ruleInstance);

            return new CallResult(target);
        }
    }
}
