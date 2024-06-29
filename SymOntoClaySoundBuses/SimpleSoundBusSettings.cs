using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SymOntoClay.SoundBuses
{
    public class SimpleSoundBusSettings: IObjectToString
    {
        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
        public CustomThreadPoolSettings ThreadingSettings { get; set; }

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
            sb.PrintExisting(n, nameof(CancellationToken), CancellationToken);
            sb.PrintObjProp(n, nameof(ThreadingSettings), ThreadingSettings);
            return sb.ToString();
        }
    }
}
