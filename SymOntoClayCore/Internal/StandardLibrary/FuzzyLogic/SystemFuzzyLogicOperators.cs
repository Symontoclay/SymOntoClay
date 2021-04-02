using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.FuzzyLogic
{
    public static class SystemFuzzyLogicOperators
    {
        public static double Not(double x)
        {
            return 1 - x;
        }

        public static double Very(double x)
        {
            return Math.Pow(x, 2);
        }
    }
}
