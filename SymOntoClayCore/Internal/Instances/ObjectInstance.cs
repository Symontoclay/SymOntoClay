using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class ObjectInstance: BaseIndependentInstance
    {
        public ObjectInstance(CodeItem codeItem, IEngineContext context, IStorage parentStorage, LocalCodeExecutionContext parentCodeExecutionContext)
            : base(codeItem, context, parentStorage, parentCodeExecutionContext, new ObjectStorageFactory(), null)
        {
            var dataResolversFactory = context.DataResolversFactory;

            _methodsResolver = dataResolversFactory.GetMethodsResolver();
            _varsResolver = dataResolversFactory.GetVarsResolver();

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

        /// <inheritdoc/>
        public override IExecutable GetExecutable(KindOfFunctionParameters kindOfParameters, IDictionary<StrongIdentifierValue, Value> namedParameters, IList<Value> positionedParameters)
        {
            if(_function != null)
            {
                if(_methodsResolver.IsFit(_function, kindOfParameters, namedParameters, positionedParameters, _localCodeExecutionContext))
                {
                    return _functionExecutable;
                }

                return null;
            }

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void SetPropertyValue(StrongIdentifierValue propertyName, Value value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void SetVarValue(StrongIdentifierValue varName, Value value)
        {
#if DEBUG
            //Log($"varName = {varName}");
            //Log($"value = {value}");
#endif

            _varsResolver.SetVarValue(varName, value, _localCodeExecutionContext);
        }

        /// <inheritdoc/>
        public override Value GetPropertyValue(StrongIdentifierValue propertyName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Value GetVarValue(StrongIdentifierValue varName)
        {
#if DEBUG
            //Log($"varName = {varName}");
#endif

            return _varsResolver.GetVarValue(varName, _localCodeExecutionContext);
        }
    }
}
