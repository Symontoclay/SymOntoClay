/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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

        public IHostSupport HostSupport { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintExisting(n, nameof(HostListener), HostListener);

            sb.PrintExisting(n, nameof(SyncContext), SyncContext);
            sb.AppendLine($"{spaces}{nameof(TmpDir)} = {TmpDir}");
            sb.PrintExisting(n, nameof(HostSupport), HostSupport);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }
    }
}
