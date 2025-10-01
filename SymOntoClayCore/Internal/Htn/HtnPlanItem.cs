using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Text;

namespace SymOntoClay.Core.Internal.Htn
{
    public class HtnPlanItem: IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToDbgString, IObjectToHumanizedString, IMonitoredHumanizedObject
    {
        public BasePrimitiveHtnTask ExecutedTask { get; set; }
        public CompoundHtnTaskCase TaskCase { get; set; }

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

            sb.PrintObjProp(n, nameof(ExecutedTask), ExecutedTask);
            sb.PrintObjProp(n, nameof(TaskCase), TaskCase);

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

            sb.PrintShortObjProp(n, nameof(ExecutedTask), ExecutedTask);
            sb.PrintShortObjProp(n, nameof(TaskCase), TaskCase);

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

            sb.PrintBriefObjProp(n, nameof(ExecutedTask), ExecutedTask);
            sb.PrintBriefObjProp(n, nameof(TaskCase), TaskCase);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToDbgString()
        {
            return ToDbgString(0u);
        }

        /// <inheritdoc/>
        public string ToDbgString(uint n)
        {
            return this.GetDefaultToDbgStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToDbgString.PropertiesToDbgString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{NToHumanizedString()}");
            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToHumanizedString(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            return ToHumanizedString(DebugHelperOptions.FromHumanizedOptions(options));
        }

        /// <inheritdoc/>
        public string ToHumanizedString(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        /// <inheritdoc/>
        public string ToHumanizedLabel(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            return ToHumanizedLabel(DebugHelperOptions.FromHumanizedOptions(options));
        }

        /// <inheritdoc/>
        public string ToHumanizedLabel(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        /// <inheritdoc/>
        public string ToHumanizedString(IMonitorLogger logger)
        {
            return NToHumanizedString();
        }

        /// <inheritdoc/>
        public MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return new MonitoredHumanizedLabel()
            {
                Label = NToHumanizedString()
            };
        }

        private string NToHumanizedString()
        {
            return ExecutedTask?.ToHumanizedLabel();
        }
    }
}
