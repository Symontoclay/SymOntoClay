using SymOntoClay.CoreHelper.DebugHelpers;
using SymOntoClay.UnityAsset.Core.Internal.EndPoints.MainThread;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// General settings of a game world.
    /// </summary>
    public class WorldSettings: IObjectToString
    {
        /// <summary>
        /// Gets or sets list of file paths for describing places for searching required source files.
        /// </summary>
        public IList<string> SourceFilesDirs { get; set; }

        /// <summary>
        /// Gets or sets root dir for saving and loading images of executed code.
        /// </summary>
        public string ImagesRootDir { get; set; }

        /// <summary>
        /// Gets or sets list of file paths for describing places searching dictionaries of translation text to facts.
        /// </summary>
        public IList<string> DictionariesDirs { get; set; }

        public string TmpDir { get; set; }

        /// <summary>
        /// Gets or sets logging settings.
        /// </summary>
        public LoggingSettings Logging { get; set; }

        /// <summary>
        /// Gets or sets file name of SymOntoClay host file.
        /// The file describes facts which are visible for other NPCs or can be recognized in some way by player.
        /// </summary>
        public string HostFile { get; set; }

        public IInvokerInMainThread InvokerInMainThread { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var nextNSpaces = DisplayHelper.Spaces(nextN);
            var sb = new StringBuilder();

            sb.PrintObjProp(n, nameof(Logging), Logging);
            sb.PrintPODList(n, nameof(SourceFilesDirs), SourceFilesDirs);
            sb.AppendLine($"{spaces}{nameof(ImagesRootDir)} = {ImagesRootDir}");
            sb.PrintPODList(n, nameof(DictionariesDirs), DictionariesDirs);
            sb.AppendLine($"{spaces}{nameof(TmpDir)} = {TmpDir}");
            sb.AppendLine($"{spaces}{nameof(HostFile)} = {HostFile}");
            sb.PrintExisting(n, nameof(InvokerInMainThread), InvokerInMainThread);

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }
    }
}
