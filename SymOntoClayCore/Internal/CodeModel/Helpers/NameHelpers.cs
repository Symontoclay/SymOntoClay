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

            name.DictionaryName = dictionary.Name;
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

        public static List<SimpleName> CreateSimpleNames(Name name, IEntityDictionary dictionary)
        {
            if (name.Namespaces.IsNullOrEmpty())
            {
                var item = new SimpleName();
                item.DictionaryName = dictionary.Name;
                item.NameValue = name.NameValue;
                item.FullNameValue = name.NameValue;
                CalculateIndex(item, dictionary);
                return new List<SimpleName>() { item };
            }
            else
            {
                throw new NotSupportedException("Namespaces are not supported yet!");
            }
        }

        public static void CalculateIndex(SimpleName name, IEntityDictionary dictionary)
        {
            name.FullNameKey = dictionary.GetKey(name.FullNameValue);
        }
    }
}
