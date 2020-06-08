using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Helpers
{
    public static class NameHelpers
    {
        public static Name CreateRuleOrFactName(IEntityDictionary dictionary)
        {
            var text = $"#{Guid.NewGuid().ToString("D")}";
            return CreateName(text, dictionary);
        }

        public static Name CreateName(string text, IEntityDictionary dictionary)
        {
            var name = new Name() { IsEmpty = false };

            if (text.Contains("::") || text.Contains("("))
            {
                throw new NotSupportedException("Symbols `::`, `(` and `)` are not supported yet!");
            }

            name.DictionaryName = dictionary.Name;
            name.Kind = KindOfName.Concept;
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
        }
    }
}
