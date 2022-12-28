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
            _methodsResolver = context.DataResolversFactory.GetMethodsResolver();

            _function = codeItem.AsFunction;
        }

        /// <inheritdoc/>
        public override KindOfInstance KindOfInstance => KindOfInstance.ObjectInstance;

        private readonly Function _function;
        private readonly MethodsResolver _methodsResolver;

        /// <inheritdoc/>
        public override IExecutable GetExecutable(KindOfFunctionParameters kindOfParameters, IDictionary<StrongIdentifierValue, Value> namedParameters, IList<Value> positionedParameters)
        {
            if(_function != null)
            {
                if(_methodsResolver.IsFit(_function, kindOfParameters, namedParameters, positionedParameters, _localCodeExecutionContext))
                {
                    return _function;
                }

                return null;
            }

            throw new NotImplementedException();
        }
    }
}
