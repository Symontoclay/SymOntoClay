using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SymOntoClay.Core.Internal.Compiling.Internal.Helpers
{
    public static class KindOfCompilePushValHelper
    {
#if DEBUG
        //private static readonly NLog.ILogger _globalLogger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public static List<InternalKindOfCompilePushVal> ConvertToInternalItems(KindOfCompilePushVal kindOfCompilePushVal)
        {
#if DEBUG
            //_globalLogger.Info($"kindOfCompilePushVal = {kindOfCompilePushVal:G} ({kindOfCompilePushVal:D})");
#endif

            Validate(kindOfCompilePushVal);

            var result = new List<InternalKindOfCompilePushVal>();

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.DirectAllCases))
            {
                result.Add(InternalKindOfCompilePushVal.DirectVar);
                result.Add(InternalKindOfCompilePushVal.DirectProp);
            }

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.SetAllCases))
            {
                result.Add(InternalKindOfCompilePushVal.SetVar);
                result.Add(InternalKindOfCompilePushVal.SetProp);
            }

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.GetAllCases))
            {
                result.Add(InternalKindOfCompilePushVal.GetVar);
                result.Add(InternalKindOfCompilePushVal.GetProp);
            }

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.DirectVar))
            {
                result.Add(InternalKindOfCompilePushVal.DirectVar);
            }

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.SetVar))
            {
                result.Add(InternalKindOfCompilePushVal.SetVar);
            }

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.GetVar))
            {
                result.Add(InternalKindOfCompilePushVal.GetVar);
            }

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.DirectProp))
            {
                result.Add(InternalKindOfCompilePushVal.DirectProp);
            }

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.SetProp))
            {
                result.Add(InternalKindOfCompilePushVal.SetProp);
            }

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.GetProp))
            {
                result.Add(InternalKindOfCompilePushVal.GetProp);
            }

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.DirectOther) ||
                kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.SetOther) ||
                kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.GetOther))
            {
                var internalKindOfItemList = GetInternalKindOfItem(result);

#if DEBUG
                //_globalLogger.Info($"internalKindOfItemList = {JsonConvert.SerializeObject(internalKindOfItemList.Select(p => p.ToString()), Formatting.Indented)}");
#endif

                if (internalKindOfItemList.Any())
                {
                    foreach (var item in internalKindOfItemList) 
                    {
                        switch(item)
                        {
                            case InternalKindOfCompilePushValItem.Variable:
                                if(kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.DirectOther))
                                {
                                    result.Add(InternalKindOfCompilePushVal.DirectVar);
                                }

                                if(kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.SetOther))
                                {
                                    result.Add(InternalKindOfCompilePushVal.SetVar);
                                }

                                if(kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.GetOther))
                                {
                                    result.Add(InternalKindOfCompilePushVal.GetVar);
                                }
                                break;

                            case InternalKindOfCompilePushValItem.Property:
                                if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.DirectOther))
                                {
                                    result.Add(InternalKindOfCompilePushVal.DirectProp);
                                }

                                if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.SetOther))
                                {
                                    result.Add(InternalKindOfCompilePushVal.SetProp);
                                }

                                if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.GetOther))
                                {
                                    result.Add(InternalKindOfCompilePushVal.GetProp);
                                }
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(item), item, null);
                        }
                    }
                }
            }

            Validate(result, kindOfCompilePushVal);

            return result;
        }

        private static List<InternalKindOfCompilePushValItem> GetInternalKindOfItem(List<InternalKindOfCompilePushVal> existingItems)
        {
#if DEBUG
            //_globalLogger.Info($"existingItems = {JsonConvert.SerializeObject(existingItems.Select(p => p.ToString()), Formatting.Indented)}");
#endif

            var result = new List<InternalKindOfCompilePushValItem> { InternalKindOfCompilePushValItem.Variable, InternalKindOfCompilePushValItem.Property };

            if (existingItems.Contains(InternalKindOfCompilePushVal.DirectVar) ||
                existingItems.Contains(InternalKindOfCompilePushVal.SetVar) ||
                existingItems.Contains(InternalKindOfCompilePushVal.GetVar))
            {
                result.Remove(InternalKindOfCompilePushValItem.Variable);
            }

            if (existingItems.Contains(InternalKindOfCompilePushVal.DirectProp) ||
                existingItems.Contains(InternalKindOfCompilePushVal.SetProp) ||
                existingItems.Contains(InternalKindOfCompilePushVal.GetProp))
            {
                result.Remove(InternalKindOfCompilePushValItem.Property);
            }

            return result;
        }

        private static void Validate(List<InternalKindOfCompilePushVal> items, KindOfCompilePushVal kindOfCompilePushVal)
        {
            var userVar = new List<InternalKindOfCompilePushVal>();

            if (items.Contains(InternalKindOfCompilePushVal.DirectVar))
            {
                userVar.Add(InternalKindOfCompilePushVal.DirectVar);
            }

            if (items.Contains(InternalKindOfCompilePushVal.SetVar))
            {
                userVar.Add(InternalKindOfCompilePushVal.SetVar);
            }

            if (items.Contains(InternalKindOfCompilePushVal.GetVar))
            {
                userVar.Add(InternalKindOfCompilePushVal.GetVar);
            }

#if DEBUG
            //_globalLogger.Info($"userVar.Count = {userVar.Count}");
#endif

            if (userVar.Count > 1)
            {
                throw new Exception($"The options {string.Join(", ", userVar.Select(p => p.ToString()))} can not be used together. Original flags: {kindOfCompilePushVal:G} ({kindOfCompilePushVal:D})");
            }

            var usedProp = new List<InternalKindOfCompilePushVal>();

            if (items.Contains(InternalKindOfCompilePushVal.DirectProp))
            {
                usedProp.Add(InternalKindOfCompilePushVal.DirectProp);
            }

            if (items.Contains(InternalKindOfCompilePushVal.SetProp))
            {
                usedProp.Add(InternalKindOfCompilePushVal.SetProp);
            }

            if (items.Contains(InternalKindOfCompilePushVal.GetProp))
            {
                usedProp.Add(InternalKindOfCompilePushVal.GetProp);
            }

#if DEBUG
            //_globalLogger.Info($"usedProp.Count = {usedProp.Count}");
#endif

            if (usedProp.Count > 1)
            {
                throw new Exception($"The options {string.Join(", ", usedProp.Select(p => p.ToString()))} can not be used together. Original flags: {kindOfCompilePushVal:G} ({kindOfCompilePushVal:D})");
            }
        }

        private static void Validate(KindOfCompilePushVal kindOfCompilePushVal)
        {
#if DEBUG
            //_globalLogger.Info($"kindOfCompilePushVal = {kindOfCompilePushVal:G} ({kindOfCompilePushVal:D})");
#endif

            var usedAll = new List<KindOfCompilePushVal>();

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.DirectAllCases))
            {
                usedAll.Add(KindOfCompilePushVal.DirectAllCases);
            }

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.SetAllCases))
            {
                usedAll.Add(KindOfCompilePushVal.SetAllCases);
            }

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.GetAllCases))
            {
                usedAll.Add(KindOfCompilePushVal.GetAllCases);
            }

#if DEBUG
            //_globalLogger.Info($"usedAll.Count = {usedAll.Count}");
#endif

            if (usedAll.Count > 1)
            {
                throw new Exception($"The options {string.Join(", ", usedAll.Select(p => p.ToString()))} can not be used together.");
            }

            var usedOther = new List<KindOfCompilePushVal>();

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.DirectOther))
            {
                usedOther.Add(KindOfCompilePushVal.DirectOther);
            }

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.SetOther))
            {
                usedOther.Add(KindOfCompilePushVal.SetOther);
            }

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.GetOther))
            {
                usedOther.Add(KindOfCompilePushVal.GetOther);
            }

#if DEBUG
            //_globalLogger.Info($"usedOther.Count = {usedOther.Count}");
#endif

            if (usedOther.Count > 1)
            {
                throw new Exception($"The options {string.Join(", ", usedOther.Select(p => p.ToString()))} can not be used together.");
            }

            var userVar = new List<KindOfCompilePushVal>();

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.DirectVar))
            {
                userVar.Add(KindOfCompilePushVal.DirectVar);
            }

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.SetVar))
            {
                userVar.Add(KindOfCompilePushVal.SetVar);
            }

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.GetVar))
            {
                userVar.Add(KindOfCompilePushVal.GetVar);
            }

#if DEBUG
            //_globalLogger.Info($"userVar.Count = {userVar.Count}");
#endif

            if (userVar.Count > 1)
            {
                throw new Exception($"The options {string.Join(", ", userVar.Select(p => p.ToString()))} can not be used together.");
            }

            var usedProp = new List<KindOfCompilePushVal>();

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.DirectProp))
            {
                usedProp.Add(KindOfCompilePushVal.DirectProp);
            }

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.SetProp))
            {
                usedProp.Add(KindOfCompilePushVal.SetProp);
            }

            if (kindOfCompilePushVal.HasFlag(KindOfCompilePushVal.GetProp))
            {
                usedProp.Add(KindOfCompilePushVal.GetProp);
            }

#if DEBUG
            //_globalLogger.Info($"usedProp.Count = {usedProp.Count}");
#endif

            if (usedProp.Count > 1)
            {
                throw new Exception($"The options {string.Join(", ", usedProp.Select(p => p.ToString()))} can not be used together.");
            }
        }
    }
}
