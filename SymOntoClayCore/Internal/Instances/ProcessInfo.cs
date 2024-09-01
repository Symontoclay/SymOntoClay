/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Threading;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.Core.Internal.Instances
{
    public class ProcessInfo: BaseProcessInfo
    {
        public ProcessInfo(CancellationToken cancellationToken, ICustomThreadPool threadPool)
            : base(cancellationToken, threadPool)
        {
        }

        /// <inheritdoc/>
        public override string EndPointName => string.Empty;

        public CodeFrame CodeFrame { get; set; }

        /// <inheritdoc/>
        public override IReadOnlyList<int> Devices => _devices;

        /// <inheritdoc/>
        public override IReadOnlyList<string> Friends => _friends;

        /// <inheritdoc/>
        public override bool IsFriend(IMonitorLogger logger, IProcessInfo other)
        {
            return false;
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            base.OnDisposed();
        }

        #region private fields
        private readonly List<int> _devices = new List<int>();
        private readonly List<string> _friends = new List<string>();
        #endregion

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(CodeFrame), CodeFrame);

            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjProp(n, nameof(CodeFrame), CodeFrame);

            sb.Append(base.PropertiesToShortString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintExisting(n, nameof(CodeFrame), CodeFrame);

            sb.Append(base.PropertiesToBriefString(n));

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            var sb = new StringBuilder($"proc: {Id} ({Status})");

            var metadata = CodeFrame?.Metadata;

            if (metadata != null)
            {
                sb.Append($" {metadata.ToHumanizedString(options)}");
            }

            var callMethodId = CodeFrame?.CallMethodId;

            if (!string.IsNullOrWhiteSpace(callMethodId))
            {
                sb.Append($" <CallMethodId: {callMethodId}>");
            }

            var arguments = CodeFrame?.Arguments;

            if(!arguments.IsNullOrEmpty())
            {
                var argsStrList = new List<string>();

                foreach (var arg in arguments) 
                {
                    argsStrList.Add($"{arg.Key.ToHumanizedLabel(options)} = {arg.Value.ToHumanizedLabel(options)}");
                }

                sb.Append($" <{string.Join(", ", argsStrList)}>");
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            var sb = new StringBuilder($"proc: {Id} ({Status})");

            var metadata = CodeFrame?.Metadata;

            if (metadata != null)
            {
                sb.Append($" {metadata.ToHumanizedLabel(options)}");
            }

            var callMethodId = CodeFrame?.CallMethodId;

            if(!string.IsNullOrWhiteSpace(callMethodId))
            {
                sb.Append($" <CallMethodId: {callMethodId}>");
            }

            var arguments = CodeFrame?.Arguments;

            if (!arguments.IsNullOrEmpty())
            {
                var argsStrList = new List<string>();

                foreach (var arg in arguments)
                {
                    argsStrList.Add($"{arg.Key.ToHumanizedLabel(options)} = {arg.Value.ToHumanizedLabel(options)}");
                }

                sb.Append($" <{string.Join(", ", argsStrList)}>");
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(IMonitorLogger logger)
        {
            return ToHumanizedString();
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
#if DEBUG
            //logger.Info("3814F1B0-CBF4-42B0-926F-543960BF81BE", $"CodeFrame?.Metadata.ToLabel(logger) = {CodeFrame?.Metadata.ToLabel(logger)}");
#endif

            var result = new MonitoredHumanizedLabel();

            var sb = new StringBuilder($"proc: {Id} ({Status})");

            result.CallMethodId = CodeFrame?.CallMethodId;

            var metadata = CodeFrame?.Metadata;

            if(metadata != null)
            {
                var metadataLabel = metadata.ToLabel(logger);

                result.KindOfCodeItemDescriptor = metadataLabel.KindOfCodeItemDescriptor;

                var metadataLabelStr = metadataLabel.Label;

                if (!string.IsNullOrWhiteSpace(metadataLabelStr))
                {
                    sb.Append($" {metadataLabelStr}");
                }

                result.Signatures = metadataLabel.Signatures;
                metadataLabel.Values = metadataLabel.Values;
            }

            var arguments = CodeFrame?.Arguments;

#if DEBUG
            //logger.Info("FB3E7263-9699-4D49-8140-D84B5B814F71", $"arguments = {arguments.WriteDict_1_ToString()}");
#endif

            if (arguments != null)
            {
                var values = new List<MonitoredHumanizedMethodParameterValue>();

                foreach (var argument in arguments)
                {
                    var item = new MonitoredHumanizedMethodParameterValue();
                    item.NameHumanizedStr = argument.Key.ToHumanizedLabel();
                    item.ValueHumanizedStr = argument.Value.ToHumanizedLabel();

                    values.Add(item);
                }

                result.Values = values;
            }

            result.Label = sb.ToString();

            return result;
        }
    }
}
