using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.MetadataStorage
{
    public class MetadataStorage: BaseLoggedComponent, IMetadataStorage
    {
        public MetadataStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;
        }

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        /// <inheritdoc/>
        public IStorage Storage => _realStorageContext.Storage;

        private readonly object _lockObj = new object();

        private CodeEntity _mainCodeEntity;

        private Dictionary<Name, CodeEntity> _codeEntitiesDict = new Dictionary<Name, CodeEntity>();

        /// <inheritdoc/>
        public void Append(CodeEntity codeEntity)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"codeEntity = {codeEntity}");
#endif

                var name = codeEntity.Name;

#if DEBUG
                //Log($"name = {name}");
#endif

                if (_codeEntitiesDict.ContainsKey(name))
                {
                    throw new NotImplementedException();
                }
                else
                {
                    if (codeEntity.Kind == KindOfCodeEntity.App && codeEntity.CodeFile != null && codeEntity.CodeFile.IsMain)
                    {
                        _mainCodeEntity = codeEntity;
                    }

                    _codeEntitiesDict[name] = codeEntity;
                }              
            }
        }

        /// <inheritdoc/>
        public CodeEntity MainCodeEntity 
        {
            get
            {
                return _mainCodeEntity;
            }
        }
    }
}
