﻿using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public abstract class BaseStoredGameComponentSettings: BaseGameComponentSettings
    {
        /// <summary>
        /// Gets or sets file name of SymOntoClay host file.
        /// The file describes facts which are visible for other NPCs or can be recognized in some way by player.
        /// </summary>
        public string HostFile { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(HostFile)} = {HostFile}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }
    }
}
