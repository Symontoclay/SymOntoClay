/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;

namespace SymOntoClay.Core
{
    public interface ITasksStorage : ISpecificStorage
    {
        BaseCompoundHtnTask GetBaseCompoundTaskByName(IMonitorLogger logger, StrongIdentifierValue name, KindOfTask requestingKindOfTask);
        BaseHtnTask GetBaseTaskByName(IMonitorLogger logger, StrongIdentifierValue name, KindOfTask requestingKindOfTask);

        #region RootTask
        void Append(IMonitorLogger logger, RootHtnTask rootTask);
        RootHtnTask GetRootTask(IMonitorLogger logger, StrongIdentifierValue name);
        IEnumerable<RootHtnTask> GetAllRootTasks(IMonitorLogger logger);
        #endregion

        #region StrategicTask
        void Append(IMonitorLogger logger, StrategicHtnTask strategicTask);
        StrategicHtnTask GetStrategicTask(IMonitorLogger logger, StrongIdentifierValue name);
        #endregion

        #region TacticalTask
        void Append(IMonitorLogger logger, TacticalHtnTask tacticalTask);
        TacticalHtnTask GetTacticalTask(IMonitorLogger logger, StrongIdentifierValue name);
        #endregion

        #region CompoundTask
        void Append(IMonitorLogger logger, CompoundHtnTask compoundTask);
        CompoundHtnTask GetCompoundTaskByName(IMonitorLogger logger, StrongIdentifierValue name);
        #endregion

        #region PrimitiveTask
        void Append(IMonitorLogger logger, PrimitiveHtnTask primitiveTask);
        PrimitiveHtnTask GetPrimitiveTaskByName(IMonitorLogger logger, StrongIdentifierValue name);
        #endregion
    }
}
