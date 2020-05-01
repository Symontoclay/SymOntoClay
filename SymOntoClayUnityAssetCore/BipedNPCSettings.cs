using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// Biped NPC (Non-Player Character) setting.
    /// </summary>
    public class BipedNPCSettings: IObjectToString
    {
        /// <summary>
        /// Gets or sets file name of SymOntoClay logic file.
        /// The file describes active logic which will be executed on the NPC.
        /// </summary>
        public string LogicFile { get; set; }

        /// <summary>
        /// Gets or sets file name of SymOntoClay host file.
        /// The file describes facts which are visible for other NPCs or can be recognized in some way by player.
        /// </summary>
        public string HostFile { get; set; }

        /// <summary>
        /// Gets or sets host listener.
        /// </summary>
        public IHostListener HostListener { get; set; }

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
        public string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(LogicFile)} = {LogicFile}");
            sb.AppendLine($"{spaces}{nameof(HostFile)} = {HostFile}");

            var hostListenerMark = HostListener == null ? "No" : "Yes";
            sb.AppendLine($"{spaces}{nameof(HostListener)} = {hostListenerMark}");

            return sb.ToString();
        }
    }
}
