using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class ProcessInfo: IProcessInfo
    {
        public ProcessInfo()
        {
            Id = NameHelper.GetNewEntityNameString();
        }

        /// <inheritdoc/>
        public string Id { get; private set; }

        /// <inheritdoc/>
        public ProcessStatus Status 
        { 
            get
            {
                lock(_lockObj)
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

        public CodeFrame CodeFrame { get; set; }

        #region private fields
        private readonly object _lockObj = new object();
        private ProcessStatus _status = ProcessStatus.Created;
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
            sb.PrintObjProp(n, nameof(CodeFrame), CodeFrame);

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
            sb.PrintShortObjProp(n, nameof(CodeFrame), CodeFrame);

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
            sb.PrintBriefObjProp(n, nameof(CodeFrame), CodeFrame);

            return sb.ToString();
        }
    }
}
