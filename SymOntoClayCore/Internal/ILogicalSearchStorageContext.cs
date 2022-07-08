using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public interface ILogicalSearchStorageContext
    {
        IList<T> Filter<T>(IList<T> source, bool enableModalitiesControll) where T : ILogicalSearchItem;
        IList<T> Filter<T>(IList<T> source, IDictionary<RuleInstance, IItemWithModalities> additionalModalities) where T : ILogicalSearchItem;
        IList<T> Filter<T>(IList<T> source, bool enableModalitiesControll, IDictionary<RuleInstance, IItemWithModalities> additionalModalities) where T : ILogicalSearchItem;
    }
}
