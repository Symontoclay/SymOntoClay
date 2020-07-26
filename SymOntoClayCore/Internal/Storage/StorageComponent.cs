using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class StorageComponent: BaseComponent, IStorageComponent
    {
        public StorageComponent(IEngineContext context, IStandaloneStorage parentStorage)
            : base(context.Logger)
        {
            _context = context;
            _parentStorage = parentStorage;
        }

        private readonly IEngineContext _context;
        private readonly IStandaloneStorage _parentStorage;

        private GlobalStorage _globalStorage;

        public IStorage GlobalStorage => _globalStorage;

        private List<RealStorage> _storagesList;

        public void LoadFromSourceCode()
        {
            _storagesList = new List<RealStorage>();

            var globalStorageSettings = new RealStorageSettings();
            globalStorageSettings.Logger = Logger;
            globalStorageSettings.EntityDictionary = _context.Dictionary;
            globalStorageSettings.Compiler = _context.Compiler;

            if (_parentStorage.Storage != null)
            {
                globalStorageSettings.ParentsStorages = new List<IStorage>() { _parentStorage.Storage };
            }

            globalStorageSettings.CommonNamesStorage = _context.CommonNamesStorage;

            _globalStorage = new GlobalStorage(globalStorageSettings);
            _storagesList.Add(_globalStorage);

            _globalStorage.DefaultSettingsOfCodeEntity = CreateDefaultSettingsOfCodeEntity();

#if IMAGINE_WORKING
            //Log("Do");
#else
                throw new NotImplementedException();
#endif
        }

        private DefaultSettingsOfCodeEntity CreateDefaultSettingsOfCodeEntity()
        {
            var result = new DefaultSettingsOfCodeEntity();

            return result;
        }
    }
}
