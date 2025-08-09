/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.CoreHelper
{
    public static class ObjectHelper
    {
#if DEBUG
        private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public static bool IsNumber(object value)
        {
            if(value == null)
            {
                return false;
            }

            var type = value.GetType();

            if(type == typeof(float))
            {
                return true;
            }

            if (type == typeof(double))
            {
                return true;
            }

            if (type == typeof(decimal))
            {
                return true;
            }

            if (type == typeof(ulong))
            {
                return true;
            }

            if (type == typeof(long))
            {
                return true;
            }

            if (type == typeof(uint))
            {
                return true;
            }

            if (type == typeof(int))
            {
                return true;
            }

            if (type == typeof(ushort))
            {
                return true;
            }

            if (type == typeof(short))
            {
                return true;
            }

            if (type == typeof(byte))
            {
                return true;
            }

            return false;
        }

        public static bool IsEquals(object value1, object value2)
        {
            if (value1.Equals(value2))
            {
                return true;
            }

            if (IsNumber(value1) && IsNumber(value2))
            {
                if (Convert.ToDouble(value1) == Convert.ToDouble(value2))
                {
                    return true;
                }
            }

            return false;
        }

        public static List<Type> GetAllBaseTypes(Type type)
        {
            var result = new List<Type>();

            result.AddRange(type.GetInterfaces());

            FillUpBaseTypes(type.BaseType, result);

            return result;
        }

        private static void FillUpBaseTypes(Type type, List<Type> result)
        {
            if(type == null)
            {
                return;
            }

            result.Add(type);

            FillUpBaseTypes(type.BaseType, result);
        }

        public static void PrintUnknownObjPropOptString(this StringBuilder sb, uint n, string propName, object value)
        {
            var spaces = DisplayHelper.Spaces(n);

            if(value == null)
            {
                sb.AppendLine($"{spaces}{propName} = NULL");
                return;
            }

            var nextN = n + DisplayHelper.IndentationStep;

            var type = value.GetType();

#if DEBUG
            //_globalLogger.Info($"type.FullName = {type.FullName}");
            //_globalLogger.Info($"type.Name = {type.Name}");
            //_globalLogger.Info($"type.IsGenericType = {type.IsGenericType}");
#endif

            if(type.IsGenericType)
            {
                if(type.Name == "List`1")
                {
                    NPrintUnknownObjListPropOptString(sb, n, propName, value, type);
                    return;
                }

                if(type.Name == "Dictionary`2")
                {
                    NPrintUnknownObjDictPropOptString(sb, n, propName, value, type);
                    return;
                }
            }

            if(Implements(type, typeof(IObjectToString)))
            {
                var convertedValue = (IObjectToString)value;

                sb.PrintObjProp(n, propName, convertedValue);
                return;
            }

            sb.AppendLine($"{spaces}{propName} = {value}");
        }

        private static void NPrintUnknownObjListPropOptString(StringBuilder sb, uint n, string propName, object value, Type type)
        {
            var genericParamType = type.GetGenericArguments()[0];

#if DEBUG
            //_globalLogger.Info($"genericParamType.FullName = {genericParamType.FullName}");
#endif

            var sourceList = (IEnumerable)value;

            if (Implements(genericParamType, typeof(IObjectToString)))
            {
                sb.PrintObjListProp(n, propName, ConvertToListWithKnownType<IObjectToString>(sourceList));

                return;
            }

            sb.PrintPODListProp(n, propName, ConvertToListWithKnownType<object>(sourceList));
        }

        private static void NPrintUnknownObjDictPropOptString(StringBuilder sb, uint n, string propName, object value, Type type)
        {
            var keyGenericParamType = type.GetGenericArguments()[0];
            var valueGenericParamType = type.GetGenericArguments()[1];

#if DEBUG
            //_globalLogger.Info($"keyGenericParamType.FullName = {keyGenericParamType.FullName}");
            //_globalLogger.Info($"valueGenericParamType.FullName = {valueGenericParamType.FullName}");
#endif

            var sourceDict = (IDictionary)value;

            if (Implements(keyGenericParamType, typeof(IObjectToString)))
            {
                if(Implements(valueGenericParamType, typeof(IObjectToString)))
                {
                    sb.PrintObjDict_1_Prop(n, propName, ConvertToDictionaryKnownTypes<IObjectToString, IObjectToString>(sourceDict));

                    return;
                }
            }
            else
            {
                if(keyGenericParamType == typeof(string))
                {
                    if (Implements(valueGenericParamType, typeof(IObjectToString)))
                    {
                        sb.PrintObjDict_3_Prop(n, propName, ConvertToDictionaryKnownTypes<string, IObjectToString>(sourceDict));

                        return;
                    }
                    else
                    {
                        sb.PrintPODDictProp(n, propName, ConvertToDictionaryKnownTypes<object, object>(sourceDict));

                        return;
                    }
                }
                else
                {
                    sb.PrintPODDictProp(n, propName, ConvertToDictionaryKnownTypes<object, object>(sourceDict));

                    return;
                }
            }

            sb.PrintPODDictProp(n, propName, ConvertToDictionaryKnownTypes<object, object>(sourceDict));
        }

        public static void PrintUnknownObjPropOptShortString(this StringBuilder sb, uint n, string propName, object value)
        {
            var spaces = DisplayHelper.Spaces(n);

            if (value == null)
            {
                sb.AppendLine($"{spaces}{propName} = NULL");
                return;
            }

            var nextN = n + DisplayHelper.IndentationStep;

            var type = value.GetType();

#if DEBUG
            //_globalLogger.Info($"type.FullName = {type.FullName}");
            //_globalLogger.Info($"type.Name = {type.Name}");
            //_globalLogger.Info($"type.IsGenericType = {type.IsGenericType}");
#endif

            if (type.IsGenericType && type.Name == "List`1")
            {
                NPrintUnknownObjListPropOptShortString(sb, n, propName, value, type);
                return;
            }

            if (Implements(type, typeof(IObjectToShortString)))
            {
                var convertedValue = (IObjectToShortString)value;

                sb.PrintShortObjProp(n, propName, convertedValue);
                return;
            }

            sb.AppendLine($"{spaces}{propName} = {value}");
        }

        private static void NPrintUnknownObjListPropOptShortString(StringBuilder sb, uint n, string propName, object value, Type type)
        {
            var genericParamType = type.GetGenericArguments()[0];

#if DEBUG
            //_globalLogger.Info($"genericParamType.FullName = {genericParamType.FullName}");
#endif

            var sourceList = (IEnumerable)value;

            if (Implements(genericParamType, typeof(IObjectToShortString)))
            {
                sb.PrintShortObjListProp(n, propName, ConvertToListWithKnownType<IObjectToShortString>(sourceList));

                return;
            }

            sb.PrintPODListProp(n, propName, ConvertToListWithKnownType<object>(sourceList));
        }

        public static void PrintUnknownObjPropOptBriefString(this StringBuilder sb, uint n, string propName, object value)
        {
            var spaces = DisplayHelper.Spaces(n);

            if (value == null)
            {
                sb.AppendLine($"{spaces}{propName} = NULL");
                return;
            }

            var nextN = n + DisplayHelper.IndentationStep;

            var type = value.GetType();

#if DEBUG
            //_globalLogger.Info($"type.FullName = {type.FullName}");
            //_globalLogger.Info($"type.Name = {type.Name}");
            //_globalLogger.Info($"type.IsGenericType = {type.IsGenericType}");
#endif

            if (type.IsGenericType && type.Name == "List`1")
            {
                NPrintUnknownObjListPropOptBriefString(sb, n, propName, value, type);
                return;
            }

            if (Implements(type, typeof(IObjectToBriefString)))
            {
                var convertedValue = (IObjectToBriefString)value;

                sb.PrintBriefObjProp(n, propName, convertedValue);
                return;
            }

            sb.AppendLine($"{spaces}{propName} = {value}");
        }

        private static void NPrintUnknownObjListPropOptBriefString(StringBuilder sb, uint n, string propName, object value, Type type)
        {
            var genericParamType = type.GetGenericArguments()[0];

#if DEBUG
            //_globalLogger.Info($"genericParamType.FullName = {genericParamType.FullName}");
#endif

            var sourceList = (IEnumerable)value;

            if (Implements(genericParamType, typeof(IObjectToBriefString)))
            {
                sb.PrintBriefObjListProp(n, propName, ConvertToListWithKnownType<IObjectToBriefString>(sourceList));

                return;
            }

            sb.PrintPODListProp(n, propName, ConvertToListWithKnownType<object>(sourceList));
        }

        public static void PrintUnknownObjPropOptDbgString(this StringBuilder sb, uint n, string propName, object value)
        {
            var spaces = DisplayHelper.Spaces(n);

            if (value == null)
            {
                sb.AppendLine($"{spaces}{propName} = NULL");
                return;
            }

            var nextN = n + DisplayHelper.IndentationStep;

            var type = value.GetType();

#if DEBUG
            //_globalLogger.Info($"type.FullName = {type.FullName}");
            //_globalLogger.Info($"type.Name = {type.Name}");
            //_globalLogger.Info($"type.IsGenericType = {type.IsGenericType}");
#endif

            if (type.IsGenericType && type.Name == "List`1")
            {
                NPrintUnknownObjListPropOptDbgString(sb, n, propName, value, type);
                return;
            }

            if (Implements(type, typeof(IObjectToDbgString)))
            {
                var convertedValue = (IObjectToDbgString)value;

                sb.PrintDbgObjProp(n, propName, convertedValue);
                return;
            }

            sb.AppendLine($"{spaces}{propName} = {value}");
        }

        private static void NPrintUnknownObjListPropOptDbgString(StringBuilder sb, uint n, string propName, object value, Type type)
        {
            var genericParamType = type.GetGenericArguments()[0];

#if DEBUG
            //_globalLogger.Info($"genericParamType.FullName = {genericParamType.FullName}");
#endif

            var sourceList = (IEnumerable)value;

            if (Implements(genericParamType, typeof(IObjectToDbgString)))
            {
                sb.PrintDbgObjListProp(n, propName, ConvertToListWithKnownType<IObjectToDbgString>(sourceList));

                return;
            }

            sb.PrintPODListProp(n, propName, ConvertToListWithKnownType<object>(sourceList));
        }

        private static bool Implements(Type type, Type interfaceType)
        {
            return type.FindInterfaces((m, filterCriteria) => { return (m == interfaceType) ? true : false; }, null).Length > 0;
        }

        private static List<T> ConvertToListWithKnownType<T>(IEnumerable sourceList)
        {
            var convertedList = new List<T>();

            foreach (object item in sourceList)
            {
                convertedList.Add((T)item);
            }

            return convertedList;
        }

        private static Dictionary<K, V> ConvertToDictionaryKnownTypes<K, V>(IDictionary sourceDict)
        {
            var convertedDictionary = new Dictionary<K, V>();

            foreach(DictionaryEntry item in sourceDict)
            {
#if DEBUG
                _globalLogger.Info($"item = {item}");
                _globalLogger.Info($"item.GetType().FullName = {item.GetType().FullName}");
#endif

                convertedDictionary[(K)item.Key] = (V)item.Value;
            }

            return convertedDictionary;
        }
    }
}
