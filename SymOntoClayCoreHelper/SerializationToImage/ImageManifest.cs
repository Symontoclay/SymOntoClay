using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System;
using System.Text;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class ImageManifest: IObjectToString
    {
        public Version Version { get; set; }
        public string RootObjectsRelativePath { get; set; }
        public string RootObjectsAndSettingsRelativePath { get; set; }
        public string ObjectsDataRelativePath { get; set; }
        public string ImageRootRelativePath { get; set; }
        public string SerializedTypesRelativePath { get; set; }

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
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Version)} = {Version}");
            sb.AppendLine($"{spaces}{nameof(RootObjectsRelativePath)} = {RootObjectsRelativePath}");
            sb.AppendLine($"{spaces}{nameof(RootObjectsAndSettingsRelativePath)} = {RootObjectsAndSettingsRelativePath}");
            sb.AppendLine($"{spaces}{nameof(ObjectsDataRelativePath)} = {ObjectsDataRelativePath}");
            sb.AppendLine($"{spaces}{nameof(ImageRootRelativePath)} = {ImageRootRelativePath}");
            sb.AppendLine($"{spaces}{nameof(SerializedTypesRelativePath)} = {SerializedTypesRelativePath}");

            return sb.ToString();
        }
    }
}
