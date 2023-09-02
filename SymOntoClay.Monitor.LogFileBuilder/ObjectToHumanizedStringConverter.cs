using Newtonsoft.Json;
using SymOntoClay.Core.DebugHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SymOntoClay.Monitor.LogFileBuilder
{
    public static class ObjectToHumanizedStringConverter
    {
#if DEBUG
        private static readonly global::NLog.ILogger _globalLogger = global::NLog.LogManager.GetCurrentClassLogger();
#endif

        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            PreserveReferencesHandling = PreserveReferencesHandling.All
        };

        public static string FromBase64StringToHumanizedString(string base64Content, string typeName, HumanizedOptions options = HumanizedOptions.ShowAll)
        {
#if DEBUG
            _globalLogger.Info($"base64Content = '{base64Content}'");
            _globalLogger.Info($"typeName = {typeName}");
            _globalLogger.Info($"options = {options}");
#endif

            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return FromBase64StringToHumanizedString(base64Content, typeName, opt);
        }

        public static string FromBase64StringToHumanizedString(string base64Content, string typeName, DebugHelperOptions options)
        {
#if DEBUG
            _globalLogger.Info($"base64Content = '{base64Content}'");
            _globalLogger.Info($"typeName = {typeName}");
            _globalLogger.Info($"options = {options}");
#endif

            var base64bytes = Convert.FromBase64String(base64Content);
            var jsonStr = Encoding.UTF8.GetString(base64bytes);

#if DEBUG
            _globalLogger.Info($"jsonStr = {jsonStr}");
#endif

            if(typeName.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                return "NULL";
            }

            var type = Type.GetType(typeName);

#if DEBUG
            _globalLogger.Info($"type.FullName = {type.FullName}");
#endif

            var obj = JsonConvert.DeserializeObject(jsonStr, type, _jsonSerializerSettings);

#if DEBUG
            _globalLogger.Info($"obj?.GetType().FullName = {obj?.GetType().FullName}");
#endif

            return ToHumanizedString(obj, type, options);
        }

        public static string ToHumanizedString(object obj, HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return ToHumanizedString(obj, opt);
        }

        public static string ToHumanizedString(object obj, DebugHelperOptions options)
        {
            return ToHumanizedString(obj, obj?.GetType(), options);
        }

        public static string ToHumanizedString(object obj, Type type, DebugHelperOptions options)
        {
#if DEBUG
            _globalLogger.Info($"type?.FullName = {type?.FullName}");
            _globalLogger.Info($"type?.IsClass = {type?.IsClass}");
            _globalLogger.Info($"type?.IsPrimitive = {type?.IsPrimitive}");
            _globalLogger.Info($"type?.IsEnum = {type?.IsEnum}");
            _globalLogger.Info($"type?.IsGenericType = {type?.IsGenericType}");

            _globalLogger.Info($"options = {options}");
#endif

            if(obj == null)
            {
                return "NULL";
            }

            if(type == typeof(int))
            {
                return obj.ToString();
            }

            if(type.GetInterfaces().Contains(typeof(IDictionary)))
            {
#if DEBUG
                _globalLogger.Info($"Dictionary!!!!");
#endif

                var dict = (IDictionary)obj;

                var dict_2 = new Dictionary<object, object>();

                foreach(DictionaryEntry de in dict)
                {
                    dict_2.Add(de.Key, de.Value);
                }

                return JsonConvert.SerializeObject(dict_2.ToDictionary(p => ToHumanizedString(p.Key, p.Key?.GetType(), options), p => ToHumanizedString(p.Value, p.Value?.GetType(), options)));
            }

            //return string.Empty;
            throw new NotImplementedException();
        }
    }
}
