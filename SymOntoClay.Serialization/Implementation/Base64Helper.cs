using System.Text;
using System;

namespace SymOntoClay.Serialization.Implementation
{
    public static class Base64Helper
    {
        public static string ToBase64String(string input)
        {
            var base64Array = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(base64Array);
        }

        public static string FromBase64String(string input)
        {
            var base64Array = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(base64Array);
        }
    }
}
