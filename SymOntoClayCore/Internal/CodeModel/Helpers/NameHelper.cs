using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Helpers
{
    public static class NameHelper
    {
        public static Name CreateRuleOrFactName(IEntityDictionary dictionary)
        {
            return CreateEntityName(dictionary);
        }

        public static Name CreateEntityName(IEntityDictionary dictionary)
        {
            var text = $"#{Guid.NewGuid():D}";
            return CreateName(text, dictionary);
        }

        public static Name CreateName(string text, IEntityDictionary dictionary)
        {
            var name = new Name() { IsEmpty = false };

            text = text.ToLower().Trim();

            if (text.Contains("::") || text.Contains("("))
            {
                throw new NotSupportedException("Symbols `::`, `(` and `)` are not supported yet!");
            }

            name.DictionaryName = dictionary.Name;
            name.KindOfName = KindOfName.Concept;

            if(text.StartsWith("@>"))
            {
                name.KindOfName = KindOfName.Channel;
            }
            else
            {
                if(text.StartsWith("#"))
                {
                    name.KindOfName = KindOfName.Entity;
                }
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
        }
    }
}
