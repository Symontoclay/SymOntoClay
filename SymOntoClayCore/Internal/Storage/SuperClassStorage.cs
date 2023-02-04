using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class SuperClassStorage : RealStorage
    {
        public SuperClassStorage(RealStorageSettings settings, StrongIdentifierValue targetClassName)
            : base(KindOfStorage.SuperClass, settings)
        {
            _targetClassName = targetClassName;
        }

        private readonly StrongIdentifierValue _targetClassName;

        /// <inheritdoc/>
        public override StrongIdentifierValue TargetClassName => _targetClassName;
    }
}
