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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class ObjectInstance: BaseIndependentInstance
    {
        public ObjectInstance(CodeItem codeItem, IEngineContext context, IStorage parentStorage, ILocalCodeExecutionContext parentCodeExecutionContext)
            : base(codeItem, context, parentStorage, parentCodeExecutionContext, context.StorageFactories.ObjectStorageFactory, null)
        {
            var dataResolversFactory = context.DataResolversFactory;

            _methodsResolver = dataResolversFactory.GetMethodsResolver();
            _varsResolver = dataResolversFactory.GetVarsResolver();
            _strongIdentifierExprValueResolver = dataResolversFactory.GetStrongIdentifierExprValueResolver();

            _function = codeItem.AsFunction;

            if(_function != null)
            {
                _functionExecutable = new ExecutableWithTargetLocalContext(_function, parentCodeExecutionContext);
            }
        }

        /// <inheritdoc/>
        public override KindOfInstance KindOfInstance => KindOfInstance.ObjectInstance;

        private readonly Function _function;
        private readonly ExecutableWithTargetLocalContext _functionExecutable;
        private readonly MethodsResolver _methodsResolver;
        private readonly VarsResolver _varsResolver;
        private readonly StrongIdentifierExprValueResolver _strongIdentifierExprValueResolver;

        /// <inheritdoc/>
        public override IExecutable GetExecutable(IMonitorLogger logger, KindOfFunctionParameters kindOfParameters, IDictionary<StrongIdentifierValue, Value> namedParameters, IList<Value> positionedParameters)
        {
            if(_function != null)
            {
                if(_methodsResolver.IsFit(logger, _function, kindOfParameters, namedParameters, positionedParameters, _localCodeExecutionContext))
                {
                    return _functionExecutable;
                }

                return null;
            }

            throw new NotImplementedException("5B3EB4DF-A09D-4DB2-B28E-038F2F8E6CB9");
        }

        /// <inheritdoc/>
        public override IMember GetMember(IMonitorLogger logger, StrongIdentifierValue memberName)
        {
#if DEBUG
            Info("63F8DB00-73FF-4A5C-91AE-F5BB80D71742", $"memberName = {memberName}");
#endif

            return _strongIdentifierExprValueResolver.GetMember(logger, memberName, this, _localCodeExecutionContext);
        }

        /// <inheritdoc/>
        public override ValueCallResult SetPropertyValue(IMonitorLogger logger, StrongIdentifierValue propertyName, Value value)
        {
            throw new NotImplementedException("E9A0088C-5940-455B-9FE3-9FC23431B078");
        }

        /// <inheritdoc/>
        public override ValueCallResult SetVarValue(IMonitorLogger logger, StrongIdentifierValue varName, Value value)
        {
            return _varsResolver.SetVarValue(logger, varName, value, _localCodeExecutionContext);
        }

        /// <inheritdoc/>
        public override Value GetPropertyValue(IMonitorLogger logger, StrongIdentifierValue propertyName)
        {
            throw new NotImplementedException("E44BF62D-00E4-4E1C-9428-A42FF977093A");
        }

        /// <inheritdoc/>
        public override Value GetVarValue(IMonitorLogger logger, StrongIdentifierValue varName)
        {
            return _varsResolver.GetVarValue(logger, varName, _localCodeExecutionContext);
        }

        /// <inheritdoc/>
        protected override void RunPreConstructors(IMonitorLogger logger)
        {
        }

        /// <inheritdoc/>
        protected override void RunConstructors(IMonitorLogger logger)
        {
        }
    }
}
