using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class ExecutableWithTargetLocalContext: IExecutable
    {
        public ExecutableWithTargetLocalContext(IExecutable executable, LocalCodeExecutionContext localCodeExecutionContext)
        {
            _executable = executable;
            _localCodeExecutionContext = localCodeExecutionContext;
        }

        private readonly IExecutable _executable;
        private readonly LocalCodeExecutionContext _localCodeExecutionContext;

        /// <inheritdoc/>
        public bool IsSystemDefined => _executable.IsSystemDefined;

        /// <inheritdoc/>
        public IList<IFunctionArgument> Arguments => _executable.Arguments;

        /// <inheritdoc/>
        public CompiledFunctionBody CompiledFunctionBody => _executable.CompiledFunctionBody;

        /// <inheritdoc/>
        public CodeItem CodeItem => _executable.CodeItem;

        /// <inheritdoc/>
        public ISystemHandler SystemHandler => _executable.SystemHandler;

        /// <inheritdoc/>
        public LocalCodeExecutionContext OwnLocalCodeExecutionContext => _localCodeExecutionContext;

        /// <inheritdoc/>
        public StrongIdentifierValue Holder => _executable.Holder;

        /// <inheritdoc/>
        public IExecutionCoordinator TryActivate(IEngineContext context)
        {
            return _executable.TryActivate(context);
        }

        /// <inheritdoc/>
        public bool ContainsArgument(StrongIdentifierValue name)
        {
            return _executable.ContainsArgument(name);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return _executable.ToString(n);
        }

        /// <inheritdoc/>
        public string PropertiesToString(uint n)
        {
            return _executable.PropertiesToString(n);
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return _executable.ToShortString();
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return _executable.ToShortString(n);
        }

        /// <inheritdoc/>
        public string PropertiesToShortString(uint n)
        {
            return _executable.PropertiesToShortString(n);
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return _executable.ToBriefString();
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return _executable.ToBriefString(n);
        }

        /// <inheritdoc/>
        public string PropertiesToBriefString(uint n)
        {
            return _executable.PropertiesToBriefString(n);
        }
    }
}
