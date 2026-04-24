using System;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public abstract class BaseObjectSerializer: IObjectSerializer
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        protected BaseObjectSerializer(ISerializedObjectsPool serializedObjectsPool)
        {
            _serializedObjectsPool = serializedObjectsPool;
        }

        private readonly ISerializedObjectsPool _serializedObjectsPool;

        /// <inheritdoc/>
        public SerializedValue GetSerializedValue(object obj)
        {
#if DEBUG
            _logger.Info($"obj = {obj}");
#endif

            if (_serializedObjectsPool.TryGetSerializedValue(obj, out var serializedValue))
            {
                return serializedValue;
            }

            if (obj == null)
            {
                return _serializedObjectsPool.RegSerializedValue(obj);
            }

            var type = obj.GetType();

#if DEBUG
            _logger.Info($"type.FullName = {type.FullName}");
            _logger.Info($"type.Name = {type.Name}");
            _logger.Info($"type.IsGenericType = {type.IsGenericType}");
#endif

            if (type.FullName.StartsWith("System.Action"))
            {
                throw new NotImplementedException("C1D988C7-C669-4F35-BD9A-0CB5A905681D");
            }

            if (type.FullName.StartsWith("System.Func"))
            {
                throw new NotImplementedException("C95A929D-6481-4BFC-AE03-69AB4BDA31EA");
            }

            switch (type.FullName)
            {
                case "System.Object":
                    throw new NotImplementedException("C2852EA5-8D1B-4C22-9CF6-692DF4CAA5E3");

                case "System.Threading.CancellationTokenSource":
                    throw new NotImplementedException("C9A45E85-6923-46AE-8E46-EA25956B3385");

                case "System.Threading.CancellationTokenSource+Linked1CancellationTokenSource":
                case "System.Threading.CancellationTokenSource+Linked2CancellationTokenSource":
                case "System.Threading.CancellationTokenSource+LinkedNCancellationTokenSource":
                    throw new NotImplementedException("C2B5F85D-817D-4CDA-92CC-467C1B6947C8");

                case "System.Threading.CancellationToken":
                    throw new NotImplementedException("C14F0793-B2B5-417B-960F-346D27852C23");

                case "System.Threading.ManualResetEvent":
                    throw new NotImplementedException("C80B4E5C-B0C2-4670-AFF5-A9CB895DC57C");

                case "SymOntoClay.Threading.CustomThreadPool":
                    throw new NotImplementedException("C6891BB7-9AFD-4B3F-B0AA-BFD3C849B48D");
            }

            switch (type.Name)
            {
                case "List`1":
                    throw new NotImplementedException("C2461E4C-E8BF-4D0B-9FEC-99CD7FFA1A6A");

                case "Stack`1":
                    throw new NotImplementedException("C24CD827-A2CB-4918-8F7F-D7F877C1348E");

                case "Queue`1":
                    throw new NotImplementedException("C63365DB-1BDC-4DC6-901E-AF6D216B7122");

                case "Dictionary`2":
                    throw new NotImplementedException("C3B8333F-D325-485A-AC21-66F223500EB5");

                default:
                    if (type.FullName.StartsWith("System.Threading.") ||
                        type.FullName.StartsWith("System.Collections."))
                    {
                        throw new NotImplementedException("C7BE5FD7-C04F-4584-80C9-F2DDA4069926");
                    }

                    throw new NotImplementedException("C5394237-52FF-4CDB-89ED-5F3FEA383FD0");
            }
        }
    }
}
