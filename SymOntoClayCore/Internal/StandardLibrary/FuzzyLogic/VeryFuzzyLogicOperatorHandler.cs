using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.StandardLibrary.FuzzyLogic
{
    public class VeryFuzzyLogicOperatorHandler: BaseFuzzyLogicOperatorHandler
    {
        /// <inheritdoc/>
        public override KindOfFuzzyLogicOperatorFunction Kind => KindOfFuzzyLogicOperatorFunction.Very;

        /// <inheritdoc/>
        public override double SystemCall(double x)
        {
            return SystemFuzzyLogicOperators.Very(x);
        }
    }
}
