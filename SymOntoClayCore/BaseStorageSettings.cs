using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core
{
    public abstract class BaseStorageSettings: BaseCoreSettings
    {
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
        /// Gets or ses reference to shared modules storage.
        /// </summary>
        public IModulesStorage ModulesStorage { get; set; }

        /// <summary>
        /// Gets or sets parent storage.
        /// </summary>
        public IStandaloneStorage ParentStorage { get; set; }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Id)} = {Id}");
            sb.AppendLine($"{spaces}{nameof(AppFile)} = {AppFile}");

            sb.PrintExisting(n, nameof(ModulesStorage), ModulesStorage);
            sb.PrintExisting(n, nameof(ParentStorage), ParentStorage);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }
    }
}
