using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.DataResolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public abstract class BaseLogicalOperatorHandler: BaseLoggedComponent
    {
        protected BaseLogicalOperatorHandler(IEngineContext engineContext)
            : base(engineContext.Logger)
        {
            _engineContext = engineContext;

            var dataResolversFactory = engineContext.DataResolversFactory;

            _fuzzyLogicResolver = dataResolversFactory.GetFuzzyLogicResolver();
        }

        private readonly IEngineContext _engineContext;
        private readonly FuzzyLogicResolver _fuzzyLogicResolver;

        protected NumberValue ConvertOperandToNumberValue(Value operand, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            Log($"operand = {operand}");
#endif

            var kindOfValue = operand.KindOfValue;

            switch (kindOfValue)
            {
                case KindOfValue.NumberValue:
                    return operand.AsNumberValue;

                case KindOfValue.LogicalValue:
                    return ValueConvertor.ConvertLogicalValueToNumberValue(operand.AsLogicalValue, _engineContext);

                case KindOfValue.StrongIdentifierValue:
                    {
                        var val = operand.AsStrongIdentifierValue;

                        var normalizedNameValue = val.NormalizedNameValue;

                        switch(normalizedNameValue)
                        {
                            case "true":
                                return new NumberValue(1);

                            case "false":
                                return new NumberValue(0);

                            default:
                                return _fuzzyLogicResolver.Resolve(val, localCodeExecutionContext);
                        }                        
                    }                    

                case KindOfValue.FuzzyLogicNonNumericSequenceValue:
                    return _fuzzyLogicResolver.Resolve(operand.AsFuzzyLogicNonNumericSequenceValue, localCodeExecutionContext);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfValue), kindOfValue, null);
            }

            throw new NotImplementedException();
        }
    }
}
