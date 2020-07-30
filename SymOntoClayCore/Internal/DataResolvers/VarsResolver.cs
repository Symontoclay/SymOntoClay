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

            throw new NotImplementedException();
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

                var targetValue = storageItem.Value.VarStorage.GetSystemValueDirectly(varName);

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
    }
}
