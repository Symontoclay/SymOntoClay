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
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface IVarStorage: ISpecificStorage
    {
        void SetSystemValue(IMonitorLogger logger, StrongIdentifierValue varName, Value value);
        Value GetSystemValueDirectly(IMonitorLogger logger, StrongIdentifierValue varName);
        
        void Append(IMonitorLogger logger, VarInstance varItem);
        IList<WeightedInheritanceResultItem<VarInstance>> GetVarDirectly(IMonitorLogger logger, StrongIdentifierValue name, IList<WeightedInheritanceItem> weightedInheritanceItems);
        VarInstance GetLocalVarDirectly(IMonitorLogger logger, StrongIdentifierValue name);

        void SetValue(IMonitorLogger logger, StrongIdentifierValue varName, Value value, ILocalCodeExecutionContext localCodeExecutionContext);

        void AddOnChangedHandler(IOnChangedVarStorageHandler handler);
        void RemoveOnChangedHandler(IOnChangedVarStorageHandler handler);

        void AddOnChangedWithKeysHandler(IOnChangedWithKeysVarStorageHandler handler);
        void RemoveOnChangedWithKeysHandler(IOnChangedWithKeysVarStorageHandler handler);

#if DEBUG
        void DbgPrintVariables(IMonitorLogger logger);
#endif
    }
}
