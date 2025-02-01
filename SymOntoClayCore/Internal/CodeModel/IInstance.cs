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

using SymOntoClay.Common;
using SymOntoClay.Common.Disposing;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public interface IInstance: ISymOntoClayDisposable, IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToHumanizedString, IMonitoredHumanizedObject
    {
        KindOfInstance KindOfInstance { get; }
        StrongIdentifierValue Name { get; }
        float Priority { get; }
        CodeItem CodeItem { get; }
        IExecutionCoordinator ExecutionCoordinator { get; }
        ILocalCodeExecutionContext LocalCodeExecutionContext { get; }
        void CancelExecution(IMonitorLogger logger, string messagePointId, ReasonOfChangeStatus reasonOfChangeStatus, Changer changer = null, string callMethodId = "");
        void CancelExecution(IMonitorLogger logger, string messagePointId, ReasonOfChangeStatus reasonOfChangeStatus, List<Changer> changers, string callMethodId = "");
        void AddChildInstance(IMonitorLogger logger, IInstance instance);
        void RemoveChildInstance(IMonitorLogger logger, IInstance instance);
        void SetParent(IMonitorLogger logger, IInstance instance);
        void ResetParent(IMonitorLogger logger, IInstance instance);
        IList<IInstance> GetTopIndependentInstances(IMonitorLogger logger);
        bool ActivateIdleAction(IMonitorLogger logger);
        IExecutable GetExecutable(IMonitorLogger logger, KindOfFunctionParameters kindOfParameters, IDictionary<StrongIdentifierValue, Value> namedParameters, IList<Value> positionedParameters);
        void SetPropertyValue(IMonitorLogger logger, StrongIdentifierValue propertyName, Value value);
        void SetVarValue(IMonitorLogger logger, StrongIdentifierValue varName, Value value);
        Value GetPropertyValue(IMonitorLogger logger, StrongIdentifierValue propertyName);
        Value GetVarValue(IMonitorLogger logger, StrongIdentifierValue varName);
    }
}
