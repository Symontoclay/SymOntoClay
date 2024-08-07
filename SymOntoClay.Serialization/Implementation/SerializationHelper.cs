using Newtonsoft.Json;
using NLog;
using System;

namespace SymOntoClay.Serialization.Implementation
{
    public static class SerializationHelper
    {
#if DEBUG
        private static ILogger _logger = LogManager.GetCurrentClassLogger();
#endif

        public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
            TypeNameHandling = TypeNameHandling.All//,
            //ReferenceLoopHandling = ReferenceLoopHandling.Serialize
        };

        public static bool IsObject(Type type)
        {
            return type == typeof(object);
        }

        public static bool IsObjectPtr(Type type)
        {
            return type == typeof(ObjectPtr);
        }

        public static bool IsPrimitiveType(object item)
        {
            if (item == null)
            {
                return true;
            }

            return IsPrimitiveType(item.GetType());
        }

        public static bool IsPrimitiveType(Type type)
        {
#if DEBUG
            _logger.Info($"type.FullName = {type.FullName}");
            _logger.Info($"type.Name = {type.Name}");
            _logger.Info($"type.IsGenericType = {type.IsGenericType}");
#endif

            switch (type.FullName)
            {
                case "System.Int32":
                    return true;

                case "System.Int64":
                    return true;

                default:
                    return false;
            }
        }
    }
}
