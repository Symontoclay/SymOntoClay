using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.LogicalStorage
{
    public class CommonPersistIndexedLogicalData: BaseLoggedComponent
    {
        public CommonPersistIndexedLogicalData(IEntityLogger logger)
            : base(logger)
        {
        }

        public void NSetIndexedRuleInstanceToIndexData(IndexedRuleInstance indexedRuleInstance)
        {
#if DEBUG
            Log($"indexedRuleInstance = {indexedRuleInstance}");
#endif

            IndexedRuleInstancesDict[indexedRuleInstance.Key] = indexedRuleInstance;

            var kind = indexedRuleInstance.Kind;

            switch (kind)
            {
                case KindOfRuleInstance.Fact:
                case KindOfRuleInstance.Annotation:
                case KindOfRuleInstance.EntityCondition:
                    {
                        if (indexedRuleInstance.IsPart_1_Active)
                        {
                            NAddIndexedRulePartToKeysOfRelationsIndex(IndexedRulePartsOfFactsDict, indexedRuleInstance.Part_1);
                        }
                        else
                        {
                            NAddIndexedRulePartToKeysOfRelationsIndex(IndexedRulePartsOfFactsDict, indexedRuleInstance.Part_2);
                        }
                    }
                    break;

                case KindOfRuleInstance.Rule:
                    {
                        var part_1 = indexedRuleInstance.Part_1;

                        if (part_1.HasVars && !part_1.HasQuestionVars && part_1.RelationsDict.Count == 1)
                        {
                            NAddIndexedRulePartToKeysOfRelationsIndex(IndexedRulePartsWithOneRelationWithVarsDict, part_1);
                        }

                        var part_2 = indexedRuleInstance.Part_2;

                        if (part_2.HasVars && !part_2.HasQuestionVars && part_2.RelationsDict.Count == 1)
                        {
                            NAddIndexedRulePartToKeysOfRelationsIndex(IndexedRulePartsWithOneRelationWithVarsDict, part_2);
                        }
                    }
                    break;

                //case KindOfRuleInstance.EntityCondition:
                //break;

                case KindOfRuleInstance.QuestionVars:
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }

            switch (kind)
            {
                case KindOfRuleInstance.Annotation:
                case KindOfRuleInstance.EntityCondition:
                    AdditionalRuleInstancesDict[indexedRuleInstance.Key] = indexedRuleInstance;
                    break;
            }

#if IMAGINE_WORKING
            Log("End");
#else
            throw new NotImplementedException();
#endif
        }
    }
}
