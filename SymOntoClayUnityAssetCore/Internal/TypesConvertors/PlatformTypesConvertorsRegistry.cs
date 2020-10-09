/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
