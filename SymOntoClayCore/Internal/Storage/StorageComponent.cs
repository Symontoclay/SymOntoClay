using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class StorageComponent: BaseComponent, IStorageComponent
    {
        public StorageComponent(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IEngineContext _context;

        private GlobalStorage _globalStorage;

        private List<RealStorage> _storagesList;

        public void LoadFromSourceCode()
        {
            _storagesList = new List<RealStorage>();

            _globalStorage = new GlobalStorage(Logger, _context.Dictionary);
            _storagesList.Add(_globalStorage);

#if IMAGINE_WORKING
            Log("Do");
#else
                throw new NotImplementedException();
#endif
        }
    }
}
