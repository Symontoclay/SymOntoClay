using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class SimpleName : BaseLoggedComponent, IEquatable<SimpleName>, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public SimpleName(string text, string nspace, ICodeModelContext context)
            : base(context.Logger)
        {
            throw new NotSupportedException("Namespaces are not supported yet!");

            _context = context;
        }

        public SimpleName(string text, ICodeModelContext context)
            : base(context.Logger)
        {
            _context = context;

            NameValue = text;
            FullNameValue = text;

            CalculateIndex();
        }

        private readonly ICodeModelContext _context;

        public string NameValue { get; private set; }
        public string FullNameValue { get; private set; }

        public ulong FullNameKey { get; private set; }

        public void CalculateIndex()
        {
            var dictionary = _context.Dictionary;

            FullNameKey = dictionary.GetKey(FullNameValue);
        }

        public bool Equals(SimpleName other)
        {
            if (other == null)
            {
                return false;
            }

            return NameValue == other.NameValue && FullNameValue == other.FullNameValue && FullNameKey == other.FullNameKey;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var personObj = obj as SimpleName;
            if (personObj == null)
            {
                return false;
            }

            return Equals(personObj);
        }

        public override int GetHashCode()
        {
            return FullNameKey.GetHashCode();
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
            sb.AppendLine($"{spaces}{nameof(NameValue)} = {NameValue}");
            sb.AppendLine($"{spaces}{nameof(FullNameValue)} = {FullNameValue}");
            sb.AppendLine($"{spaces}{nameof(FullNameKey)} = {FullNameKey}");
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
            sb.AppendLine($"{spaces}{nameof(NameValue)} = {NameValue}");
            sb.AppendLine($"{spaces}{nameof(FullNameValue)} = {FullNameValue}");
            sb.AppendLine($"{spaces}{nameof(FullNameKey)} = {FullNameKey}");
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
            sb.AppendLine($"{spaces}{nameof(NameValue)} = {NameValue}");
            sb.AppendLine($"{spaces}{nameof(FullNameValue)} = {FullNameValue}");
            sb.AppendLine($"{spaces}{nameof(FullNameKey)} = {FullNameKey}");
            return sb.ToString();
        }
    }
}
