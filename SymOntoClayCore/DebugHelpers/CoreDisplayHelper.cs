using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SymOntoClay.Core.DebugHelpers
{
    public static class CoreDisplayHelper
    {
        public static string TypesListToHumanizedString(this IEnumerable<StrongIdentifierValue> typesList)
        {
            if (typesList.IsNullOrEmpty())
            {
                return StandardNamesConstants.AnyTypeName;
            }

            if(typesList.Count() == 1)
            {
                return typesList.Single().ToHumanizedLabel();
            }

            return $"({string.Join(" | ", typesList.Select(p => p.ToHumanizedLabel()))})";
        }
    }
}
