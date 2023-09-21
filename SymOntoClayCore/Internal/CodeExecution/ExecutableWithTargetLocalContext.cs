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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class ExecutableWithTargetLocalContext: IExecutable
    {
        public ExecutableWithTargetLocalContext(IExecutable executable, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            _executable = executable;
            _localCodeExecutionContext = localCodeExecutionContext;
        }

        private readonly IExecutable _executable;
        private readonly ILocalCodeExecutionContext _localCodeExecutionContext;

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
        public ILocalCodeExecutionContext OwnLocalCodeExecutionContext => _localCodeExecutionContext;

        /// <inheritdoc/>
        public StrongIdentifierValue Holder => _executable.Holder;

        /// <inheritdoc/>
        IExecutionCoordinator IExecutable.GetCoordinator(IMonitorLogger logger, IEngineContext context, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return _executable.GetCoordinator(logger, context, localCodeExecutionContext);
        }

        /// <inheritdoc/>
        IExecutable IExecutable.Activate(IMonitorLogger logger, IEngineContext context, ILocalCodeExecutionContext localCodeExecutionContext, IExecutionCoordinator executionCoordinator)
        {
            return _executable.Activate(logger, context, localCodeExecutionContext, executionCoordinator);
        }

        /// <inheritdoc/>
        public bool ContainsArgument(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return _executable.ContainsArgument(logger, name);
        }

        /// <inheritdoc/>
        public bool NeedActivation => _executable.NeedActivation;

        /// <inheritdoc/>
        public bool IsActivated => _executable.IsActivated;

        /// <inheritdoc/>
        public UsingLocalCodeExecutionContextPreferences UsingLocalCodeExecutionContextPreferences => _executable.UsingLocalCodeExecutionContextPreferences;

        /// <inheritdoc/>
        public bool IsInstance => _executable.IsInstance;

        /// <inheritdoc/>
        public IInstance AsInstance => _executable.AsInstance;

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
