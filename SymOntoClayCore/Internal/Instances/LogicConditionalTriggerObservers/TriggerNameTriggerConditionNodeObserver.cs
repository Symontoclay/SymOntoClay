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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.Core.Internal.DataResolvers;
using System.Collections.Generic;
using System.Linq;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers
{
    public class TriggerNameTriggerConditionNodeObserver : BaseTriggerConditionNodeObserver
    {
        public TriggerNameTriggerConditionNodeObserver(IEngineContext engineContext, IStorage storage, TriggerConditionNode condition, KindOfTriggerCondition kindOfTriggerCondition)
            : base(engineContext.Logger)
        {
            _triggerName = condition.Name;
            
            _storage = storage;
            _synonymsResolver = engineContext.DataResolversFactory.GetSynonymsResolver();

            _synonymsList = _synonymsResolver.GetSynonyms(Logger, _triggerName, storage);

            storage.TriggersStorage.OnNamedTriggerInstanceChangedWithKeys += TriggersStorage_OnNamedTriggerInstanceChangedWithKeys;//no need
        }

        private readonly StrongIdentifierValue _triggerName;
        private readonly List<StrongIdentifierValue> _synonymsList;
        private readonly IStorage _storage;
        private readonly SynonymsResolver _synonymsResolver;

        private void TriggersStorage_OnNamedTriggerInstanceChangedWithKeys(IList<StrongIdentifierValue> namesList)
        {
            if (namesList.Contains(_triggerName))
            {
                EmitOnChanged();
            }
            else
            {
                if(_synonymsList?.Intersect(namesList).Any() ?? false)
                {
                    EmitOnChanged();
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            _storage.TriggersStorage.OnNamedTriggerInstanceChangedWithKeys -= TriggersStorage_OnNamedTriggerInstanceChangedWithKeys;

            base.OnDisposed();
        }
    }
}
