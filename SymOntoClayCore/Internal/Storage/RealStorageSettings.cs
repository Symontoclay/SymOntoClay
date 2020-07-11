using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class RealStorageSettings: IObjectToString
    {
        public IEntityLogger Logger { get; set; }
        public IEntityDictionary EntityDictionary { get; set; }
        public ICompiler Compiler { get; set; }
        public ICommonNamesStorage CommonNamesStorage { get; set; }
        public IList<IStorage> ParentsStorages { get; set; }

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
            sb.PrintExisting(n, nameof(Logger), Logger);
            sb.PrintExisting(n, nameof(EntityDictionary), EntityDictionary);
            sb.PrintExisting(n, nameof(Compiler), Compiler);
            sb.PrintObjListProp(n, nameof(ParentsStorages), ParentsStorages);
            return sb.ToString();
        }
    }
}
