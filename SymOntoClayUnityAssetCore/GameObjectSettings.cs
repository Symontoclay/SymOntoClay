using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// Game object settings.
    /// </summary>
    public class GameObjectSettings : IObjectToString
    {
        /// <summary>
        /// Gets or sets unique Id.
        /// It allows us to identify each item of the game.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets file name of SymOntoClay host file.
        /// The file describes facts which are visible for other NPCs or can be recognized in some way by player.
        /// </summary>
        public string HostFile { get; set; }

        /// <summary>
        /// Gets or sets host listener.
        /// </summary>
        public IPlatformHostListener HostListener { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(HostFile)} = {HostFile}");

            var hostListenerMark = HostListener == null ? "No" : "Yes";
            sb.AppendLine($"{spaces}{nameof(HostListener)} = {hostListenerMark}");

            return sb.ToString();
        }
    }
}
