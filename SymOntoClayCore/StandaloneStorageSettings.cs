using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public class StandaloneStorageSettings: IObjectToString
    {
        public bool IsWorld { get; set; }

        /// <summary>
        /// Gets or sets unique Id.
        /// It allows us to identify each item of the game.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets app file.
        /// </summary>
        public string AppFile { get; set; }

        /// <summary>
        /// Gets or sets reference to logger.
        /// </summary>
        public IEntityLogger Logger { get; set; }

        /// <summary>
        /// Gets or ses reference to shared dictionary.
        /// </summary>
        public IEntityDictionary Dictionary { get; set; }

        /// <summary>
        /// Gets or ses reference to shared modules storage.
        /// </summary>
        public IModulesStorage ModulesStorage { get; set; }

        /// <summary>
        /// Gets or sets parent storage.
        /// </summary>
        public IStandaloneStorage ParentStorage { get; set; }

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
            sb.AppendLine($"{spaces}{nameof(IsWorld)} = {IsWorld}");
            sb.AppendLine($"{spaces}{nameof(Id)} = {Id}");
            sb.AppendLine($"{spaces}{nameof(AppFile)} = {AppFile}");
            sb.PrintExisting(n, nameof(Logger), Logger);
            sb.PrintExisting(n, nameof(Dictionary), Dictionary);
            sb.PrintExisting(n, nameof(ModulesStorage), ModulesStorage);
            sb.PrintExisting(n, nameof(ParentStorage), ParentStorage);
            return sb.ToString();
        }
    }
}
