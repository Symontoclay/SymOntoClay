/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using SymOntoClay.Core;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.TypesConverters
{
    public class PlatformTypesConvertersRegistry : BaseLoggedComponent, IPlatformTypesConvertersRegistry
    {
        public PlatformTypesConvertersRegistry(IEntityLogger logger)
            : base(logger)
        {
        }

        private readonly Type _nullValueType = typeof(NullValue);

        private readonly object _lockObj = new object();
        private readonly Dictionary<Type, Dictionary<Type, IPlatformTypesConverter>> _convertorsDict = new Dictionary<Type, Dictionary<Type, IPlatformTypesConverter>>();

        /// <inheritdoc/>
        public void AddConvertor(IPlatformTypesConverter convertor)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"convertor = {convertor}");
#endif

                Dictionary<Type, IPlatformTypesConverter> targetDict = null;

                if (convertor.CanConvertToCoreType)
                {
                    if (_convertorsDict.ContainsKey(convertor.PlatformType))
                    {
                        targetDict = _convertorsDict[convertor.PlatformType];
                    }
                    else
                    {
                        targetDict = new Dictionary<Type, IPlatformTypesConverter>();
                        _convertorsDict[convertor.PlatformType] = targetDict;
                    }

                    targetDict[convertor.CoreType] = convertor;
                }

                if (convertor.CanConvertToPlatformType)
                {
                    if (_convertorsDict.ContainsKey(convertor.CoreType))
                    {
                        targetDict = _convertorsDict[convertor.CoreType];
                    }
                    else
                    {
                        targetDict = new Dictionary<Type, IPlatformTypesConverter>();
                        _convertorsDict[convertor.CoreType] = targetDict;
                    }

                    targetDict[convertor.PlatformType] = convertor;
                }
            }
        }

        /// <inheritdoc/>
        public bool CanConvert(Type source, Type dest)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"source.FullName = {source.FullName}");
                //Log($"dest.FullName = {dest.FullName}");
                //Log($"dest.IsGenericType = {dest.IsGenericType}");
                //Log($"dest.IsNested = {dest.IsNested}");
                //Log($"dest.IsPrimitive = {dest.IsPrimitive}");
                //Log($"dest.IsSpecialName = {dest.IsSpecialName}");
                //Log($"dest.IsClass = {dest.IsClass}");
                //Log($"dest.IsInterface = {dest.IsInterface}");

                //if (dest.IsGenericType)
                //{
                //    foreach(var genericArgument in dest.GenericTypeArguments)
                //    {
                //        Log($"genericArgument.FullName = {genericArgument.FullName}");
                //    }
                //}
#endif

                if (source == _nullValueType && (dest.IsClass || dest.IsInterface || (dest.IsGenericType && dest.FullName.StartsWith("System.Nullable"))))
                {
#if DEBUG
                    //Log($"source == _nullValueType && (dest.IsClass || dest.IsInterface || (dest.IsGenericType && dest.FullName.StartsWith(\"System.Nullable\")))");
#endif

                    return true;
                }

                if(NCanConvert(source, dest))
                {
#if DEBUG
                    //Log($"NCanConvert(source, dest)");
#endif

                    return true;
                }

                var baseSourceTypes = ObjectHelper.GetAllBaseTypes(source).Where(p => !IsTooGeneralType(p)).ToList();

                foreach (var baseSourceType in baseSourceTypes)
                {
#if DEBUG
                    //Log($"baseSourceType.FullName = {baseSourceType.FullName}");
#endif

                    if (NCanConvert(baseSourceType, dest))
                    {
#if DEBUG
                        //Log($"NCanConvert(baseSourceType, dest)");
#endif

                        return true;
                    }
                }

                var baseDestTypes = ObjectHelper.GetAllBaseTypes(dest).Where(p => !IsTooGeneralType(p)).ToList();

                foreach(var baseDestType in baseDestTypes)
                {
#if DEBUG
                    //Log($"baseDestType.FullName = {baseDestType.FullName}");
#endif
                        

                    if (NCanConvert(source, baseDestType))
                    {
#if DEBUG
                        //Log($"NCanConvert(source, baseDestType)");
#endif

                        return true;
                    }

                    foreach (var baseSourceType in baseSourceTypes)
                    {
#if DEBUG
                        //Log($"baseSourceType.FullName (2) = {baseSourceType.FullName}");
#endif

                        if (NCanConvert(baseSourceType, baseDestType))
                        {
#if DEBUG
                            //Log($"NCanConvert(baseSourceType, baseDestType)");
#endif

                            return true;
                        }
                    }
                }

                return false;
            }
        }

        private bool IsTooGeneralType(Type type)
        {
            if(type == typeof(object))
            {
                return true;
            }

            if(type == typeof(SymOntoClay.Core.Internal.CodeModel.IObjectWithLongHashCodes))
            {
                return true;
            }

            if (type == typeof(SymOntoClay.Core.Internal.CodeModel.IAnnotatedItem))
            {
                return true;
            }

            if (type == typeof(SymOntoClay.CoreHelper.DebugHelpers.IObjectToString))
            {
                return true;
            }

            if (type == typeof(SymOntoClay.CoreHelper.DebugHelpers.IObjectToShortString))
            {
                return true;
            }

            if (type == typeof(SymOntoClay.CoreHelper.DebugHelpers.IObjectToBriefString))
            {
                return true;
            }

            if (type == typeof(SymOntoClay.CoreHelper.DebugHelpers.IObjectToDbgString))
            {
                return true;
            }

            if (type == typeof(SymOntoClay.Core.DebugHelpers.IObjectToHumanizedString))
            {
                return true;
            }

            if (type == typeof(SymOntoClay.CoreHelper.ISymOntoClayDisposable))
            {
                return true;
            }

            if (type == typeof(System.IDisposable))
            {
                return true;
            }

            if (type == typeof(SymOntoClay.Core.Internal.CodeModel.Value))
            {
                return true;
            }

            if (type == typeof(SymOntoClay.Core.Internal.CodeModel.AnnotatedItem))
            {
                return true;
            }

            if (type == typeof(SymOntoClay.Core.Internal.CodeModel.ItemWithLongHashCodes))
            {
                return true;
            }

            return false;
        }

        private bool NCanConvert(Type source, Type dest)
        {
            if (_convertorsDict.ContainsKey(source))
            {
                var targetDict = _convertorsDict[source];

                if (targetDict.ContainsKey(dest))
                {
                    return true;
                }
            }

            if (source == dest)
            {
                return true;
            }

            if (dest.IsAssignableFrom(source))
            {
                return true;
            }

            if (dest.IsGenericType && dest.FullName.StartsWith("System.Nullable"))
            {
                return CanConvert(source, dest.GenericTypeArguments[0]);
            }

            return false;
        }

        /// <inheritdoc/>
        public object Convert(Type sourceType, Type destType, object sourceValue, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
            if (sourceType == _nullValueType)
            {
                return null;
            }

            lock (_lockObj)
            {
#if DEBUG
                //Log($"sourceType.FullName = {sourceType.FullName}");
                //Log($"destType.FullName = {destType.FullName}");
                //Log($"sourceValue = {sourceValue}");
#endif

                var nConvertResult = NConvert(sourceType, destType, sourceValue, context, localContext);

                if(nConvertResult.Item2)
                {
                    return nConvertResult.Item1;
                }

                var baseSourceTypes = ObjectHelper.GetAllBaseTypes(sourceType);

                foreach (var baseSourceType in baseSourceTypes)
                {
#if DEBUG
                    //Log($"baseSourceType.FullName = {baseSourceType.FullName}");
#endif

                    nConvertResult = NConvert(baseSourceType, destType, sourceValue, context, localContext);

                    if (nConvertResult.Item2)
                    {
                        return nConvertResult.Item1;
                    }
                }

                var baseDestTypes = ObjectHelper.GetAllBaseTypes(destType);

                foreach (var baseDestType in baseDestTypes)
                {
#if DEBUG
                    //Log($"baseDestType.FullName = {baseDestType.FullName}");
#endif

                    nConvertResult = NConvert(sourceType, baseDestType, sourceValue, context, localContext);

                    if (nConvertResult.Item2)
                    {
                        return nConvertResult.Item1;
                    }

                    foreach (var baseSourceType in baseSourceTypes)
                    {
#if DEBUG
                        //Log($"baseSourceType.FullName (2) = {baseSourceType.FullName}");
#endif

                        nConvertResult = NConvert(baseSourceType, baseDestType, sourceValue, context, localContext);

                        if (nConvertResult.Item2)
                        {
                            return nConvertResult.Item1;
                        }
                    }
                }

                return sourceValue;
            }
        }

        private (object, bool) NConvert(Type sourceType, Type destType, object sourceValue, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
#if DEBUG
            //Log($"sourceType.FullName = {sourceType.FullName}");
            //Log($"destType.FullName = {destType.FullName}");
            //Log($"sourceValue = {sourceValue}");
#endif

            if (_convertorsDict.ContainsKey(sourceType))
            {
                var targetDict = _convertorsDict[sourceType];

                if (targetDict.ContainsKey(destType))
                {
                    var convertor = targetDict[destType];

                    if (convertor.CoreType == sourceType)
                    {
                        return (convertor.ConvertToPlatformType(sourceValue, context, localContext), true);
                    }

                    return (convertor.ConvertToCoreType(sourceValue, context, localContext), true);
                }
            }

#if DEBUG
            //Log($"!_convertorsDict.ContainsKey(sourceType)");
#endif

            if (destType.IsGenericType && destType.FullName.StartsWith("System.Nullable"))
            {
                return NConvert(sourceType, destType.GenericTypeArguments[0], sourceValue, context, localContext);
            }

            return (sourceValue, false);
        }
    }
}
