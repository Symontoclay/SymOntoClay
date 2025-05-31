using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper
{
    public static class StringBuilderHelper
    {
        public static char? Last(this StringBuilder sb)
        {
            var sbLength = sb.Length;

            if(sbLength == 0)
            {
                return null;
            }

            return sb[sbLength - 1];
        }
    }
}
