using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.Operators
{
    public class AssignOperatorHandler : BaseOperatorHandler, IBinaryOperatorHandler
    {
        public AssignOperatorHandler(IEngineContext engineContext)
            : base(engineContext)
        {
            var dataResolversFactory = engineContext.DataResolversFactory;

            _varsResolver = dataResolversFactory.GetVarsResolver();
        }

        private readonly VarsResolver _varsResolver;

        /// <inheritdoc/>
        public Value Call(Value rightOperand, Value leftOperand, Value annotation, LocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            //Log($"leftOperand = {leftOperand}");
            //Log($"rightOperand = {rightOperand}");
            //Log($"annotation = {annotation}");
#endif

            rightOperand = TryResolveFromVar(rightOperand, localCodeExecutionContext);

#if DEBUG
            //Log($"rightOperand (after) = {rightOperand}");
#endif

            var kindOfLeftOperand = leftOperand.KindOfValue;

            switch(kindOfLeftOperand)
            {
                case KindOfValue.StrongIdentifierValue:
                    {
                        var leftIdentifierValue = leftOperand.AsStrongIdentifierValue;

                        var kindOfNameOfLeftIdentifierValue = leftIdentifierValue.KindOfName;

                        switch(kindOfNameOfLeftIdentifierValue)
                        {
                            case KindOfName.Var:
                                {
                                    var kindOfRightOperand = rightOperand.KindOfValue;

                                    switch (kindOfRightOperand)
                                    {
                                        case KindOfValue.StrongIdentifierValue:
                                            throw new NotImplementedException();

                                        default:
                                            _varsResolver.SetVarValue(leftIdentifierValue, rightOperand, localCodeExecutionContext);
                                            return rightOperand;
                                    }
                                }

                            default:
                                throw new ArgumentOutOfRangeException(nameof(kindOfNameOfLeftIdentifierValue), kindOfNameOfLeftIdentifierValue, null);
                        }
                    }

                case KindOfValue.PointRefValue:
                    leftOperand.SetValue(rightOperand);
                    return rightOperand;

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfLeftOperand), kindOfLeftOperand, null);
            }

            throw new NotImplementedException();
        }
    }
}
