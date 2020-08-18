using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Internal.EndPoints
{
    public class PlatformProcessInfo : IProcessInfo
    {
        public PlatformProcessInfo(Task task, CancellationTokenSource cancellationTokenSource, IList<int> devices)
        {
            Id = NameHelper.GetNewEntityNameString();
            _task = task;
            _cancellationTokenSource = cancellationTokenSource;
            Devices = devices;
        }

        /// <inheritdoc/>
        public string Id { get; private set; }

        /// <inheritdoc/>
        public ProcessStatus Status
        {
            get
            {
                lock (_lockObj)
                {
                    return _status;
                }
            }

            set
            {
                lock (_lockObj)
                {
                    _status = value;
                }
            }
        }

        /// <inheritdoc/>
        public IList<int> Devices { get; private set; }

        /// <inheritdoc/>
        public void Start()
        {
            lock (_lockObj)
            {
                _task.Start();
                _status = ProcessStatus.Running;
            }
        }

        /// <inheritdoc/>
        public void Cancel()
        {
            lock (_lockObj)
            {
                _cancellationTokenSource.Cancel();
                _status = ProcessStatus.Canceled;
            }
        }

        #region private fields
        private readonly object _lockObj = new object();
        private ProcessStatus _status = ProcessStatus.Created;
        private readonly Task _task;
        private readonly CancellationTokenSource _cancellationTokenSource;
        #endregion

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

            sb.AppendLine($"{spaces}{nameof(Status)} = {Status}");
            sb.PrintValueTypesListProp(n, nameof(Devices), Devices);

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

            sb.AppendLine($"{spaces}{nameof(Status)} = {Status}");
            sb.PrintValueTypesListProp(n, nameof(Devices), Devices);

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

            sb.AppendLine($"{spaces}{nameof(Status)} = {Status}");
            sb.PrintValueTypesListProp(n, nameof(Devices), Devices);

            return sb.ToString();
        }
    }
}
