using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClayProjectFiles
{
    public class WorldSpaceFiles : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public string LogicFile { get; set; }
        public string WorldFile { get; set; }
        public string SharedLibrariesDir { get; set; }
        public string ImagesRootDir { get; set; }
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
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(LogicFile)} = {LogicFile}");
            sb.AppendLine($"{spaces}{nameof(WorldFile)} = {WorldFile}");
            sb.AppendLine($"{spaces}{nameof(SharedLibrariesDir)} = {SharedLibrariesDir}");
            sb.AppendLine($"{spaces}{nameof(ImagesRootDir)} = {ImagesRootDir}");
            sb.AppendLine($"{spaces}{nameof(TmpDir)} = {TmpDir}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(LogicFile)} = {LogicFile}");
            sb.AppendLine($"{spaces}{nameof(WorldFile)} = {WorldFile}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(LogicFile)} = {LogicFile}");
            sb.AppendLine($"{spaces}{nameof(WorldFile)} = {WorldFile}");

            return sb.ToString();
        }
    }
}
