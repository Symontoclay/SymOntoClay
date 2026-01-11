/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.Instances;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class RuleInstanceReference: Value
    {
        public RuleInstanceReference(RuleInstance originalRuleInstance)
        {
            CurrentRuleInstance = originalRuleInstance;
        }

        private RuleInstanceReference()
        {
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.RuleInstanceReference;

        /// <inheritdoc/>
        public override bool IsRuleInstanceReference => true;

        /// <inheritdoc/>
        public override RuleInstanceReference AsRuleInstanceReference => this;

        public RuleInstance CurrentRuleInstance { get; private set; }

        private bool _isCloned;
        private object _isClonedLockObj = new object();

        public void TryCloneCurrentRuleInstance()
        {
            lock(_isClonedLockObj)
            {
                if(_isCloned)
                {
                    return;
                }

                _isCloned = true;

                CurrentRuleInstance = CurrentRuleInstance?.Clone();
            }
        }

        /// <inheritdoc/>
        public override IMember GetMember(IMonitorLogger logger, StrongIdentifierValue memberName)
        {
#if DEBUG
            //logger.Info("7E8BD5EE-C5C9-4306-8AB0-0DC14AE622EA", $"memberName = {memberName}");
#endif

            var normalizedMemberName = memberName.NormalizedNameValue;

            switch(normalizedMemberName)
            {
                case "o":
                    return new ObligationModalityMember(this);

                case "so":
                    return new SelfObligationModalityMember(this);

                default:
                    throw new ArgumentOutOfRangeException(nameof(normalizedMemberName), normalizedMemberName, null);
            }
        }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            throw new NotImplementedException("4835EE9A-329B-4F7E-A5FA-F8FC2C00013C");
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            throw new NotImplementedException("AE2BF91D-686D-4A8E-B224-D8C4A8E028E3");
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public RuleInstanceReference Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public RuleInstanceReference Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (RuleInstanceReference)context[this];
            }

            var result = new RuleInstanceReference();
            context[this] = result;

            result.CurrentRuleInstance = CurrentRuleInstance?.Clone(context);
            result._isCloned = _isCloned;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(CurrentRuleInstance), CurrentRuleInstance);
            sb.AppendLine($"{spaces}{nameof(_isCloned)} = {_isCloned}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(CurrentRuleInstance), CurrentRuleInstance);
            sb.AppendLine($"{spaces}{nameof(_isCloned)} = {_isCloned}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(CurrentRuleInstance), CurrentRuleInstance);
            sb.AppendLine($"{spaces}{nameof(_isCloned)} = {_isCloned}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return CurrentRuleInstance?.ToHumanizedLabel(options);
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return CurrentRuleInstance?.ToHumanizedString(options);
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return CurrentRuleInstance?.ToLabel(logger);
        }
    }
}
