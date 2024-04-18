/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
