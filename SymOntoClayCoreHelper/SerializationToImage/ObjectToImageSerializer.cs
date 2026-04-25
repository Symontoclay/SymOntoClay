using System;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class ObjectToImageSerializer: BaseObjectSerializer
    {
        public ObjectToImageSerializer(ISerializedObjectsPool serializedObjectsPool)
            : base(serializedObjectsPool)
        {
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeBareObject(object obj)
        {
            throw new NotImplementedException("C7AE1374-FAF6-4987-B2EC-18254BB986DD");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericList(object obj)
        {
            throw new NotImplementedException("C7F00063-BE35-44CD-9528-32C3600EF75E");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericStack(object obj)
        {
            throw new NotImplementedException("C577B505-79EB-4EB0-81D6-CEE7E181C31D");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericQueue(object obj)
        {
            throw new NotImplementedException("C3BE6016-0DA1-4238-BB7E-C12668369925");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericDictionary(object obj)
        {
            throw new NotImplementedException("C59FDA1A-7C7B-4E67-A6F4-B0507CA6E2DF");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeComposite(object obj)
        {
            throw new NotImplementedException("C8F59117-E3EC-4B09-84D2-CDBC0CB7CCD0");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeAction(object obj)
        {
            throw new NotImplementedException("C741439E-BC15-4F4C-8F0A-C775975A3863");
        }
    }
}
