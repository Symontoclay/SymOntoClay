/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public interface IInstance: ISymOntoClayDisposable, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        KindOfInstance KindOfInstance { get; }
        StrongIdentifierValue Name { get; }
        float Priority { get; }
        IExecutionCoordinator ExecutionCoordinator { get; }
        void CancelExecution();
        void AddChildInstance(IInstance instance);
        void RemoveChildInstance(IInstance instance);
        void SetParent(IInstance instance);
        void ResetParent(IInstance instance);
        IList<IInstance> GetTopIndependentInstances();
        bool ActivateIdleAction();
        IExecutable GetExecutable(KindOfFunctionParameters kindOfParameters, IDictionary<StrongIdentifierValue, Value> namedParameters, IList<Value> positionedParameters);
        void SetPropertyValue(StrongIdentifierValue propertyName, Value value);
        void SetVarValue(StrongIdentifierValue varName, Value value);
        Value GetPropertyValue(StrongIdentifierValue propertyName);
        Value GetVarValue(StrongIdentifierValue varName);
    }
}
