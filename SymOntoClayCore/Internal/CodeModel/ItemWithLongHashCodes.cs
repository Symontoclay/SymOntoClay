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
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class ItemWithLongHashCodes: IObjectWithLongHashCodes
    {
        private bool _isDirty = true;

        public void CheckDirty(CheckDirtyOptions options = null)
        {
            if (_isDirty)
            {
                CalculateLongHashCodes(options);
                _isDirty = false;
            }
        }

        protected ulong? _longConditionalHashCode;

        /// <inheritdoc/>
        public ulong GetLongConditionalHashCode()
        {
            return GetLongConditionalHashCode(null);
        }

        public virtual ulong GetLongConditionalHashCode(CheckDirtyOptions options)
        {
            if (!_longConditionalHashCode.HasValue)
            {
                CalculateLongHashCodes(options);
            }

            return _longConditionalHashCode.Value;
        }

        protected ulong? _longHashCode;

        /// <inheritdoc/>
        public ulong GetLongHashCode()
        {
            return GetLongHashCode(null);
        }

        public virtual ulong GetLongHashCode(CheckDirtyOptions options)
        {
            if (!_longHashCode.HasValue)
            {
                CalculateLongHashCodes(options);
            }

            return _longHashCode.Value;
        }

        public virtual void CalculateLongHashCodes(CheckDirtyOptions options)
        {
            _longHashCode = CalculateLongHashCode(options);
            _longConditionalHashCode = CalculateLongConditionalHashCode(options);
            _isDirty = false;
        }

        protected virtual ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            return 0u;
        }

        protected virtual ulong CalculateLongConditionalHashCode(CheckDirtyOptions options)
        {
            return 0u;
        }
    }
}
