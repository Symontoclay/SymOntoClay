/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

        public static string ConvertNameToId(string source)
        {
            if(string.IsNullOrWhiteSpace(source))
            {
                return string.Empty;
            }

            if(source.StartsWith("#"))
            {
                if (source.StartsWith("#`"))
                {
                    return source;
                }

                return $"{source.Insert(1, "`")}`";
            }

            return source;
        }

        public static StrongIdentifierValue CreateRuleOrFactName()
        {
            return CreateEntityName();
        }

        public static StrongIdentifierValue CreateEntityName()
        {
            var text = GetNewEntityNameString();
            return CreateName(text);
        }

        public static StrongIdentifierValue CreateName(string text)
        {
            if(string.IsNullOrWhiteSpace(text))
            {
                return new StrongIdentifierValue() { IsEmpty = true };
            }

            var name = new StrongIdentifierValue() { IsEmpty = false };

            text = text.ToLower().Trim();

            if (text.Contains("::") || text.Contains("("))
            {
                throw new NotSupportedException("Symbols `::`, `(` and `)` are not supported yet!");
            }

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
            else if (text.StartsWith("$"))
            {
                name.KindOfName = KindOfName.LogicalVar;
            }

            name.NameValue = text;

            if(text.Contains(" ") && !text.Contains("`"))
            {
                var kindOfName = name.KindOfName;

                switch(kindOfName)
                {
                    case KindOfName.Concept:
                        name.NameValue = $"`{name.NameValue}`";
                        break;

                    case KindOfName.Entity:
                        name.NameValue = $"#`{name.NameValue.Replace("#", string.Empty)}`";
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, null);
                }
            }

            name.NormalizedNameValue = NormalizeString(text);

            return name;
        }

        public static string NormalizeString(string value)
        {
            return value.ToLower().Replace("`", string.Empty).Trim();
        }
    }
}
