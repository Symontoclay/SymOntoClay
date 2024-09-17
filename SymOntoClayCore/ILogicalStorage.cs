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
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface ILogicalStorage: ISpecificStorage, IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToDbgString, IObjectToHumanizedString, IMonitoredHumanizedObject
    {
        void Append(IMonitorLogger logger, RuleInstance ruleInstance);
        void Append(IMonitorLogger logger, RuleInstance ruleInstance, bool isPrimary);
        void Append(IMonitorLogger logger, IList<RuleInstance> ruleInstancesList);

        void Remove(IMonitorLogger logger, RuleInstance ruleInstance);
        void Remove(IMonitorLogger logger, IList<RuleInstance> ruleInstancesList);
        void RemoveById(IMonitorLogger logger, string id);

        void AddOnChangedHandler(IOnChangedLogicalStorageHandler handler);
        void RemoveOnChangedHandler(IOnChangedLogicalStorageHandler handler);

        void AddOnChangedWithKeysHandler(IOnChangedWithKeysLogicalStorageHandler handler);
        void RemoveOnChangedWithKeysHandler(IOnChangedWithKeysLogicalStorageHandler handler);

        void AddOnAddingFactHandler(IOnAddingFactHandler handler);
        void RemoveOnAddingFactHandler(IOnAddingFactHandler handler);

        IList<LogicalQueryNode> GetAllRelations(IMonitorLogger logger, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode);
        IList<RuleInstance> GetAllOriginFacts(IMonitorLogger logger);
        IList<BaseRulePart> GetIndexedRulePartOfFactsByKeyOfRelation(IMonitorLogger logger, StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode);
        IList<BaseRulePart> GetIndexedRulePartWithOneRelationWithVarsByKeyOfRelation(IMonitorLogger logger, StrongIdentifierValue name, ILogicalSearchStorageContext logicalSearchStorageContext, LogicalSearchExplainNode parentExplainNode, LogicalSearchExplainNode rootParentExplainNode);
        IReadOnlyList<LogicalQueryNode> GetLogicalQueryNodes(IMonitorLogger logger, IList<LogicalQueryNode> exceptList, ReplacingNotResultsStrategy replacingNotResultsStrategy, IList<KindOfLogicalQueryNode> targetKindsOfItems);

        void DbgPrintFactsAndRules(IMonitorLogger logger);
    }
}
