/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class VarsResolver : BaseResolver
    {
        public VarsResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public IndexedValue GetVarValue(IndexedStrongIdentifierValue varName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"varName = {varName}");
#endif

            if(varName.KindOfName == KindOfName.SystemVar)
            {
                return GetSystemVarValue(varName, localCodeExecutionContext, options);
            }

            return GetUsualVarValue(varName, localCodeExecutionContext, options);
        }

        private IndexedValue GetSystemVarValue(IndexedStrongIdentifierValue varName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"varName = {varName}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
#endif

            foreach (var storageItem in storagesList)
            {
#if DEBUG
                //Log($"storageItem.Key = {storageItem.Key}; storageItem.Value.Kind = '{storageItem.Value.Kind}'");
#endif

                var targetValue = storageItem.Storage.VarStorage.GetSystemValueDirectly(varName);

#if DEBUG
                //Log($"targetValue = {targetValue}");
#endif

                if(targetValue != null)
                {
                    return targetValue;
                }
            }

            throw new NotImplementedException();
        }

        private IndexedValue GetUsualVarValue(IndexedStrongIdentifierValue varName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Log($"varName = {varName}");
#endif

            var storage = localCodeExecutionContext.Storage;

            var storagesList = GetStoragesList(storage);

#if DEBUG
            //Log($"storagesList.Count = {storagesList.Count}");
#endif

            foreach (var storageItem in storagesList)
            {
#if DEBUG
                //Log($"storageItem.Key = {storageItem.Key}; storageItem.Value.Kind = '{storageItem.Value.Kind}'");
#endif

                var targetValue = storageItem.Storage.VarStorage.GetValueDirectly(varName);

#if DEBUG
                //Log($"targetValue = {targetValue}");
#endif

                if (targetValue != null)
                {
                    return targetValue;
                }
            }

            return new NullValue().GetIndexedValue(_context);
        }
    }
}
