/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Internal.TypesConvertors
{
    public class PlatformTypesConvertorsRegistry : BaseLoggedComponent, IPlatformTypesConvertorsRegistry
    {
        public PlatformTypesConvertorsRegistry(IEntityLogger logger)
            : base(logger)
        {
        }

        private readonly object _lockObj = new object();
        private readonly Dictionary<Type, Dictionary<Type, IPlatformTypesConvertor>> _convertorsDict = new Dictionary<Type, Dictionary<Type, IPlatformTypesConvertor>>();

        /// <inheritdoc/>
        public void AddConvertor(IPlatformTypesConvertor convertor)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"convertor = {convertor}");
#endif

                Dictionary<Type, IPlatformTypesConvertor> targetDict = null;

                if (convertor.CanConvertToCoreType)
                {
                    if (_convertorsDict.ContainsKey(convertor.PlatformType))
                    {
                        targetDict = _convertorsDict[convertor.PlatformType];
                    }
                    else
                    {
                        targetDict = new Dictionary<Type, IPlatformTypesConvertor>();
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
                        targetDict = new Dictionary<Type, IPlatformTypesConvertor>();
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
#endif

                if (!_convertorsDict.ContainsKey(source))
                {
                    return false;
                }

                var targetDict = _convertorsDict[source];

                if (!targetDict.ContainsKey(dest))
                {
                    return false;
                }

                return true;
            }
        }

        /// <inheritdoc/>
        public object Convert(Type sourceType, Type destType, object sourceValue)
        {
            lock (_lockObj)
            {
#if DEBUG
                //Log($"sourceType.FullName = {sourceType.FullName}");
                //Log($"destType.FullName = {destType.FullName}");
                //Log($"sourceValue = {sourceValue}");
#endif

                var convertor = _convertorsDict[sourceType][destType];

#if DEBUG
                //Log($"convertor = {convertor}");
#endif

                if (convertor.CoreType == sourceType)
                {
                    return convertor.ConvertToPlatformType(sourceValue, Logger);
                }

                return convertor.ConvertToCoreType(sourceValue, Logger);
            }
        }
    }
}
