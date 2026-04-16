using System;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class SerializerToImage
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public SerializerToImage(SerializationToImageSettings serializationSettings)
        {
        }

        public void Serialize(object obj)
        {
#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            throw new NotImplementedException("C5663A8F-AD33-4C0B-A90A-6E82E64D9D8C");
        }
    }
}
