using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// Biped NPC (Non-Player Character) setting.
    /// </summary>
    public class BipedNPCSettings: BaseManualControllingGameComponentSettings
    {
        /// <summary>
        /// Gets or sets file name of SymOntoClay logic file.
        /// The file describes active logic which will be executed on the NPC.
        /// </summary>
        public string LogicFile { get; set; }

        public IPlatformSupport PlatformSupport { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(LogicFile)} = {LogicFile}");

            sb.PrintExisting(n, nameof(PlatformSupport), PlatformSupport);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }
    }
}
