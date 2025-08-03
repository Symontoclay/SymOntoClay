using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class MemberValue : Value
    {
        public MemberValue(IMember member)
        {
            Member = member;
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.MemberValue;

        /// <inheritdoc/>
        public override bool IsMemberValue => true;

        /// <inheritdoc/>
        public override MemberValue AsMemberValue => this;

        public IMember Member { get; private set; }

        public KindOfMember KindOfMember => Member.KindOfMember;
        public CallResult GetValue(IMonitorLogger logger) => Member.GetValue(logger);

        /// <inheritdoc/>
        public override bool IsHostMethodValue => Member.IsHostMethodValue;

        /// <inheritdoc/>
        public override HostMethodValue AsHostMethodValue => Member.AsHostMethodValue;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return Member;
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            throw new NotImplementedException("C54D0622-A5DD-4826-8F6C-43EEFFB377DC");
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            return base.CalculateLongHashCode(options) ^ (ulong)Math.Abs(Member.GetHashCode());
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneValue(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (Value)context[this];
            }

            var result = new MemberValue(Member);
            context[this] = result;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Member), Member);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(Member), Member);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(Member), Member);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}{ToHumanizedString()}";
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return Member.ToHumanizedString(options);
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return Member.ToHumanizedLabel(options);
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return Member.ToLabel(logger);
        }
    }
}
