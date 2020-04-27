using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public class LoggingSettings: IObjectToString
    {
        public string LogDir { get; set; }
        public string RootContractName { get; set; }
        public bool EnableLogging { get; set; }
        public bool EnableRemoteConnection { get; set; }
        public IList<IPlatformLogger> PlatformLoggers { get; set; }

        /// <inheritdoc/>
        public string PropertiesToString(uint n)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            throw new NotImplementedException();
        }
    }
}
