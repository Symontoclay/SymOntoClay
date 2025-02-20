/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Monitor.Common;
using System;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class ValueResolvingHelper : BaseContextComponent
    {
        public ValueResolvingHelper(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            var dataResolversFactory = _context.DataResolversFactory;

            _varsResolver = dataResolversFactory.GetVarsResolver();
        }

        private readonly IMainStorageContext _context;
        private VarsResolver _varsResolver;

        [Obsolete("Implement compilation instead of this one")]
        public Value TryResolveFromVarOrExpr(IMonitorLogger logger, Value operand, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            if (operand.IsStrongIdentifierValue)
            {
                var identifier = operand.AsStrongIdentifierValue;

                if (identifier.KindOfName == KindOfName.Var || identifier.KindOfName == KindOfName.SystemVar)
                {
                    return _varsResolver.GetVarValue(logger, identifier, localCodeExecutionContext);
                }
            }

            if(operand.IsPointRefValue)
            {
                var pointRef = operand.AsPointRefValue;
                var leftOperand = pointRef.LeftOperand;
                var rightOperand = pointRef.RightOperand;

                if(leftOperand.IsHostValue)
                {
                    return operand;
                }

                leftOperand = TryResolveFromVarOrExpr(logger, leftOperand, localCodeExecutionContext);

                return leftOperand.GetMemberValue(logger, rightOperand.AsStrongIdentifierValue);
            }

            return operand;
        }

        public void CheckWrongValue(IMonitorLogger logger, Value operand)
        {
            if (operand.IsStrongIdentifierValue)
            {
                var identifier = operand.AsStrongIdentifierValue;

                if (identifier.KindOfName == KindOfName.Var || identifier.KindOfName == KindOfName.SystemVar)
                {
                    throw new NotSupportedException($"Unresolved value {operand.ToHumanizedString()}. {identifier.KindOfName} has to be resolved by command {nameof(OperationCode.LoadFromVar)}");
                }
            }

            if (operand.IsPointRefValue)
            {
                throw new NotSupportedException($"Unresolved value {operand.ToHumanizedString()}. {operand.KindOfValue} has to be resolved by some new command.");
            }
        }
    }
}
