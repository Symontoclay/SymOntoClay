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

using NLog;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class MutablePartOfRuleInstanceValue : Value
    {
        public MutablePartOfRuleInstanceValue(MutablePartOfRuleInstance mutablePartOfRuleInstance)
        {
            MutablePartOfRuleInstance = mutablePartOfRuleInstance;
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.MutablePartOfRuleInstance;

        /// <inheritdoc/>
        public override bool IsMutablePartOfRuleInstanceValue => true;

        /// <inheritdoc/>
        public override MutablePartOfRuleInstanceValue AsMutablePartOfRuleInstanceValue => this;

        public MutablePartOfRuleInstance MutablePartOfRuleInstance { get; private set; }

        private List<StrongIdentifierValue> _builtInSuperTypes;

        /// <inheritdoc/>
        public override IReadOnlyList<StrongIdentifierValue> BuiltInSuperTypes => _builtInSuperTypes;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return MutablePartOfRuleInstance;
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override void SetPropertyValue(StrongIdentifierValue propertyName, Value value)
        {
            var propertyNameStr = propertyName.NormalizedNameValue;

            switch(propertyNameStr)
            {
                case "o":
                    MutablePartOfRuleInstance.ObligationModality = value;
                    break;

                case "so":
                    MutablePartOfRuleInstance.SelfObligationModality = value;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(propertyNameStr), propertyNameStr, null);
            }
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            _builtInSuperTypes = new List<StrongIdentifierValue>() { NameHelper.CreateName(StandardNamesConstants.FactTypeName) };

            return base.CalculateLongHashCode(options);
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

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public MutablePartOfRuleInstanceValue Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public MutablePartOfRuleInstanceValue Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (MutablePartOfRuleInstanceValue)context[this];
            }

            var result = new MutablePartOfRuleInstanceValue(MutablePartOfRuleInstance.Clone(context));
            context[this] = result;

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(MutablePartOfRuleInstance), MutablePartOfRuleInstance);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(MutablePartOfRuleInstance), MutablePartOfRuleInstance);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjProp(n, nameof(MutablePartOfRuleInstance), MutablePartOfRuleInstance);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            return $"{spaces}{DebugHelperForRuleInstance.ToString(MutablePartOfRuleInstance.Parent, MutablePartOfRuleInstance)}";
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            var opt = options.Clone();
            opt.MutablePartOfRuleInstance = MutablePartOfRuleInstance;

            return DebugHelperForRuleInstance.ToString(MutablePartOfRuleInstance.Parent, opt);
        }
    }
}
