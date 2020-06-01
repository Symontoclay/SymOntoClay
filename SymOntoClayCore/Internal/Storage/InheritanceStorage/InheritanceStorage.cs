﻿using Newtonsoft.Json;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.InheritanceStorage
{
    public class InheritanceStorage: BaseLoggedComponent, IInheritanceStorage
    {
        public InheritanceStorage(KindOfStorage kind, RealStorageContext realStorageContext)
            : base(realStorageContext.Logger)
        {
            _kind = kind;
            _realStorageContext = realStorageContext;
        }

        private readonly object _lockObj = new object();

        private readonly KindOfStorage _kind;

        public KindOfStorage Kind => _kind;

        private readonly RealStorageContext _realStorageContext;

        private readonly Dictionary<SimpleName, Dictionary<SimpleName, List<InheritanceItem>>> _nonIndexedInfo = new Dictionary<SimpleName, Dictionary<SimpleName, List<InheritanceItem>>>();
        private readonly Dictionary<ulong, Dictionary<ulong, List<InheritanceItem>>> _indexedInfo = new Dictionary<ulong, Dictionary<ulong, List<InheritanceItem>>>();

        public void SetInheritance(Name subItem, InheritanceItem inheritanceItem)
        {
            SetInheritance(subItem, inheritanceItem, true);
        }

        public void SetInheritance(Name subItem, InheritanceItem inheritanceItem, bool isPrimary)
        {
            lock(_lockObj)
            {
#if DEBUG
                Log($"subItem = {subItem}");
                Log($"inheritanceItem = {inheritanceItem}");
                Log($"isPrimary = {isPrimary}");

                var subItemSimpleNames = NameHelpers.CreateSimpleNames(subItem, _realStorageContext.EntityDictionary);

                Log($"subItemSimpleNames = {subItemSimpleNames.WriteListToString()}");
#endif

                var superItemSimpleNames = NameHelpers.CreateSimpleNames(inheritanceItem.Name, _realStorageContext.EntityDictionary);

                if (subItemSimpleNames.IsNullOrEmpty() || superItemSimpleNames.IsNullOrEmpty())
                {
                    return;
                }

                foreach (var subItemSimpleName in subItemSimpleNames)
                {
#if DEBUG
                    Log($"subItemSimpleName = {subItemSimpleName}");
#endif

                    foreach(var superItemSimpleName in superItemSimpleNames)
                    {
#if DEBUG
                        Log($"superItemSimpleName = {superItemSimpleName}");
#endif

                        var subKey = subItemSimpleName.FullNameKey;
                        var superKey = superItemSimpleName.FullNameKey;

                        if (_nonIndexedInfo.ContainsKey(subItemSimpleName))
                        {
                            var dict = _nonIndexedInfo[subItemSimpleName];
                            var indexedDict = _indexedInfo[subKey];

                            if (dict.ContainsKey(superItemSimpleName))
                            {
                                dict[superItemSimpleName].Add(inheritanceItem);
                                indexedDict[superKey].Add(inheritanceItem);
                            }
                            else
                            {
                                dict[superItemSimpleName] = new List<InheritanceItem>() { inheritanceItem };
                                indexedDict[superKey] = new List<InheritanceItem>() { inheritanceItem };
                            }
                        }
                        else
                        {
                            var dict = new Dictionary<SimpleName, List<InheritanceItem>>();
                            _nonIndexedInfo[subItemSimpleName] = dict;
                            dict[superItemSimpleName] = new List<InheritanceItem>() { inheritanceItem };

                            var indexedDict = new Dictionary<ulong, List<InheritanceItem>>();
                            _indexedInfo[subKey] = indexedDict;
                            indexedDict[superKey] = new List<InheritanceItem>() { inheritanceItem };
                        }
                    }
                }

#if DEBUG
                //Log($"_nonIndexedInfo = {JsonConvert.SerializeObject(_nonIndexedInfo, Formatting.Indented)}");
                //Log($"_indexedInfo = {JsonConvert.SerializeObject(_indexedInfo, Formatting.Indented)}");
#endif
            }

            if(isPrimary)
            {
                var inheritanceFact = CreateInheritanceFact(subItem, inheritanceItem);

#if DEBUG
                Log($"inheritanceFact = {inheritanceFact}");
#endif

                _realStorageContext.LogicalStorage.Append(inheritanceFact, false);
            }

#if DEBUG
            Log("End");
#endif
        }

        private RuleInstance CreateInheritanceFact(Name subItem, InheritanceItem inheritanceItem)
        {
            throw new NotImplementedException();
        }
    }
}
