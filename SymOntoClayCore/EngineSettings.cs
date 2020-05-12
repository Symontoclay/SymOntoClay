﻿using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public class EngineSettings: IObjectToString
    {
        /// <summary>
        /// Gets or sets unique Id.
        /// It allows us to identify each item of the game.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets app file.
        /// </summary>
        public string AppFile { get; set; }

        /// <summary>
        /// Gets or sets host listener.
        /// </summary>
        public IHostListener HostListener { get; set; }

        /// <summary>
        /// Gets or sets reference to logger.
        /// </summary>
        public IEntityLogger Logger { get; set; }

        /// <summary>
        /// Gets or ses reference to shared dictionary.
        /// </summary>
        public IEntityDictionary Dictionary { get; set; }

        /// <summary>
        /// Gets or ses reference to shared modules storage.
        /// </summary>
        public IModulesStorage ModulesStorage { get; set; }

        /// <summary>
        /// Gets or sets parent storage.
        /// </summary>
        public IStandaloneStorage ParentStorage { get; set; }

        public ISyncContext SyncContext { get; set; }

        public string TmpDir { get; set; }

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
            var nextN = n + 4;
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Id)} = {Id}");
            sb.AppendLine($"{spaces}{nameof(AppFile)} = {AppFile}");
            sb.PrintExisting(n, nameof(HostListener), HostListener);
            sb.PrintExisting(n, nameof(Logger), Logger);
            sb.PrintExisting(n, nameof(Dictionary), Dictionary);
            sb.PrintExisting(n, nameof(ModulesStorage), ModulesStorage);
            sb.PrintExisting(n, nameof(ParentStorage), ParentStorage);
            sb.PrintExisting(n, nameof(SyncContext), SyncContext);
            sb.AppendLine($"{spaces}{nameof(TmpDir)} = {TmpDir}");
            return sb.ToString();
        }
    }
}
