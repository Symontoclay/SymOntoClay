using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Helpers
{
    public static class NameHelpers
    {
        public static Name CreateName(string text, List<string> targetNamespaces, IEntityDictionary dictionary)
        {
            var name = new Name() { IsEmpty = false };

            if (text.Contains("::") || text.Contains("("))
            {
                throw new NotSupportedException("Symbols `::`, `(` and `)` are not supported yet!");
            }

            name.NameValue = text;

            CalculateIndex(name, dictionary);

            return name;
        }

        public static void CalculateIndex(Name name, IEntityDictionary dictionary)
        {
            if (name.IsEmpty)
            {
                return;
            }

            name.NameKey = dictionary.GetKey(name.NameValue);

            if (name.Namespaces.IsNullOrEmpty())
            {
                name.FullNameKeys = new List<ulong>() { name.NameKey };
            }
            else
            {
                throw new NotSupportedException("Namespaces are not supported yet!");
            }
        }
    }
}
