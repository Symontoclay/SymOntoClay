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
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;

namespace SymOntoClay.Core
{
    public interface ITriggersStorage : ISpecificStorage
    {
        void Append(IMonitorLogger logger, InlineTrigger inlineTrigger);
        IList<WeightedInheritanceResultItem<InlineTrigger>> GetSystemEventsTriggersDirectly(IMonitorLogger logger, KindOfSystemEventOfInlineTrigger kindOfSystemEvent, IList<WeightedInheritanceItem> weightedInheritanceItems);
        IList<WeightedInheritanceResultItem<InlineTrigger>> GetLogicConditionalTriggersDirectly(IMonitorLogger logger, IList<WeightedInheritanceItem> weightedInheritanceItems);
        IList<WeightedInheritanceResultItem<InlineTrigger>> GetAddFactTriggersDirectly(IMonitorLogger logger, IList<WeightedInheritanceItem> weightedInheritanceItems);

        void Append(IMonitorLogger logger, INamedTriggerInstance namedTriggerInstance);
        void Remove(IMonitorLogger logger, INamedTriggerInstance namedTriggerInstance);

        void AddOnNamedTriggerInstanceChangedHandler(IOnNamedTriggerInstanceChangedTriggersStorageHandler handler);
        void RemoveOnNamedTriggerInstanceChangedHandler(IOnNamedTriggerInstanceChangedTriggersStorageHandler handler);

        void AddOnNamedTriggerInstanceChangedWithKeysHandler(IOnNamedTriggerInstanceChangedWithKeysTriggersStorageHandler handler);
        void RemoveOnNamedTriggerInstanceChangedWithKeysHandler(IOnNamedTriggerInstanceChangedWithKeysTriggersStorageHandler handler);

        IList<INamedTriggerInstance> GetNamedTriggerInstancesDirectly(IMonitorLogger logger, StrongIdentifierValue name);
    }
}
