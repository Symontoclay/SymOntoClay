using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeExecution
{
    public class VoidCallResult : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public KindOfCallResult KindOfResult { get; set; }

        public IExecutable Executable { get; set; }
        public Exception SystemException { get; set; }
        public RuleInstance DslException { get; set; }

        public bool IsError => KindOfResult == KindOfCallResult.WasSystemException || KindOfResult == KindOfCallResult.WasDslException;

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
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfResult)} = {KindOfResult}");
            sb.PrintObjProp(n, nameof(Executable), Executable);
            sb.AppendLine($"{spaces}{nameof(SystemException)} = {SystemException}");
            sb.PrintObjProp(n, nameof(DslException), DslException);
            sb.AppendLine($"{spaces}{nameof(IsError)} = {IsError}");

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
            return PropertiesToShortString(n);
        }

        protected virtual string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfResult)} = {KindOfResult}");
            sb.PrintShortObjProp(n, nameof(Executable), Executable);
            sb.AppendLine($"{spaces}{nameof(SystemException)} = {SystemException}");
            sb.PrintShortObjProp(n, nameof(DslException), Executable);
            sb.AppendLine($"{spaces}{nameof(IsError)} = {IsError}");

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
            return PropertiesToBriefString(n);
        }

        protected virtual string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(KindOfResult)} = {KindOfResult}");
            sb.PrintBriefObjProp(n, nameof(Executable), Executable);
            sb.AppendLine($"{spaces}{nameof(SystemException)} = {SystemException}");
            sb.PrintBriefObjProp(n, nameof(DslException), DslException);
            sb.AppendLine($"{spaces}{nameof(IsError)} = {IsError}");

            return sb.ToString();
        }
    }
}
