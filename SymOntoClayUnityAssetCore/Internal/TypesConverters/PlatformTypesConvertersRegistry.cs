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

            if (type == typeof(SymOntoClay.Common.IObjectToString))
            {
                return true;
            }

            if (type == typeof(SymOntoClay.Common.IObjectToShortString))
            {
                return true;
            }

            if (type == typeof(SymOntoClay.Common.IObjectToBriefString))
            {
                return true;
            }

            if (type == typeof(SymOntoClay.Common.IObjectToDbgString))
            {
                return true;
            }

            if (type == typeof(SymOntoClay.Core.DebugHelpers.IObjectToHumanizedString))
            {
                return true;
            }

            if (type == typeof(SymOntoClay.Common.Disposing.ISymOntoClayDisposable))
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

        /// <inheritdoc/>
        public Value ConvertToValue(IMonitorLogger logger, Type sourceType, object sourceValue, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
#if DEBUG
            //logger.Info("FC273C9B-1CC9-4621-A2F0-2F43C8652748", $"sourceType.FullName = {sourceType.FullName}");
            //logger.Info("0C2C5887-CA2F-4202-8D98-B726461CA131", $"sourceValue = {sourceValue}");
#endif

            if(sourceValue == null)
            {
                return NullValue.Instance;
            }

            if (sourceType.IsGenericType && sourceType.FullName.StartsWith("System.Nullable"))
            {
#if DEBUG
                logger.Info("D33D3FE7-AC0C-498E-9189-CF73E089B37F", $"sourceType.GenericTypeArguments[0] = {sourceType.GenericTypeArguments[0]}");
#endif

                return NConvertToValue(logger, sourceType.GenericTypeArguments[0], sourceValue, context, localContext);
            }

            return NConvertToValue(logger, sourceType, sourceValue, context, localContext);
        }

        private Value NConvertToValue(IMonitorLogger logger, Type sourceType, object sourceValue, IEngineContext context, ILocalCodeExecutionContext localContext)
        {
#if DEBUG
            //logger.Info("BBE87F7A-E429-4428-A57D-2FE844A6138F", $"sourceType.FullName = {sourceType.FullName}");
            //logger.Info("6DB7DD53-4148-4F9F-8B2B-73510EBBEE8E", $"sourceValue = {sourceValue}");
#endif

            if (sourceType == typeof(float) || 
                sourceType == typeof(double) ||
                sourceType == typeof(decimal) ||
                sourceType == typeof(sbyte) ||
                sourceType == typeof(byte) ||
                sourceType == typeof(short) ||
                sourceType == typeof(ushort) ||
                sourceType == typeof(int) ||
                sourceType == typeof(uint) ||
                sourceType == typeof(long) ||
                sourceType == typeof(ulong))
            {
                return CreateNumberValue(System.Convert.ToDouble(sourceValue));
            }

            throw new NotImplementedException();
        }

        private NumberValue CreateNumberValue(double? systemValue)
        {
            var result = new NumberValue(systemValue);
            result.CheckDirty();
            return result;
        }
    }
}
