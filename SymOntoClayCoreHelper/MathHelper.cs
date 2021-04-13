using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper
{
    public static class MathHelper
    {
        public static double TrapezoidIntegral(double a, double b, int n, Func<double, double> fun)
        {
            var width = (b - a) / n;

            double result = 0;
            for (var step = 0; step < n; step++)
            {
                var x1 = a + step * width;
                var x2 = a + (step + 1) * width;

                result += 0.5 * (x2 - x1) * (fun(x1) + fun(x2));
            }

            return result;
        }
    }
}
