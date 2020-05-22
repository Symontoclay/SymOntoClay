using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class Name: BaseLoggedComponent, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public Name(string text, List<string> targetNamespaces, ICodeModelContext context)
            : base(context.Logger)
        {
            _context = context;

            if(text.Contains("::") || text.Contains("("))
            {
                throw new NotSupportedException("Symbols `::`, `(` and `)` are not supported yet!");
            }

            NameValue = text;

            CalculateIndex();
        }

        private readonly ICodeModelContext _context;

        public string NameValue { get; private set; }
        public ulong NameKey { get; private set; }

        public IList<Namespace> Namespaces { get; private set; } = new List<Namespace>();

        public IList<ulong> FullNameKeys { get; private set; } = new List<ulong>();

        public void CalculateIndex()
        {
            var dictionary = _context.Dictionary;

            NameKey = dictionary.GetKey(NameValue);

            if(Namespaces.IsNullOrEmpty())
            {
                FullNameKeys = new List<ulong>() { NameKey };
            }
            else
            {
                throw new NotSupportedException("Namespaces are not supported yet!");
            }
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
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(NameValue)} = {NameValue}");
            sb.AppendLine($"{spaces}{nameof(NameKey)} = {NameKey}");

            sb.PrintObjListProp(n, nameof(Namespaces), Namespaces);

            sb.PrintPODList(n, nameof(FullNameKeys), FullNameKeys);

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
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(NameValue)} = {NameValue}");
            sb.AppendLine($"{spaces}{nameof(NameKey)} = {NameKey}");

            sb.PrintShortObjListProp(n, nameof(Namespaces), Namespaces);

            sb.PrintPODList(n, nameof(FullNameKeys), FullNameKeys);

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
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(NameValue)} = {NameValue}");
            sb.AppendLine($"{spaces}{nameof(NameKey)} = {NameKey}");

            sb.PrintBriefObjListProp(n, nameof(Namespaces), Namespaces);

            sb.PrintPODList(n, nameof(FullNameKeys), FullNameKeys);

            return sb.ToString();
        }
    }
}
