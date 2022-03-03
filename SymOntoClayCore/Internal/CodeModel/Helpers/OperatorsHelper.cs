/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Helpers
{
    public static class OperatorsHelper
    {
        //I use operator precedence of C and C++. https://en.wikipedia.org/wiki/Operators_in_C_and_C%2B%2B
        public static int GetPriority(KindOfOperator kind)
        {
            switch(kind)
            {
                case KindOfOperator.Assign:
                    return 17;

                case KindOfOperator.LeftRightStream:
                    return 16;

                case KindOfOperator.And:                
                    return 14;

                case KindOfOperator.Or:
                    return 15;

                case KindOfOperator.Not:
                    return 3;

                case KindOfOperator.Is:
                    return 10;

                case KindOfOperator.IsNot:
                    return 10;

                case KindOfOperator.More:
                case KindOfOperator.MoreOrEqual:
                case KindOfOperator.Less:
                case KindOfOperator.LessOrEqual:
                    return 9;

                case KindOfOperator.Add:
                case KindOfOperator.Sub:
                    return 6;

                case KindOfOperator.Point:
                    return 2;

                case KindOfOperator.CallFunction:
                case KindOfOperator.Predicate:
                case KindOfOperator.CallLogicalQuery:
                    return 2;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        public static string GetSymbol(KindOfOperator kind)
        {
            switch (kind)
            {
                case KindOfOperator.Assign:
                    return "=";

                case KindOfOperator.LeftRightStream:
                    return ">>";

                case KindOfOperator.And:
                    return "AND";

                case KindOfOperator.Is:
                    return "IS";

                case KindOfOperator.IsNot:
                    return "IS NOT";

                case KindOfOperator.Point:
                    return ".";

                case KindOfOperator.CallFunction:
                    return "()";

                case KindOfOperator.CallLogicalQuery:
                    return "CALL";

                case KindOfOperator.More:
                    return ">";

                case KindOfOperator.MoreOrEqual:
                    return ">=";

                case KindOfOperator.Less:
                    return "<";

                case KindOfOperator.LessOrEqual:
                    return "<=";

                case KindOfOperator.Add:
                    return "+";

                case KindOfOperator.Sub:
                    return "-";

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}
