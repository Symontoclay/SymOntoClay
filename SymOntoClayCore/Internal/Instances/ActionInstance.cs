using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class ActionInstance : InstanceInfo, IExecutable
    {
        public ActionInstance(ActionPtr actionPtr, IEngineContext context, IStorage parentStorage)
            : base(actionPtr.Action.Name, context, parentStorage)
        {
            _action = actionPtr.Action;
            _operator = actionPtr.Operator;

            _iOp = _operator;
        }

        private readonly ActionDef _action;
        private readonly Operator _operator;
        private readonly IExecutable _iOp;

        public IExecutionCoordinator ExecutionCoordinator => throw new NotImplementedException();

        //public void Init()
        //{
        //    throw new NotImplementedException();
        //}

        /// <inheritdoc/>
        IExecutionCoordinator IExecutable.TryActivate(IEngineContext context)
        {
            return null;
        }

        /// <inheritdoc/>
        public bool IsSystemDefined => _iOp.IsSystemDefined;

        /// <inheritdoc/>
        public IList<IFunctionArgument> Arguments => _iOp.Arguments;

        /// <inheritdoc/>
        public CompiledFunctionBody CompiledFunctionBody => _iOp.CompiledFunctionBody;

        /// <inheritdoc/>
        public CodeEntity CodeEntity => _iOp.CodeEntity;

        /// <inheritdoc/>
        public ISystemHandler SystemHandler => _iOp.SystemHandler;

        /// <inheritdoc/>
        public bool ContainsArgument(StrongIdentifierValue name)
        {
            return _iOp.ContainsArgument(name);
        }
    }
}
