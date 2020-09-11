using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public interface ILogicalStorage: ISpecificStorage
    {
        void Append(RuleInstance ruleInstance);
        void Append(RuleInstance ruleInstance, bool isPrimary);

        event Action OnChanged;

        IList<RelationIndexedLogicalQueryNode> GetAllRelations();
    }
}
