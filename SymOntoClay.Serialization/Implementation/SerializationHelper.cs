using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace SymOntoClay.Serialization.Implementation
{
    public static class SerializationHelper
    {
#if DEBUG
        private static ILogger _logger = LogManager.GetCurrentClassLogger();
#endif

        private static CultureInfo _cultureInfo = new CultureInfo("en-150");

        public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
            TypeNameHandling = TypeNameHandling.All,
            Culture = _cultureInfo
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

            if(type.Name == "Nullable`1")
            {
                throw new NotImplementedException("70CA8FE2-2DFB-4916-B9CE-16C4341DE645");
            }

            switch (type.FullName)
            {
                case "System.Byte":
                    return true;

                case "System.SByte":
                    return true;

                case "System.Int16":
                    return true;

                case "System.Int32":
                    return true;

                case "System.Int64":
                    return true;

                case "System.UInt16":
                    return true;

                case "System.UInt32":
                    return true;

                case "System.UInt64":
                    return true;

                case "System.Single":
                    return true;

                case "System.Decimal":
                    return true;

                case "System.Double":
                    return true;

                case "System.Boolean":
                    return true;

                case "System.String":
                    return true;

                case "System.Char":
                    return true;

                case "System.DateTime":
                    return true;

                case "System.DateOnly":
                    return true;

                case "System.TimeOnly":
                    return true;

                case "System.TimeSpan":
                    return true;

                default:
                    return false;
            }
        }

        public static FieldInfo[] GetFields(object obj)
        {
            return GetFields(obj.GetType());
        }

        public static FieldInfo[] GetFields(Type type)
        {
            var result = new List<FieldInfo>();

            var targetType = type;

            while (targetType != null)
            {
                result.AddRange(targetType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance));

                targetType = targetType.BaseType;
            }

            return result.ToArray();
        }
    }
}
