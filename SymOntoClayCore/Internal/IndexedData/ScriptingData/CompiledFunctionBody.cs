using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.IndexedData.ScriptingData
{
    public class CompiledFunctionBody : IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public Dictionary<int, ScriptCommand> Commands { get; set; } = new Dictionary<int, ScriptCommand>();

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
            var nextNSpaces = DisplayHelper.Spaces(nextN);
            var nextNextN = nextN + 4;
            var nextNextNSpaces = DisplayHelper.Spaces(nextNextN);
            var nextNextNextN = nextNextN + 4;
            var sb = new StringBuilder();

            if (Commands == null)
            {
                sb.AppendLine($"{spaces}{nameof(Commands)} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(Commands)}");
                foreach (var item in Commands)
                {
                    sb.AppendLine($"{nextNSpaces}Begin Item");
                    sb.AppendLine($"{nextNextNSpaces}Key = {item.Key}");
                    sb.AppendLine($"{nextNextNSpaces}Begin Value");
                    sb.Append(item.Value.ToString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Value");
                    sb.AppendLine($"{nextNSpaces}End Item");
                }
                sb.AppendLine($"{spaces}End {nameof(Commands)}");
            }

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
            var nextN = n + 4;
            var nextNSpaces = DisplayHelper.Spaces(nextN);
            var nextNextN = nextN + 4;
            var nextNextNSpaces = DisplayHelper.Spaces(nextNextN);
            var nextNextNextN = nextNextN + 4;
            var sb = new StringBuilder();

            if (Commands == null)
            {
                sb.AppendLine($"{spaces}{nameof(Commands)} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(Commands)}");
                foreach (var item in Commands)
                {
                    sb.AppendLine($"{nextNSpaces}Begin Item");
                    sb.AppendLine($"{nextNextNSpaces}Key = {item.Key}");
                    sb.AppendLine($"{nextNextNSpaces}Begin Value");
                    sb.Append(item.Value.ToShortString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Value");
                    sb.AppendLine($"{nextNSpaces}End Item");
                }
                sb.AppendLine($"{spaces}End {nameof(Commands)}");
            }

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
            var nextN = n + 4;
            var nextNSpaces = DisplayHelper.Spaces(nextN);
            var nextNextN = nextN + 4;
            var nextNextNSpaces = DisplayHelper.Spaces(nextNextN);
            var nextNextNextN = nextNextN + 4;
            var sb = new StringBuilder();

            if (Commands == null)
            {
                sb.AppendLine($"{spaces}{nameof(Commands)} = NULL");
            }
            else
            {
                sb.AppendLine($"{spaces}Begin {nameof(Commands)}");
                foreach (var item in Commands)
                {
                    sb.AppendLine($"{nextNSpaces}Begin Item");
                    sb.AppendLine($"{nextNextNSpaces}Key = {item.Key}");
                    sb.AppendLine($"{nextNextNSpaces}Begin Value");
                    sb.Append(item.Value.ToBriefString(nextNextNextN));
                    sb.AppendLine($"{nextNextNSpaces}End Value");
                    sb.AppendLine($"{nextNSpaces}End Item");
                }
                sb.AppendLine($"{spaces}End {nameof(Commands)}");
            }

            return sb.ToString();
        }
    }
}
