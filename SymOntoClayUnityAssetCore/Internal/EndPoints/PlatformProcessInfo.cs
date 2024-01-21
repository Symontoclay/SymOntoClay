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

using Newtonsoft.Json;
using NLog;
using SymOntoClay.Core;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Internal.EndPoints
{
    public class PlatformProcessInfo : BaseProcessInfo
    {
        public PlatformProcessInfo(CancellationTokenSource cancellationTokenSource, string endPointName, Dictionary<string, Value> arguments, IReadOnlyList<int> devices, IReadOnlyList<string> friends, string callMethodId)
        {
            _cancellationTokenSource = cancellationTokenSource;
            _endPointName = endPointName;
            _arguments = arguments;
            _devices = devices;
            _friends = friends;
            _callMethodId = callMethodId;
        }

        public void SetTask(Task task)
        {
            _task = task;
        }

        /// <inheritdoc/>
        public override string EndPointName => _endPointName;

        /// <inheritdoc/>
        protected override void ProcessPlatformStart(IMonitorLogger logger)
        {
#if DEBUG
            logger.Info("3B8AD3FD-CF36-4C4F-8C2D-E8217EAEDEE0", $"Before _task.Start();{ToHumanizedLabel()}");
            logger.Info("EE039A53-4B2C-4A3F-8D4D-6FD2E5A917A6", $"_task.Status = {_task.Status};{ToHumanizedLabel()}");
#endif
            _task.Start();
            //_task.RunSynchronously();

#if DEBUG
            logger.Info("26E52663-E5B6-4442-8917-2A6E73B90540", $"After _task.Start();{ToHumanizedLabel()}");
            logger.Info("5BB6B976-1374-451B-AB2E-B37CDCFA89CD", $"_task.Status = {_task.Status};{ToHumanizedLabel()}");
#endif
        }

        /// <inheritdoc/>
        protected override void ProcessPlatformCancelation(IMonitorLogger logger)
        {
            _cancellationTokenSource.Cancel();
        }

        /// <inheritdoc/>
        public override IReadOnlyList<int> Devices => _devices;

        /// <inheritdoc/>
        public override IReadOnlyList<string> Friends => _friends;

        /// <inheritdoc/>
        public override bool IsFriend(IMonitorLogger logger, IProcessInfo other)
        {
            if ((!other.Friends.IsNullOrEmpty() && other.Friends.Any(p => p == EndPointName)) || (!Friends.IsNullOrEmpty() && Friends.Any(p => p == other.EndPointName)))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            lock (_statusLockObj)
            {
                if(!NIsFinished(_logger))
                {
                    _cancellationTokenSource.Cancel();
                }
            }

            base.OnDisposed();
        }

        #region private fields
        private readonly string _endPointName;
        private readonly Dictionary<string, Value> _arguments;
        private readonly IReadOnlyList<int> _devices;
        private readonly IReadOnlyList<string> _friends;
        private readonly string _callMethodId;
        private Task _task;
        private readonly CancellationTokenSource _cancellationTokenSource;
        #endregion

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            var sb = new StringBuilder($"proc: {Id} ({Status}); {EndPointName}");

            if (!_arguments.IsNullOrEmpty())
            {
                var argsStrList = new List<string>();

                foreach (var arg in _arguments)
                {
                    argsStrList.Add($"{arg.Key} = {arg.Value.ToHumanizedLabel(options)}");
                }

                sb.Append($" <{string.Join(", ", argsStrList)}>");
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            var sb = new StringBuilder($"proc: {Id} ({Status}); {EndPointName}");

            if (!_arguments.IsNullOrEmpty())
            {
                var argsStrList = new List<string>();

                foreach (var arg in _arguments)
                {
                    argsStrList.Add($"{arg.Key} = {arg.Value.ToHumanizedLabel(options)}");
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
            var result = new MonitoredHumanizedLabel();

            var sb = new StringBuilder($"proc: {Id} ({Status}); {EndPointName}");

            result.CallMethodId = _callMethodId;

#if DEBUG
            //logger.Info("58B26851-2F4D-417F-823B-F6EA0970DA68", $"arguments = {_arguments.WriteDict_3_ToString()}");
#endif

            if (_arguments != null)
            {
                var values = new List<MonitoredHumanizedMethodParameterValue>();

                foreach (var argument in _arguments)
                {
                    var item = new MonitoredHumanizedMethodParameterValue();
                    item.NameHumanizedStr = argument.Key;
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
