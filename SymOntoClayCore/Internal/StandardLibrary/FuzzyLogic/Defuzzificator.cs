using NLog;
using SymOntoClay.CoreHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.FuzzyLogic
{
    public static class Defuzzificator
    {
        public static double Defuzzificate(KindOfDefuzzification kindOfDefuzzification, double a, double b, Func<double, double> fun)
        {
            switch(kindOfDefuzzification)
            {
                case KindOfDefuzzification.CoG:
                    return DefuzzificateByCoG(a, b, fun);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfDefuzzification), kindOfDefuzzification, null);
            }
        }

        private static double DefuzzificateByCoG(double a, double b, Func<double, double> fun)
        {
            var n = 100;

            var result = MathHelper.TrapezoidIntegral(a, b, n, (double x) => x * fun(x)) / MathHelper.TrapezoidIntegral(a, b, n, (double x) => fun(x));

            return result;
        }
    }
}
