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

using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers
{
    public class VarTriggerConditionNodeObserver : BaseTriggerConditionNodeObserver, IOnChangedWithKeysVarStorageHandler
    {
        public VarTriggerConditionNodeObserver(IMonitorLogger logger, IStorage storage, TriggerConditionNode condition, KindOfTriggerCondition kindOfTriggerCondition)
            : base(logger)
        {
            _varName = condition.Name;
            
            storage.VarStorage.AddOnChangedWithKeysHandler(this);
            _storage = storage;
        }

        private readonly StrongIdentifierValue _varName;
        private readonly IStorage _storage;

        void IOnChangedWithKeysVarStorageHandler.Invoke(StrongIdentifierValue value)
        {
            VarStorage_OnChangedWithKeys(value);
        }

        private void VarStorage_OnChangedWithKeys(StrongIdentifierValue varName)
        {
            if (_varName == varName)
            {
                EmitOnChanged();
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _storage.VarStorage.RemoveOnChangedWithKeysHandler(this);

            base.OnDisposed();
        }
    }
}
