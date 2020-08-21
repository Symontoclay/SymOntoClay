using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public abstract class BaseManualControllingGameComponentSettings: BaseStoredGameComponentSettings
    {
        /// <summary>
        /// Gets or sets host listener.
        /// </summary>
        public object HostListener { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintExisting(n, nameof(HostListener), HostListener);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }
    }
}
