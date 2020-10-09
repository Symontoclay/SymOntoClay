/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.Instances
{
    public class ProcessInfo: BaseProcessInfo
    {
        /// <inheritdoc/>
        public override ProcessStatus Status 
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
                    if(_status == value)
                    {
                        return;
                    }

                    _status = value;

                    switch(_status)
                    {
                        case ProcessStatus.Completed:
                        case ProcessStatus.Canceled:
                        case ProcessStatus.Faulted:
                            Task.Run(() => {
                                OnFinish?.Invoke(this);
                            });
                            break;
                    }
                }
            }
        }

        public CodeFrame CodeFrame { get; set; }

        /// <inheritdoc/>
        public override IReadOnlyList<int> Devices => _devices;

        /// <inheritdoc/>
        public override event ProcessInfoEvent OnFinish;

        /// <inheritdoc/>
        public override void Start()
        {
            //throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Cancel()
        {
            throw new NotImplementedException();
        }

        #region private fields
        private readonly object _lockObj = new object();
        private ProcessStatus _status = ProcessStatus.Created;
        private readonly List<int> _devices = new List<int>();
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
    }
}
