using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.ActionsStorage
{
    public class ActionsStorage: BaseLoggedComponent, IActionsStorage
    {
        public ActionsStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.MainStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;
        }

        private readonly object _lockObj = new object();

        private readonly KindOfStorage _kind;

        /// <inheritdoc/>
        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        /// <inheritdoc/>
        public IStorage Storage => _realStorageContext.Storage;

        private readonly Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, Dictionary<int, List<ActionPtr>>>> _actionsDict = new Dictionary<StrongIdentifierValue, Dictionary<StrongIdentifierValue, Dictionary<int, List<ActionPtr>>>>();

        /// <inheritdoc/>
        public void Append(ActionDef action)
        {
            lock (_lockObj)
            {
#if DEBUG
                Log($"action = {action}");
#endif

                AnnotatedItemHelper.CheckAndFillUpHolder(action, _realStorageContext.MainStorageContext.CommonNamesStorage);

                action.CheckDirty();

                var namesList = action.NamesList;

#if DEBUG
                Log($"namesList = {namesList.WriteListToString()}");
#endif

                var paramsCountList = GetParamsCountList(action);

#if DEBUG
                Log($"paramsCountList = {paramsCountList.WritePODListToString()}");
#endif

                throw new NotImplementedException();
            }
        }

        private List<int> GetParamsCountList(ActionDef action)
        {
            var result = new List<int>();

            var argumentsList = action.Operators.SelectMany(p => p.Arguments).ToList();

#if DEBUG
            Log($"argumentsList.Count = {argumentsList.Count}");
#endif

            if (!argumentsList.Any())
            {
                result.Add(0);
                return result;
            }

            var totalCount = argumentsList.Count();
            var argumentsWithoutDefaultValueCount = argumentsList.Count(p => !p.HasDefaultValue);

            if (totalCount == argumentsWithoutDefaultValueCount)
            {
                result.Add(totalCount);
                return result;
            }

            for (var i = argumentsWithoutDefaultValueCount; i <= totalCount; i++)
            {
                result.Add(i);
            }

            return result;
        }
    }
}
