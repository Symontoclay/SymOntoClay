using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class RangeBoundary: IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public NumberValue Value { get; set; }
        public bool Includes { get; set; }

        private bool _isDirty = true;

        public void CheckDirty()
        {
            if (_isDirty)
            {
                CalculateLongHashCodes();
                _isDirty = false;
            }
        }

        private ulong? _longHashCode;

        public ulong GetLongHashCode()
        {
            if (!_longHashCode.HasValue)
            {
                CalculateLongHashCodes();
            }

            return _longHashCode.Value;
        }

        public void CalculateLongHashCodes()
        {
            Value.CheckDirty();

            _longHashCode = Value.GetLongHashCode() ^ (ulong)Includes.GetHashCode();

            _isDirty = false;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Value), Value);
            sb.AppendLine($"{spaces}{nameof(Includes)} = {Includes}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(Value), Value);
            sb.AppendLine($"{spaces}{nameof(Includes)} = {Includes}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Value), Value);
            sb.AppendLine($"{spaces}{nameof(Includes)} = {Includes}");

            return sb.ToString();
        }
    }
}
