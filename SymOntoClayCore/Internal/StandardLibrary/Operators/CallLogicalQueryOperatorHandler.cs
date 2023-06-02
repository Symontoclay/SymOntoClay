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

using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Visitors;
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
        public Value Call(Value operand, Value annotation, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            operand = TryResolveFromVarOrExpr(operand, localCodeExecutionContext);

            if (!operand.IsLogicalQueryOperationValue)
            {
                throw new NotSupportedException();
            }

            var val = operand.AsLogicalQueryOperationValue;

            var kindOfLogicalQueryOperation = val.KindOfLogicalQueryOperation;

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

        private Value ProcessSelect(LogicalQueryOperationValue operand, Value annotation, ILocalCodeExecutionContext localCodeExecutionContext)
        {
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

            if (!target.IsRuleInstance)
            {
                throw new NotImplementedException();
            }

            var query = target.AsRuleInstance;

            var searchOptions = new LogicalSearchOptions();
            searchOptions.ResolvingNotResultsStrategy = ResolvingNotResultsStrategy.InResolver;
            searchOptions.ReplacingNotResultsStrategy = ReplacingNotResultsStrategy.AllKindOfItems;
            searchOptions.QueryExpression = query;
            searchOptions.LocalCodeExecutionContext = localCodeExecutionContext;

            var searchResult = _searcher.Run(searchOptions);

            return new LogicalSearchResultValue(searchResult);
        }

        private Value ProcessInsert(LogicalQueryOperationValue operand, Value annotation, ILocalCodeExecutionContext localCodeExecutionContext)
        {
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

            if (!target.IsRuleInstance)
            {
                throw new NotImplementedException();
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

            _globalLogicalStorage.Append(ruleInstance);

            return target;
        }
    }
}
