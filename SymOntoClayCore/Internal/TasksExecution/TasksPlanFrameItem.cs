using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.TasksExecution
{
    public class TasksPlanFrameItem : IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToDbgString, IObjectToHumanizedString, IMonitoredHumanizedObject
    {
        public KindOfTasksPlanFrameItemCommand KindOfCommand { get; set; }
        public int TargetPosition { get; set; }
        public BasePrimitiveTask ExecutedTask { get; set; }
        public BaseCompoundTask CompoundTask { get; set; }

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

            sb.AppendLine($"{spaces}{nameof(KindOfCommand)} = {KindOfCommand}");
            sb.AppendLine($"{spaces}{nameof(TargetPosition)} = {TargetPosition}");
            sb.PrintObjProp(n, nameof(ExecutedTask), ExecutedTask);
            sb.PrintObjProp(n, nameof(CompoundTask), CompoundTask);

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

            sb.AppendLine($"{spaces}{nameof(KindOfCommand)} = {KindOfCommand}");
            sb.AppendLine($"{spaces}{nameof(TargetPosition)} = {TargetPosition}");
            sb.PrintShortObjProp(n, nameof(ExecutedTask), ExecutedTask);
            sb.PrintShortObjProp(n, nameof(CompoundTask), CompoundTask);

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

            sb.AppendLine($"{spaces}{nameof(KindOfCommand)} = {KindOfCommand}");
            sb.AppendLine($"{spaces}{nameof(TargetPosition)} = {TargetPosition}");
            sb.PrintBriefObjProp(n, nameof(ExecutedTask), ExecutedTask);
            sb.PrintBriefObjProp(n, nameof(CompoundTask), CompoundTask);

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
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return ToHumanizedString(opt);
        }

        /// <inheritdoc/>
        public string ToHumanizedString(DebugHelperOptions options)
        {
            throw new NotImplementedException("BC75B7FE-C68B-44B9-9E37-92ED37D78F4B");
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
            switch (KindOfCommand)
            {
                case KindOfTasksPlanFrameItemCommand.Nop:
                    return "NOP";

                case KindOfTasksPlanFrameItemCommand.BeginCompoundTask:
                    return $"Begin: {CompoundTask?.ToHumanizedLabel()}";

                case KindOfTasksPlanFrameItemCommand.ExecPrimitiveTask:
                    return $"Exec: {ExecutedTask?.ToHumanizedLabel()}";

                case KindOfTasksPlanFrameItemCommand.EndCompoundTask:
                    return $"End: {CompoundTask?.ToHumanizedLabel()}";

                case KindOfTasksPlanFrameItemCommand.JumpTo:
                    return $"Jump To: {TargetPosition}";

                default:
                    throw new ArgumentOutOfRangeException(nameof(KindOfCommand), KindOfCommand, null);
            }
        }
    }
}
