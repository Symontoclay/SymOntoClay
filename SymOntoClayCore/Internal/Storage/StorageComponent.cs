using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class StorageComponent: BaseComponent, IStorageComponent
    {
        public StorageComponent(IMainStorageContext context, IStandaloneStorage parentStorage, KindOfStorage kindGlobalOfStorage)
            : base(context.Logger)
        {
            _context = context;
            _parentStorage = parentStorage;
            _kindGlobalOfStorage = kindGlobalOfStorage;
        }

        private readonly IMainStorageContext _context;
        private readonly IStandaloneStorage _parentStorage;
        private readonly KindOfStorage _kindGlobalOfStorage;

        private RealStorage _globalStorage;

        public IStorage GlobalStorage => _globalStorage;

        private List<RealStorage> _storagesList;

        public void LoadFromSourceCode()
        {
            _storagesList = new List<RealStorage>();

            var globalStorageSettings = new RealStorageSettings();
            globalStorageSettings.MainStorageContext = _context;

            if (_parentStorage != null && _parentStorage.Storage != null)
            {
                globalStorageSettings.ParentsStorages = new List<IStorage>() { _parentStorage.Storage };
            }

            switch(_kindGlobalOfStorage)
            {
                case KindOfStorage.Global:
                    _globalStorage = new GlobalStorage(globalStorageSettings);
                    break;

                case KindOfStorage.World:
                    _globalStorage = new WorldStorage(globalStorageSettings);
                    break;

                case KindOfStorage.Host:
                    _globalStorage = new HostStorage(globalStorageSettings);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_kindGlobalOfStorage), _kindGlobalOfStorage, null);
            }
            
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
