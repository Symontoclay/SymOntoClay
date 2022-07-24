using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClayBaseTestLib.Helpers
{
    public class UnityTestEngineContextFactorySettings : IObjectToString
    {
        public string WorldFile { get; set; }
        public string NPCAppFile { get; set; }
        public bool UseDefaultAppFiles { get; set; } = true;
        public object HostListener { get; set; }
        public bool UseDefaultHostListener { get; set; } = true;
        public IPlatformLogger PlatformLogger { get; set; }
        public bool UseDefaultPlatformLogger { get; set; } = true;
        public Vector3? CurrentAbsolutePosition { get; set; }
        public bool UseDefaultCurrentAbsolutePosition { get; set; } = true;
        public IList<string> DictsPaths { get; set; }
        public IList<IWordsDict> DictsList { get; set; }
        public bool UseDefaultNLPSettings { get; set; } = true;

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

            sb.AppendLine($"{spaces}{nameof(WorldFile)} = {WorldFile}");
            sb.AppendLine($"{spaces}{nameof(NPCAppFile)} = {NPCAppFile}");
            sb.AppendLine($"{spaces}{nameof(UseDefaultAppFiles)} = {UseDefaultAppFiles}");

            sb.PrintExisting(n, nameof(HostListener), HostListener);
            sb.AppendLine($"{spaces}{nameof(UseDefaultHostListener)} = {UseDefaultHostListener}");

            sb.PrintExisting(n, nameof(PlatformLogger), PlatformLogger);
            sb.AppendLine($"{spaces}{nameof(UseDefaultPlatformLogger)} = {UseDefaultPlatformLogger}");

            sb.PrintExisting(n, nameof(CurrentAbsolutePosition), CurrentAbsolutePosition);
            sb.AppendLine($"{spaces}{nameof(UseDefaultCurrentAbsolutePosition)} = {UseDefaultCurrentAbsolutePosition}");

            sb.PrintPODList(n, nameof(DictsPaths), DictsPaths);
            sb.PrintExistingList(n, nameof(DictsList), DictsList);
            sb.AppendLine($"{spaces}{nameof(UseDefaultNLPSettings)} = {UseDefaultNLPSettings}");

            return sb.ToString();
        }
    }
}
