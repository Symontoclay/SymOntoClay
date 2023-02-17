using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage
{
    public class SuperClassStorage : RealStorage
    {
        public SuperClassStorage(RealStorageSettings settings, StrongIdentifierValue targetClassName, IInstance instance)
            : base(KindOfStorage.SuperClass, settings)
        {
            _targetClassName = targetClassName;
            _instance = instance;
            _instanceName= instance.Name;
        }

        private readonly StrongIdentifierValue _targetClassName;
        private readonly StrongIdentifierValue _instanceName;
        private readonly IInstance _instance;

        /// <inheritdoc/>
        public override StrongIdentifierValue TargetClassName => _targetClassName;

        /// <inheritdoc/>
        public override StrongIdentifierValue InstanceName => _instanceName;

        /// <inheritdoc/>
        public override IInstance Instance => _instance;
    }
}
