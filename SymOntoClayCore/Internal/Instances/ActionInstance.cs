/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class ActionInstance : BaseInstance, IExecutable
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
