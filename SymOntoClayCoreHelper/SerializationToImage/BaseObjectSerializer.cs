using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        protected readonly ISerializedObjectsPool _serializedObjectsPool;

        /// <inheritdoc/>
        public SerializedValue SerializeValue(object obj, string path = "", SerializeValueMode serializeValueMode = SerializeValueMode.General)
        {
#if DEBUG
            _logger.Info($"obj = {obj}");
            _logger.Info($"path = {path}");
            _logger.Info($"serializeValueMode = {serializeValueMode}");
#endif

            if (TryGetSerializedValue(obj, out var serializedValue))
            {
#if DEBUG
                _logger.Info($"serializedValue = {serializedValue}");
#endif

                return serializedValue;
            }
            
            if (obj == null)
            {
                return _serializedObjectsPool.RegSerializedValue(obj, SerializedObjectsPoolMode.General);
            }

            if(serializeValueMode == SerializeValueMode.ExternalValue)
            {
                return _serializedObjectsPool.RegSerializedValue(obj, SerializedObjectsPoolMode.ExternalValue);
            }

            var type = obj.GetType();

#if DEBUG
            _logger.Info($"type.FullName = {type.FullName}");
            _logger.Info($"type.Name = {type.Name}");
            _logger.Info($"type.IsGenericType = {type.IsGenericType}");
#endif

            if(type.IsEnum)
            {
                return SerializePrimitiveType(obj, type);
            }

            if (type.FullName.StartsWith("System.Action"))
            {
                return SerializeAction(obj, type);
            }

            if (type.FullName.StartsWith("System.Func"))
            {
                return SerializeAction(obj, type);
            }

            switch (type.FullName)
            {
                case "System.Object":
                    return SerializeBareObject(obj, type, path);

                case "System.Threading.CancellationTokenSource":
                    throw new NotImplementedException("C9A45E85-6923-46AE-8E46-EA25956B3385");

                case "System.Threading.CancellationTokenSource+Linked1CancellationTokenSource":
                case "System.Threading.CancellationTokenSource+Linked2CancellationTokenSource":
                case "System.Threading.CancellationTokenSource+LinkedNCancellationTokenSource":
                    throw new NotImplementedException("C2B5F85D-817D-4CDA-92CC-467C1B6947C8");

                case "System.Threading.CancellationToken":
                    throw new NotImplementedException("C14F0793-B2B5-417B-960F-346D27852C23");

                case "System.Threading.ManualResetEvent":
                    return SerializeManualResetEvent(obj, type, path);

                case "SymOntoClay.Threading.CustomThreadPool":
                    throw new NotImplementedException("C6891BB7-9AFD-4B3F-B0AA-BFD3C849B48D");

                case "System.Byte":
                case "System.SByte":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.Single":
                case "System.Decimal":
                case "System.Double":
                case "System.Boolean":
                case "System.String":
                case "System.Char":
                case "System.DateTime":
                case "System.DateOnly":
                case "System.TimeOnly":
                case "System.TimeSpan":
                    return SerializePrimitiveType(obj, type);
            }

            switch (type.Name)
            {
                case "List`1":
                    return SerializeGenericList(obj, type, path);

                case "Stack`1":
                    return SerializeGenericStack(obj, type, path);

                case "Queue`1":
                    return SerializeGenericQueue(obj, type, path);

                case "Dictionary`2":
                    return SerializeGenericDictionary(obj, type, path);

                default:
                    if (type.FullName.StartsWith("System.Threading.") ||
                        type.FullName.StartsWith("System.Collections."))
                    {
                        throw new NotImplementedException("C7BE5FD7-C04F-4584-80C9-F2DDA4069926");
                    }

                    return SerializeComposite(obj, type, path);
            }
        }

        protected virtual bool TryGetSerializedValue(object obj, out SerializedValue serializedValue)
        {
            return _serializedObjectsPool.TryGetSerializedValue(obj, out serializedValue);
        }

        protected virtual SerializedValue SerializePrimitiveType(object obj, Type type)
        {
            return _serializedObjectsPool.RegSerializedValue(obj, SerializedObjectsPoolMode.General);
        }

        protected abstract SerializedValue SerializeBareObject(object obj, Type type, string path);
        protected abstract SerializedValue SerializeGenericList(object obj, Type type, string path);
        protected abstract SerializedValue SerializeGenericStack(object obj, Type type, string path);
        protected abstract SerializedValue SerializeGenericQueue(object obj, Type type, string path);
        protected abstract SerializedValue SerializeGenericDictionary(object obj, Type type, string path);
        protected abstract SerializedValue SerializeComposite(object obj, Type type, string path);
        protected abstract SerializedValue SerializeManualResetEvent(object obj, Type type, string path);
        protected abstract SerializedValue SerializeAction(object obj, Type type);

        protected IEnumerable<FieldInfo> GetFields(Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(p => !p.Name.EndsWith("k__BackingField"));
        }

        protected IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(p => p.CanWrite && IsAutoProperty(p));
        }

        public static bool IsAutoProperty(PropertyInfo prop)
        {
            var backingField = prop.DeclaringType.GetField($"<{prop.Name}>k__BackingField",
                BindingFlags.NonPublic | BindingFlags.Instance);

            return backingField != null;
        }

        protected void TmpCheckProcessedTypes(string id, Type type)
        {
            if (!_tmpProcessedTypes.Contains(type.FullName))
            {
                throw new NotSupportedException($"{id}: please check type '{type.FullName}'");
            }
        }

        protected void TmpCheckProcessedMembersOfTypes(string id, Type type, string memberName)
        {
            if(_tmpProcessedMembersOfTypes.TryGetValue(type.FullName, out var memberNamesList))
            {
                if (!memberNamesList.Contains(memberName))
                {
                    throw new NotSupportedException($"{id}: please check mebmer '{memberName}' of type '{type.FullName}'");
                }
            }
            else
            {
                throw new NotSupportedException($"{id}: please check type '{type.FullName}'");
            }
        }

        protected abstract List<string> _tmpProcessedTypes { get; set; }
        protected abstract Dictionary<string, List<string>> _tmpProcessedMembersOfTypes { get; set; }
    }
}
