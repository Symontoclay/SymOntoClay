using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.SerializationGenerator
{
    public static class GeneratorsHelper
    {
        public static string Spaces(int n)
        {
            if (n == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            for (var i = 0; i < n; i++)
            {
                sb.Append(" ");
            }

            return sb.ToString();
        }
    }
}
