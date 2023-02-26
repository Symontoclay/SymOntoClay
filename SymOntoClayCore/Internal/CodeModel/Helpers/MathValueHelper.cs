/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Helpers
{
    public static class MathValueHelper
    {
        public static NumberValue Max(IEnumerable<NumberValue> collection)
        {
            if (collection.Count() == 1)
            {
                return collection.Single();
            }

            if (collection.All(p => !p.SystemValue.HasValue))
            {
                return collection.First();
            }

            var notNullCollection = collection.Where(p => p.SystemValue.HasValue);

            if (notNullCollection.Count() == 1)
            {
                return notNullCollection.Single();
            }

            var maxSystemValue = notNullCollection.Select(p => p.SystemValue).Max();

            return notNullCollection.First(p => p.SystemValue == maxSystemValue);
        }
    }
}
