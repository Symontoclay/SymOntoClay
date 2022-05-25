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
