using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.ActionsStorage
{
    public class ActionsStorage: BaseLoggedComponent, IActionsStorage
    {
        public ActionsStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.MainStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;
        }

        private readonly object _lockObj = new object();

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        /// <inheritdoc/>
        public IStorage Storage => _realStorageContext.Storage;

        /// <inheritdoc/>
        public void Append(ActionDef action)
        {
            lock (_lockObj)
            {
#if DEBUG
                Log($"action = {action}");
#endif

                AnnotatedItemHelper.CheckAndFillUpHolder(action, _realStorageContext.MainStorageContext.CommonNamesStorage);

                action.CheckDirty();

                throw new NotImplementedException();
            }                
        }
    }
}
