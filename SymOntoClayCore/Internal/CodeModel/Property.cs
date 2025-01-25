using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class Property : CodeItem
    {
        public Property()
        {
            TypeOfAccess = DefaultTypeOfAccess;
        }

        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.Property;

        /// <inheritdoc/>
        public override bool IsProperty => true;

        /// <inheritdoc/>
        public override Property AsProperty => this;

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public Property Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public Property Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (Property)context[this];
            }

            var result = new Property();
            context[this] = result;

            result.AppendCodeItem(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            //sb.PrintObjProp(n, nameof(Value), Value);
            //sb.PrintObjListProp(n, nameof(TypesList), TypesList);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            //sb.PrintShortObjProp(n, nameof(Value), Value);
            //sb.PrintShortObjListProp(n, nameof(TypesList), TypesList);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            //sb.PrintBriefObjProp(n, nameof(Value), Value);
            //sb.PrintBriefObjListProp(n, nameof(TypesList), TypesList);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return new MonitoredHumanizedLabel()
            {
                Label = NToHumanizedString()
            };
        }

        private string NToHumanizedString()
        {
            return $"prop {Name.NameValue}";
            //return $"{Name.NameValue}: {TypesListToHumanizedString()}";
        }
    }
}
