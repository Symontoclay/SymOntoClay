using SymOntoClay.Core.Internal.CodeModel;

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
