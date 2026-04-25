using System;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class RootObjectsAndSettingsSerializer : BaseObjectSerializer
    {
        public RootObjectsAndSettingsSerializer(ISerializedObjectsPool serializedObjectsPool)
            : base(serializedObjectsPool)
        {
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeBareObject(object obj)
        {
            throw new NotImplementedException("C3125912-DADF-4A90-AC93-19102439840D");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericList(object obj)
        {
            throw new NotImplementedException("C9EB3A71-1CFD-47C5-8095-F8A7AEEE7296");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericStack(object obj)
        {
            throw new NotImplementedException("C81F1DFE-FACE-4AFA-BD4A-ED501B903D3F");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericQueue(object obj)
        {
            throw new NotImplementedException("C925989B-E02A-469D-9703-C73A22E7491D");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericDictionary(object obj)
        {
            throw new NotImplementedException("C6176FA0-9C26-4183-B80B-3A3D7E0D873F");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeComposite(object obj)
        {
            throw new NotImplementedException("C90EDF11-865C-4725-ABA4-A803814DC014");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeAction(object obj)
        {
            throw new NotImplementedException("C6866BBD-6E0A-46CD-BBCA-2D9B66381B2B");
        }
    }
}
