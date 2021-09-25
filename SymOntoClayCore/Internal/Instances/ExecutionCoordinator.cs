﻿using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class ExecutionCoordinator: IExecutionCoordinator
    {
        private readonly object _lockObj = new object();

        /// <inheritdoc/>
        public ActionExecutionStatus ExecutionStatus 
        {
            get 
            { 
                lock(_lockObj)
                {
                    return _executionStatus;
                }
            }

            set 
            {
                lock (_lockObj)
                {
                    if(_executionStatus == value)
                    {
                        return;
                    }

                    _executionStatus = value;

                    if(_executionStatus != ActionExecutionStatus.Executing)
                    {
                        OnFinished?.Invoke();
                    }
                }
            }
        }

        private ActionExecutionStatus _executionStatus;

        /// <inheritdoc/>
        public RuleInstance RuleInstance
        {
            get
            {
                lock (_lockObj)
                {
                    return _ruleInstance;
                }
            }

            set
            {
                lock (_lockObj)
                {
                    _ruleInstance = value;
                }
            }
        }

        private RuleInstance _ruleInstance;

        /// <inheritdoc/>
        public event Action OnFinished;

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

            sb.AppendLine($"{spaces}{nameof(ExecutionStatus)} = {ExecutionStatus}");
            sb.PrintObjProp(n, nameof(RuleInstance), RuleInstance);

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

            sb.AppendLine($"{spaces}{nameof(ExecutionStatus)} = {ExecutionStatus}");
            sb.PrintShortObjProp(n, nameof(RuleInstance), RuleInstance);

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

            sb.AppendLine($"{spaces}{nameof(ExecutionStatus)} = {ExecutionStatus}");
            sb.PrintBriefObjProp(n, nameof(RuleInstance), RuleInstance);

            return sb.ToString();
        }
    }
}