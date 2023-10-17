using SymOntoClay.CLI.Helpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public class LogFileCreatorInheritableOptions: LogFileCreatorOptions, IInheritableConfiguration
    {
        /// <inheritdoc/>
        public string ParentCfg { get; set; }

        /// <inheritdoc/>
        void IInheritableConfiguration.Write(SymOntoClay.CLI.Helpers.IInheritableConfiguration source)
        {
            var originalSource = source as LogFileCreatorOptions;

            if (originalSource == null)
            {
                throw new ArgumentNullException(nameof(originalSource));
            }

            base.Write(originalSource);
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(ParentCfg)} = {ParentCfg}");
            sb.Append(base.PropertiesToString(n));

            return sb.ToString();
        }
    }
}
