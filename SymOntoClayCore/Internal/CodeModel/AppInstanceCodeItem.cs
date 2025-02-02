using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Monitor.Common;
using System.Collections.Generic;
using System;
using System.Linq;
using SymOntoClay.Common.DebugHelpers;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class AppInstanceCodeItem : CodeItem
    {
        public AppInstanceCodeItem()
        {
            TypeOfAccess = TypeOfAccess.Private;
        }

        /// <inheritdoc/>
        public override bool IsAppInstanceCodeItem => true;

        /// <inheritdoc/>
        public override AppInstanceCodeItem AsAppInstanceCodeItem => this;

        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.AppInstance;

        public List<StrongIdentifierValue> RootTasks { get; set; } = new List<StrongIdentifierValue>();

        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public AppInstanceCodeItem Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public AppInstanceCodeItem Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (AppInstanceCodeItem)context[this];
            }

            var result = new AppInstanceCodeItem();
            context[this] = result;

            result.RootTasks = RootTasks?.Select(p => p.Clone(context))?.ToList() ?? new List<StrongIdentifierValue>();

            result.AppendCodeItem(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjListProp(n, nameof(RootTasks), RootTasks);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjListProp(n, nameof(RootTasks), RootTasks);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjListProp(n, nameof(RootTasks), RootTasks);

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
            return $"App instance code item: {Name.NameValue}";
        }
    }
}
