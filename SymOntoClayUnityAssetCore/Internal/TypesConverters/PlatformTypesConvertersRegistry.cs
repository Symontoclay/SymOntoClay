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
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.TypesConverters
{
    public class PlatformTypesConvertersRegistry : BaseLoggedComponent, IPlatformTypesConvertersRegistry
    {
        public PlatformTypesConvertersRegistry(IMonitorLogger logger)
            : base(logger)
        {
        }

        private readonly Type _nullValueType = typeof(NullValue);

        private readonly object _lockObj = new object();
        private readonly Dictionary<Type, Dictionary<Type, IPlatformTypesConverter>> _convertorsDict = new Dictionary<Type, Dictionary<Type, IPlatformTypesConverter>>();

        /// <inheritdoc/>
        public void AddConvertor(IMonitorLogger logger, IPlatformTypesConverter convertor)
        {
            lock (_lockObj)
            {
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
        public bool CanConvert(IMonitorLogger logger, Type source, Type dest)
        {
            lock (_lockObj)
            {
                if (source == _nullValueType && (dest.IsClass || dest.IsInterface || (dest.IsGenericType && dest.FullName.StartsWith("System.Nullable"))))
                {
                    return true;
                }

                if(NCanConvert(logger, source, dest))
                {
                    return true;
                }

                var baseSourceTypes = ObjectHelper.GetAllBaseTypes(source).Where(p => !IsTooGeneralType(logger, p)).ToList();

                foreach (var baseSourceType in baseSourceTypes)
                {
                    if (NCanConvert(logger, baseSourceType, dest))
                    {
                        return true;
                    }
                }

                var baseDestTypes = ObjectHelper.GetAllBaseTypes(dest).Where(p => !IsTooGeneralType(logger, p)).ToList();

                foreach(var baseDestType in baseDestTypes)
                {
                    if (NCanConvert(logger, source, baseDestType))
                    {
                        return true;
                    }

                    foreach (var baseSourceType in baseSourceTypes)
                    {
                        if (NCanConvert(logger, baseSourceType, baseDestType))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        private bool IsTooGeneralType(IMonitorLogger logger, Type type)
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

        private bool NCanConvert(IMonitorLogger logger, Type source, Type dest)
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
                return CanConvert(logger, source, dest.GenericTypeArguments[0]);
            }

            return false;
        }

        /// <inheritdoc/>
        public object Convert(IMonitorLogger logger, Type sourceType, Type destType, object sourceValue, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
            if (sourceType == _nullValueType)
            {
                return null;
            }

            lock (_lockObj)
            {
                var nConvertResult = NConvert(logger, sourceType, destType, sourceValue, context, localContext);

                if(nConvertResult.Item2)
                {
                    return nConvertResult.Item1;
                }

                var baseSourceTypes = ObjectHelper.GetAllBaseTypes(sourceType);

                foreach (var baseSourceType in baseSourceTypes)
                {
                    nConvertResult = NConvert(logger, baseSourceType, destType, sourceValue, context, localContext);

                    if (nConvertResult.Item2)
                    {
                        return nConvertResult.Item1;
                    }
                }

                var baseDestTypes = ObjectHelper.GetAllBaseTypes(destType);

                foreach (var baseDestType in baseDestTypes)
                {
                    nConvertResult = NConvert(logger, sourceType, baseDestType, sourceValue, context, localContext);

                    if (nConvertResult.Item2)
                    {
                        return nConvertResult.Item1;
                    }

                    foreach (var baseSourceType in baseSourceTypes)
                    {
                        nConvertResult = NConvert(logger, baseSourceType, baseDestType, sourceValue, context, localContext);

                        if (nConvertResult.Item2)
                        {
                            return nConvertResult.Item1;
                        }
                    }
                }

                return sourceValue;
            }
        }

        private (object, bool) NConvert(IMonitorLogger logger, Type sourceType, Type destType, object sourceValue, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
            if (_convertorsDict.ContainsKey(sourceType))
            {
                var targetDict = _convertorsDict[sourceType];

                if (targetDict.ContainsKey(destType))
                {
                    var convertor = targetDict[destType];

                    if (convertor.CoreType == sourceType)
                    {
                        return (convertor.ConvertToPlatformType(logger, sourceValue, context, localContext), true);
                    }

                    return (convertor.ConvertToCoreType(logger, sourceValue, context, localContext), true);
                }
            }

            if (destType.IsGenericType && destType.FullName.StartsWith("System.Nullable"))
            {
                return NConvert(logger, sourceType, destType.GenericTypeArguments[0], sourceValue, context, localContext);
            }

            return (sourceValue, false);
        }
    }
}
