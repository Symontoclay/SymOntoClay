using NLog;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Storage.LogicalStorage
{
    public static class AddingFactHelper
    {
#if DEBUG
        //private static ILogger _gbcLogger = LogManager.GetCurrentClassLogger();
#endif

        public static IAddFactOrRuleResult CallEvent(MulticastDelegate onAddingFactEvent, RuleInstance ruleInstance, FuzzyLogicResolver fuzzyLogicResolver, LocalCodeExecutionContext localCodeExecutionContext, IEntityLogger logger)
        {
            var resultsOfCallList = new List<IAddFactOrRuleResult>();

            var rawObligationsModalitiesList = new List<Value>();
            var rawSelfObligationsModalitiesList = new List<Value>();

            foreach (var item in onAddingFactEvent.GetInvocationList())
            {
                try
                {
                    var rawResultOfCall = item.DynamicInvoke(ruleInstance);

#if DEBUG
                    //_gbcLogger.Info($"rawResultOfCall = {rawResultOfCall}");
#endif

                    if (rawResultOfCall == null)
                    {
                        continue;
                    }

                    var resultOfCall = (IAddFactOrRuleResult)rawResultOfCall;

                    if (resultOfCall.KindOfResult == KindOfAddFactOrRuleResult.Reject)
                    {
                        return resultOfCall;
                    }

                    resultsOfCallList.Add(resultOfCall);

                    var mutablePart = resultOfCall.MutablePart;

                    if (mutablePart != null)
                    {
                        rawObligationsModalitiesList.Add(mutablePart.ObligationModality);
                        rawSelfObligationsModalitiesList.Add(mutablePart.SelfObligationModality);
                    }
                }
                catch(Exception e)
                {
                    logger.Error(e.ToString());
                }
            }

            if (!resultsOfCallList.Any())
            {
                return null;
            }

            if (resultsOfCallList.Count == 1)
            {
                return resultsOfCallList.Single();
            }

            if (resultsOfCallList.All(p => p.MutablePart == null))
            {
                return resultsOfCallList.First();
            }

            var result = new AddFactOrRuleResult()
            {
                KindOfResult = KindOfAddFactOrRuleResult.Accept
            };

            var resultMutablePart = new MutablePartOfRuleInstance() { Parent = ruleInstance };
            result.MutablePart = resultMutablePart;

            resultMutablePart.ObligationModality = MathValueHelper.Max(rawObligationsModalitiesList.Select(p => fuzzyLogicResolver.Resolve(p, localCodeExecutionContext)));
            resultMutablePart.SelfObligationModality = MathValueHelper.Max(rawSelfObligationsModalitiesList.Select(p => fuzzyLogicResolver.Resolve(p, localCodeExecutionContext)));

            return result;
        }
    }
}
