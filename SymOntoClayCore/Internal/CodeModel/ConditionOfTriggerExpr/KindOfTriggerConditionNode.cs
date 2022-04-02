using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr
{
    public enum KindOfTriggerConditionNode
    {
        Unknown,
        Concept,
        /// <summary>
        /// Represents entity identifier. Entity identifier always atarts from symbol '#'. For example, '#123', '#Piter', '#_123W12_1', '#`Tom Jones`'.
        /// </summary>
        Entity,
        EntityRef,
        EntityCondition,
        BinaryOperator,
        UnaryOperator,
        Value,
        Group,
        FuzzyLogicNonNumericSequence,
        Var,
        Fact,
        Duration,
        CallFunction
    }
}
