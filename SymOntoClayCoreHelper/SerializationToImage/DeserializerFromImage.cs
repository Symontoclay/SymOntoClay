using System;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class DeserializerFromImage
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public DeserializerFromImage(SerializationToImageSettings serializationSettings, IStructuralContext structuralContext)
        {
            _structuralContext = structuralContext;
        }

        private readonly IStructuralContext _structuralContext;

        public void Deserialize(object obj)
        {
#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            throw new NotImplementedException("C072330D-6772-4582-948D-CA412DCD07FE");
        }
    }
}
