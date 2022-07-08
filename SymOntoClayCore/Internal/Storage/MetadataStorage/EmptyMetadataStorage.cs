using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.MetadataStorage
{
    public class EmptyMetadataStorage : BaseEmptySpecificStorage, IMetadataStorage
    {
        public EmptyMetadataStorage(IStorage storage, IEntityLogger logger)
            : base(storage, logger)
        {
        }

        /// <inheritdoc/>
        public void Append(CodeItem codeItem)
        {
        }

        /// <inheritdoc/>
        public CodeItem MainCodeEntity => null;

        /// <inheritdoc/>
        public CodeItem GetByName(StrongIdentifierValue name)
        {
            return null;
        }
    }
}
