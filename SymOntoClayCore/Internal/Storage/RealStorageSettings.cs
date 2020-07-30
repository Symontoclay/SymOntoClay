using SymOntoClay.Core.Internal.Compiling;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class RealStorageSettings: IObjectToString
    {
        public IMainStorageContext MainStorageContext { get; set; }
        public IList<IStorage> ParentsStorages { get; set; }
        public DefaultSettingsOfCodeEntity DefaultSettingsOfCodeEntity { get; set; }

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
            sb.PrintExisting(n, nameof(MainStorageContext), MainStorageContext);
            sb.PrintObjListProp(n, nameof(ParentsStorages), ParentsStorages);
            sb.PrintObjProp(n, nameof(DefaultSettingsOfCodeEntity), DefaultSettingsOfCodeEntity);
            return sb.ToString();
        }
    }
}
