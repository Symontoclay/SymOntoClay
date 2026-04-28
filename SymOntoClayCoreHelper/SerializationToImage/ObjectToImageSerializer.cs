using System;
using System.Collections.Generic;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class ObjectToImageSerializer: BaseObjectSerializer
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public ObjectToImageSerializer(ISerializedObjectsPool serializedObjectsPool)
            : base(serializedObjectsPool)
        {
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeBareObject(object obj, Type type)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"F39D8410-288B-428D-9CE7-0A2E1C9B1186: please check type '{type.FullName}'");
            }
#endif

            throw new NotImplementedException("C7AE1374-FAF6-4987-B2EC-18254BB986DD");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericList(object obj, Type type, string path)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"F1157033-4AE6-43B1-A81C-502CB932F4B6: please check type '{type.FullName}'");
            }
#endif

            throw new NotImplementedException("C7F00063-BE35-44CD-9528-32C3600EF75E");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericStack(object obj, Type type, string path)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"B55F1AE6-7670-4158-84AC-20053293DD4A: please check type '{type.FullName}'");
            }
#endif

            throw new NotImplementedException("C577B505-79EB-4EB0-81D6-CEE7E181C31D");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericQueue(object obj, Type type, string path)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"E9247CFC-4ADD-4F14-9D3B-EA5BB918A2FB: please check type '{type.FullName}'");
            }
#endif

            throw new NotImplementedException("C3BE6016-0DA1-4238-BB7E-C12668369925");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeGenericDictionary(object obj, Type type, string path)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"567F2A4C-26BD-4ECB-81CA-5A09C06C4D8A: please check type '{type.FullName}'");
            }
#endif

            throw new NotImplementedException("C59FDA1A-7C7B-4E67-A6F4-B0507CA6E2DF");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeComposite(object obj, Type type, string path)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"FA40B5E6-45E9-4945-8B24-7409410975B1: please check type '{type.FullName}'");
            }
#endif

            throw new NotImplementedException("C8F59117-E3EC-4B09-84D2-CDBC0CB7CCD0");
        }

        /// <inheritdoc/>
        protected override SerializedValue SerializeAction(object obj, Type type)
        {
#if DEBUG
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"184ECF5C-C554-47B9-BD3B-272CE9BAD9B4: please check type '{type.FullName}'");
            }
#endif

            throw new NotImplementedException("C741439E-BC15-4F4C-8F0A-C775975A3863");
        }

        private readonly List<string> _tmpProcessedTypes = new List<string>();
    }
}
