using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public class EngineSettings: BaseStorageSettings
    {
        /// <summary>
        /// Gets or sets host listener.
        /// </summary>
        public IHostListener HostListener { get; set; }

        public IActivePeriodicObjectCommonContext SyncContext { get; set; }

        public string TmpDir { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintExisting(n, nameof(HostListener), HostListener);

            sb.PrintExisting(n, nameof(SyncContext), SyncContext);
            sb.AppendLine($"{spaces}{nameof(TmpDir)} = {TmpDir}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }
    }
}
