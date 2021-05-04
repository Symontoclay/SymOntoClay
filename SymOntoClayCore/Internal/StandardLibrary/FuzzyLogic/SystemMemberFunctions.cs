using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.FuzzyLogic
{
    public static class SystemMemberFunctions
    {
        public static double Trapezoid(double x, double a, double b, double c, double d)
        {
            if (x <= a || x >= d)
            {
                return 0;
            }

            if (x > a && x < b)
            {
                return (x - a) / (b - a);
            }

            if (x >= b && x <= c)
            {
                return 1;
            }

            if (x > c && x <= d)
            {
                return (d - x) / (d - c);
            }

            throw new NotImplementedException();
        }

        public static double SFunction(double x, double a, double m, double b)
        {
            if (x <= a)
            {
                return 0;
            }

            if (x > a && x <= m)
            {
                return 2 * (Math.Pow((x - a) / (b - a), 2));
            }

            if (x > m && x <= b)
            {
                return 1 - (2 * (Math.Pow((x - b) / (b - a), 2)));
            }

            return 1;
        }

        public static double LFunction(double x, double a, double b)
        {
            if (x <= a)
            {
                return 1;
            }

            if (x > a && x <= b)
            {
                return (b - x) / (b - a);
            }

            return 0;
        }
    }
}
