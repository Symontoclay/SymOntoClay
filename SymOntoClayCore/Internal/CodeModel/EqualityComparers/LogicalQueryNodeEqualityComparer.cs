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

using SymOntoClay.Core.Internal.DataResolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.EqualityComparers
{
    public class LogicalQueryNodeEqualityComparer : IEqualityComparer<LogicalQueryNode>
    {
        /// <inheritdoc/>
        public bool Equals(LogicalQueryNode x, LogicalQueryNode y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null))
            {
                return false;
            }

            if (ReferenceEquals(y, null))
            {
                return false;
            }

            if(x.Kind != y.Kind)
            {
                return false;
            }

            var kind = x.Kind;

            switch (kind)
            {
                case KindOfLogicalQueryNode.Value:
                    return x.Value.Equals(y.Value);

                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.Entity:
                    return x.Name == y.Name;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }

        /// <inheritdoc/>
        public int GetHashCode(LogicalQueryNode obj)
        {
            var kind = obj.Kind;

            switch(kind)
            {
                case KindOfLogicalQueryNode.Concept:
                case KindOfLogicalQueryNode.Entity:
                    return kind.GetHashCode() ^ obj.Name.GetHashCode();

                case KindOfLogicalQueryNode.Value:
                    {
                        var systemValue = obj.Value.GetSystemValue();

                        if (systemValue == null)
                        {
                            return kind.GetHashCode();
                        }

                        return kind.GetHashCode() ^ systemValue.GetHashCode();
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}
