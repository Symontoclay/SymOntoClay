using Newtonsoft.Json;
using NLog;
using System;
using System.Globalization;

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

        private static CultureInfo _cultureInfo = new CultureInfo("en-150");

        public static string PrimitiveTypeToString(Type type, object obj)
        {
#if DEBUG
            _logger.Info($"type.FullName = {type.FullName}");
            _logger.Info($"type.Name = {type.Name}");
            _logger.Info($"type.IsGenericType = {type.IsGenericType}");
            _logger.Info($"obj = '{obj}'");
            _logger.Info($"CultureInfo.CurrentCulture?.Name = '{CultureInfo.CurrentCulture?.Name}'");
            _logger.Info($"CultureInfo.CurrentCulture?.NativeName = '{CultureInfo.CurrentCulture?.NativeName}'");
#endif

            switch (type.FullName)
            {
                case "System.Byte":
                case "System.SByte":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.String":
                case "System.Char":
                case "System.Boolean":
                case "System.TimeSpan":
                    return obj.ToString();

                case "System.Single":
                    return ((System.Single)obj).ToString(_cultureInfo);

                case "System.Decimal":
                    return ((System.Decimal)obj).ToString(_cultureInfo);

                case "System.Double":
                    return ((System.Double)obj).ToString(_cultureInfo);

                case "System.DateTime":
                    return ((System.DateTime)obj).ToString(_cultureInfo);

                //case "System.DateOnly":
                //    return ((System.DateOnly)obj).ToString(_cultureInfo);

                //case "System.TimeOnly":
                //    return ((System.TimeOnly)obj).ToString(_cultureInfo);

                default:
                    throw new ArgumentOutOfRangeException(nameof(type.FullName), type.FullName, null);
            }
        }

        public static object PrimitiveTypeFromString(Type type, string primitiveValue)
        {
#if DEBUG
            _logger.Info($"type.FullName = {type.FullName}");
            _logger.Info($"type.Name = {type.Name}");
            _logger.Info($"type.IsGenericType = {type.IsGenericType}");
            _logger.Info($"primitiveValue = '{primitiveValue}'");
            _logger.Info($"CultureInfo.CurrentCulture?.Name = '{CultureInfo.CurrentCulture?.Name}'");
            _logger.Info($"CultureInfo.CurrentCulture?.NativeName = '{CultureInfo.CurrentCulture?.NativeName}'");
#endif

            switch (type.FullName)
            {
                case "System.Byte":
                    return System.Byte.Parse(primitiveValue);

                case "System.SByte":
                    return System.SByte.Parse(primitiveValue);

                case "System.Int16":
                    return System.Int16.Parse(primitiveValue);

                case "System.Int32":
                    return System.Int32.Parse(primitiveValue);

                case "System.Int64":
                    return System.Int64.Parse(primitiveValue);

                case "System.UInt16":
                    return System.UInt16.Parse(primitiveValue);

                case "System.UInt32":
                    return System.UInt32.Parse(primitiveValue);

                case "System.UInt64":
                    return System.UInt64.Parse(primitiveValue);

                case "System.Single":
                    return System.Single.Parse(primitiveValue, _cultureInfo);

                case "System.Decimal":
                    return System.Decimal.Parse(primitiveValue, _cultureInfo);

                case "System.Double":
                    return System.Double.Parse(primitiveValue, _cultureInfo);

                case "System.Boolean":
                    return System.Boolean.Parse(primitiveValue);

                case "System.String":
                    return primitiveValue;

                case "System.Char":
                    return System.Char.Parse(primitiveValue);

                case "System.DateTime":
                    return System.DateTime.Parse(primitiveValue, _cultureInfo);

                //case "System.DateOnly":
                //    return true;

                //case "System.TimeOnly":
                //    return true;

                case "System.TimeSpan":
                    return System.TimeSpan.Parse(primitiveValue);

                default:
                    throw new ArgumentOutOfRangeException(nameof(type.FullName), type.FullName, null);
            }
        }
    }
}
