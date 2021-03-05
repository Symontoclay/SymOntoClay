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

        public Value GetVarValue(StrongIdentifierValue varName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
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

        private Value GetSystemVarValue(StrongIdentifierValue varName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
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

        private Value GetUsualVarValue(StrongIdentifierValue varName, LocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
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

            return new NullValue();
        }
    }
}
