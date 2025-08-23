using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.NLP.Internal.Helpers
{
    public static class NlpStringHelper
    {
        public static string PrepareString(StrongIdentifierValue name)
        {
            return PrepareString(name.NameValue);
        }

        public static string PrepareString(string name)
        {
            return name.Replace("`", string.Empty).Trim();
        }
    }
}
