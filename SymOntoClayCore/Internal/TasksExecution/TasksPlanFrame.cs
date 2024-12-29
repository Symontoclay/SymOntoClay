using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.TasksExecution
{
    public class TasksPlanFrame : IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToDbgString, IObjectToHumanizedString, IMonitoredHumanizedObject
    {
        public int CurrentPosition { get; set; }
        public Dictionary<int, TasksPlanItem> Items { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(CurrentPosition)} = {CurrentPosition}");
            sb.PrintObjDict_2_Prop(n, nameof(Items), Items);
            //sb.PrintObjListProp(n, nameof(Items), Items);

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

            sb.AppendLine($"{spaces}{nameof(CurrentPosition)} = {CurrentPosition}");
            sb.PrintShortObjDict_2_Prop(n, nameof(Items), Items);

            //sb.PrintShortObjListProp(n, nameof(Items), Items);

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

            sb.AppendLine($"{spaces}{nameof(CurrentPosition)} = {CurrentPosition}");
            sb.PrintBriefObjDict_2_Prop(n, nameof(Items), Items);

            //sb.PrintBriefObjListProp(n, nameof(Items), Items);

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

            var nextN = n + DisplayHelper.IndentationStep;

            var nextNSpaces = DisplayHelper.Spaces(nextN);

            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}Begin Items");

            foreach(var item in Items)
            {
                var currentMark = string.Empty;

                if (item.Key == CurrentPosition)
                {
                    currentMark = "-> ";
                }

                sb.AppendLine($"{nextNSpaces}{currentMark}{item.Key}: {item.Value.ToDbgString()}");
            }

            sb.AppendLine($"{spaces}End Items");

            //sb.AppendLine($"{spaces}Tasks to execution:");
            //sb.AppendLine($"{nextNSpaces}[{string.Join(", ", Items.Select(p => p.ExecutedTask.Name.ToSystemString()))}]");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToHumanizedString(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return ToHumanizedString(opt);
        }

        /// <inheritdoc/>
        public string ToHumanizedString(DebugHelperOptions options)
        {
            throw new NotImplementedException("E47BC4FD-1AF6-43CA-9914-4EDDA10546BB");
        }

        /// <inheritdoc/>
        public string ToHumanizedLabel(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return ToHumanizedLabel(opt);
        }

        /// <inheritdoc/>
        public string ToHumanizedLabel(DebugHelperOptions options)
        {
            throw new NotImplementedException("049DC608-D1E5-4370-9E70-722C3983F2AE");
        }

        /// <inheritdoc/>
        public string ToHumanizedString(IMonitorLogger logger)
        {
            throw new NotImplementedException("3A21267A-3900-4D75-9055-C2D13FC25365");
        }

        /// <inheritdoc/>
        public MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            throw new NotImplementedException("78AABFFD-D82A-4027-ABFC-004151A5BDEE");
        }
    }
}
