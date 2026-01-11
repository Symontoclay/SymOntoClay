/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

namespace SymOntoClay.Core.Internal.CodeModel
{
    public enum KindOfCodeEntity
    {
        Unknown,
        World,
        App,
        Lib,
        Class,
        Action,
        AppInstance,
        Instance,
        InlineTrigger,
        Function,
        Constructor,
        PreConstructor,
        Operator,
        Channel,
        RuleOrFact,
        LinguisticVariable,
        Var,
        Field,
        Property,
        State,
        MutuallyExclusiveStatesSet,
        RelationDescription,
        Synonym,
        IdleActionItem,
        AnonymousObject,
        RootTask,
        StrategicTask,
        TacticalTask,
        CompoundTask,
        ReplacedCompoundTask,
        CompoundTaskCase,
        CompoundHtnTaskBefore,
        CompoundHtnTaskAfter,
        CompoundHtnTaskBackground,
        PrimitiveTask,
        BeginCompoundTask,
        EndCompoundTask,
        NopPrimitiveTask,
        JumpPrimitiveTask,
        ExecutableExpression,
        ExecutableCodeBlock
    }
}
