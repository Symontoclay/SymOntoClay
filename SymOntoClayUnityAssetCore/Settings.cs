using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    /// <summary>
    /// General settings of a game world.
    /// </summary>
    public class Settings: IObjectToString
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
        /// Gets or sets logging settings.
        /// </summary>
        public LoggingSettings Logging { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var nextN = n + 4;
            var nextNSpaces = DisplayHelper.Spaces(nextN);
            var sb = new StringBuilder();

            if(Logging == null)
            {
                sb.AppendLine($"{spaces}{nameof(Logging)} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(Logging)}");
                sb.Append(Logging.ToString(nextN));
                sb.AppendLine($"{spaces}End {nameof(Logging)}");
            }

            if(SourceFilesDirs == null)
            {
                sb.AppendLine($"{spaces}{nameof(SourceFilesDirs)} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(SourceFilesDirs)}");
                foreach(var item in SourceFilesDirs)
                {
                    sb.AppendLine($"{nextNSpaces}{item}");
                }
                sb.AppendLine($"{spaces}End {nameof(SourceFilesDirs)}");
            }

            sb.AppendLine($"{spaces}{nameof(ImagesRootDir)} = {ImagesRootDir}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }
    }
}
