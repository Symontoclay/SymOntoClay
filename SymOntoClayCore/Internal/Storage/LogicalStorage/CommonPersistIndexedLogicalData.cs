using NLog;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.LogicalStorage
{
    public class CommonPersistIndexedLogicalData
    {
#if DEBUG
        private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public void NSetIndexedRuleInstanceToIndexData(IndexedRuleInstance indexedRuleInstance)
        {
#if DEBUG
            _gbcLogger.Info($"indexedRuleInstance = {indexedRuleInstance}");
#endif

            //throw new NotImplementedException();

#if IMAGINE_WORKING
            _gbcLogger.Info("End");
#else
            throw new NotImplementedException();
#endif
        }
    }
}
