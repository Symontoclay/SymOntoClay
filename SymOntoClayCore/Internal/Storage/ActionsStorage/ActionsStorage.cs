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

                var holder = action.Holder;

#if DEBUG
                Log($"holder = {holder}");
#endif

                var namesList = action.NamesList;

#if DEBUG
                Log($"namesList = {namesList.WriteListToString()}");
#endif

                var paramsCountDict = GetParamsCountDict(action);

                var paramsCountList = paramsCountDict.Keys.ToList();

#if DEBUG
                Log($"paramsCountList = {paramsCountList.WritePODListToString()}");
#endif

                foreach(var name in namesList)
                {
#if DEBUG
                    Log($"name = {name}");
#endif

                    var targetDict = GetDictByNames(holder, name);

                    foreach (var count in paramsCountList)
                    {
#if DEBUG
                        Log($"count = {count}");
#endif

                        var operatorsList = paramsCountDict[count];

                        List<ActionPtr> targetList = null;

                        if (targetDict.ContainsKey(count))
                        {
                            targetList = targetDict[count];
                        }
                        else
                        {
                            targetList = new List<ActionPtr>();
                            targetDict[count] = targetList;
                        }

                        foreach(var op in operatorsList)
                        {
#if DEBUG
                            Log($"op = {op}");
#endif

                            if (!targetList.Any(p => p.Action == action && p.Operator == op))
                            {
                                targetList.Add(new ActionPtr(action, op));
                            }
                        }
                    }
                }
            }
        }

        private Dictionary<int, List<Operator>> GetParamsCountDict(ActionDef action)
        {
            var result = new Dictionary<int, List<Operator>>();

            foreach(var op in action.Operators)
            {
#if DEBUG
                Log($"op = {op}");
#endif

                var paramsCountList = GetParamsCountList(op);

#if DEBUG
                Log($"paramsCountList = {paramsCountList.WritePODListToString()}");
#endif

                foreach(var count in paramsCountList)
                {
                    AddToDict(count, result, op);
                }
            }

            return result;
        }

        private void AddToDict(int paramCount, Dictionary<int, List<Operator>> dict, Operator @operator)
        {
            if(dict.ContainsKey(paramCount))
            {
                var list = dict[paramCount];

                if(!list.Contains(@operator))
                {
                    list.Add(@operator);
                }

                return;
            }

            dict[paramCount] = new List<Operator>() { @operator };
        }

        private List<int> GetParamsCountList(Operator @operator)
        {
            var result = new List<int>();

            var argumentsList = @operator.Arguments;

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

        private Dictionary<int, List<ActionPtr>> GetDictByNames(StrongIdentifierValue holder, StrongIdentifierValue name)
        {
            if (_actionsDict.ContainsKey(holder))
            {
                var dict = _actionsDict[holder];

                if (dict.ContainsKey(name))
                {
                    return dict[name];
                }

                {
                    var targetDict = new Dictionary<int, List<ActionPtr>>();
                    dict[name] = targetDict;
                    return targetDict;
                }
            }

            {
                var dict = new Dictionary<StrongIdentifierValue, Dictionary<int, List<ActionPtr>>>();
                _actionsDict[holder] = dict;
                var targetDict = new Dictionary<int, List<ActionPtr>>();
                dict[name] = targetDict;
                return targetDict;
            }
        }
    }
}
