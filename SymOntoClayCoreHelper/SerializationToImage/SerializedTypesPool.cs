using System;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class SerializedTypesPool: ISerializedTypesPool
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        /// <inheritdoc/>
        public int GetOrRegisterType(Type type)
        {
#if DEBUG
            _logger.Info($"type?.Name = {type?.Name}");
            _logger.Info($"type?.FullName = {type?.FullName}");
#endif

            throw new NotImplementedException("C9B426E3-9803-4460-A016-88C9A6E92717");
        }
    }
}
