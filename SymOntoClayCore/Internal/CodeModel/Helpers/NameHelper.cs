using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Helpers
{
    public static class NameHelper
    {
        public static string GetNewEntityNameString()
        {
            return $"#{Guid.NewGuid():D}";
        }

        public static StrongIdentifierValue CreateRuleOrFactName(IEntityDictionary dictionary)
        {
            return CreateEntityName(dictionary);
        }

        public static StrongIdentifierValue CreateEntityName(IEntityDictionary dictionary)
        {
            var text = GetNewEntityNameString();
            return CreateName(text, dictionary);
        }

        public static StrongIdentifierValue CreateName(string text, IEntityDictionary dictionary)
        {
            var name = new StrongIdentifierValue() { IsEmpty = false };

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
            else if (text.StartsWith("@@"))
            {
                name.KindOfName = KindOfName.SystemVar;
            }
            else if (text == "#@")
            {
                name.KindOfName = KindOfName.AnonymousEntityCondition;
            }
            else if (text.StartsWith("#@"))
            {
                name.KindOfName = KindOfName.EntityCondition;
            }
            else if (text.StartsWith("##"))
            {
                name.KindOfName = KindOfName.EntityRefByConcept;
            }
            else if (text.StartsWith("@"))
            {
                name.KindOfName = KindOfName.Var;
            }
            else if (text.StartsWith("#"))
            {
                name.KindOfName = KindOfName.Entity;
            }

            name.NameValue = text;

            return name;
        }
    }
}
