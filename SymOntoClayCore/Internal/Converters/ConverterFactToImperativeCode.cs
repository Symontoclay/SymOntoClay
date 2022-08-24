using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Converters
{
    public class ConverterFactToImperativeCode : BaseLoggedComponent
    {
        public ConverterFactToImperativeCode(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IMainStorageContext _context;

        public CompiledFunctionBody Convert(RuleInstance ruleInstance)
        {
            var kindOfRuleInstance = ruleInstance.KindOfRuleInstance;

            switch(kindOfRuleInstance)
            {
                case KindOfRuleInstance.Fact:
                    return ConvertFact(ruleInstance);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfRuleInstance), kindOfRuleInstance, null);
            }
        }

        private CompiledFunctionBody ConvertFact(RuleInstance fact)
        {
            throw new NotImplementedException();
        }
    }
}
