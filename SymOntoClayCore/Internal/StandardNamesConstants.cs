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

using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal
{
    public static class StandardNamesConstants
    {
        #region names of types
        public const string UniversalTypeName = "⊤";
        public const string NullTypeName = "null";
        public const string WorldTypeName = "world";
        public const string AppTypeName = "app";
        public const string ClassTypeName = "class";
        public const string ActionTypeName = "action";
        public const string StateTypeName = "state";
        public const string NumberTypeName = "number";
        public const string StringTypeName = "string";
        public const string FuzzyTypeName = "fuzzy";
        public const string FactTypeName = "fact";
        public const string WaypointTypeName = "waypoint";
        public const string ConditionalEntityTypeName = "conditional entity";
        public const string EntityTypeName = "entity";
        public const string SequenceTypeName = "sequence";
        public const string SelfSystemVarName = "@@self";
        public const string HostSystemVarName = "@@host";
        public const string RandomConstraintName = "random";
        public const string NearestConstraintName = "nearest";
        public const string DefaultCtorName = "__ctor";
        public const string TimeoutAttributeName = "timeout";
        public const string PriorityAttributeName = "priority";
        #endregion
    }
}
