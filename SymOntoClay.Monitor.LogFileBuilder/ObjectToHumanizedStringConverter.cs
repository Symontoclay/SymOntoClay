/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SymOntoClay.Core.DebugHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        //private static readonly global::NLog.ILogger _globalLogger = global::NLog.LogManager.GetCurrentClassLogger();
#endif

        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            Converters = { new ArrayReferencePreservngConverter() },
            PreserveReferencesHandling = PreserveReferencesHandling.All
        };

        public static string FromBase64StringToHumanizedString(string base64Content, string typeName, HumanizedOptions options = HumanizedOptions.ShowAll)
        {
#if DEBUG
            //_globalLogger.Info($"base64Content = '{base64Content}'");
            //_globalLogger.Info($"typeName = {typeName}");
            //_globalLogger.Info($"options = {options}");
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
            //_globalLogger.Info($"base64Content = '{base64Content}'");
            //_globalLogger.Info($"typeName = {typeName}");
            //_globalLogger.Info($"options = {options}");
#endif

            var base64bytes = Convert.FromBase64String(base64Content);
            var jsonStr = Encoding.UTF8.GetString(base64bytes);

#if DEBUG
            //_globalLogger.Info($"jsonStr = {jsonStr}");
#endif

            if(typeName == null || typeName.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                return "NULL";
            }

            var type = Type.GetType(typeName);

#if DEBUG
            //_globalLogger.Info($"type.FullName = {type?.FullName}");
#endif

            if (type == null)
            {
                return jsonStr;
            }

            var obj = JsonConvert.DeserializeObject(jsonStr, type, _jsonSerializerSettings);

#if DEBUG
            //_globalLogger.Info($"obj?.GetType().FullName = {obj?.GetType().FullName}");
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
            //_globalLogger.Info($"type?.FullName = {type?.FullName}");
            //_globalLogger.Info($"type?.IsClass = {type?.IsClass}");
            //_globalLogger.Info($"type?.IsPrimitive = {type?.IsPrimitive}");
            //_globalLogger.Info($"type?.IsEnum = {type?.IsEnum}");
            //_globalLogger.Info($"type?.IsGenericType = {type?.IsGenericType}");

            //_globalLogger.Info($"options = {options}");
#endif

            if(obj == null)
            {
                return "NULL";
            }

            if(type.IsEnum)
            {
                return obj.ToString();
            }

            if(type == typeof(sbyte) ||
                type == typeof(byte) ||
                type == typeof(short) ||
                type == typeof(ushort) ||
                type == typeof(int) || 
                type == typeof(uint) ||
                type == typeof(long) ||
                type == typeof(ulong) ||
                type == typeof(nint) ||
                type == typeof(nuint) ||
                type == typeof(string))
            {
                return obj.ToString();
            }

            if(type == typeof(DateTime))
            {
                return ((DateTime)obj).ToString("dd.MM.yyyy HH:mm:ss.fffff");
            }

            var interfaces = type.GetInterfaces();

            if(interfaces.Contains(typeof(IObjectToHumanizedString)))
            {
                return ((IObjectToHumanizedString)obj).ToHumanizedString(options);
            }

            if(interfaces.Contains(typeof(IDictionary)))
            {
#if DEBUG
                //_globalLogger.Info($"Dictionary!!!!");
#endif

                var dict = (IDictionary)obj;

                var dict_2 = new Dictionary<object, object>();

                foreach(DictionaryEntry de in dict)
                {
                    dict_2.Add(de.Key, de.Value);
                }

                return JsonConvert.SerializeObject(dict_2.ToDictionary(p => ToHumanizedString(p.Key, p.Key?.GetType(), options), p => ToHumanizedString(p.Value, p.Value?.GetType(), options)));
            }

            if(interfaces.Contains(typeof(IList)))
            {
#if DEBUG
                //_globalLogger.Info($"List!!!!");
#endif

                var list = (IList)obj;

                var list_2 = new List<object>();

                foreach(var item in list)
                {
                    list_2.Add(item);
                }

                return JsonConvert.SerializeObject(list_2.Select(p => ToHumanizedString(p, p?.GetType(), options)));
            }

            return obj.ToString();
        }
    }
}
