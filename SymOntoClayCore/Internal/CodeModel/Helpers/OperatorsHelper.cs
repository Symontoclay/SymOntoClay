/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
                    return 16;

                case KindOfOperator.LeftRightStream:
                    return 15;

                case KindOfOperator.And:
                    return 14;

                case KindOfOperator.Is:
                    return 10;

                case KindOfOperator.IsNot:
                    return 10;

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

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}
