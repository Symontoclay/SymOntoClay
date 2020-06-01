using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.LogicalStorage
{
    public class LogicalStorage: BaseLoggedComponent, ILogicalStorage
    {
        public LogicalStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;
        }

        private readonly KindOfStorage _kind;

        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        public void Append(RuleInstance ruleInstance)
        {
            Append(ruleInstance, true);
        }

        public void Append(RuleInstance ruleInstance, bool isPrimary)
        {
#if DEBUG
            Log($"ruleInstance = {ruleInstance}");
            Log($"isPrimary = {isPrimary}");
#endif

            //throw new NotImplementedException();

#if IMAGINE_WORKING
            Log("End");
#else
            throw new NotImplementedException();
#endif
        }
    }
}
