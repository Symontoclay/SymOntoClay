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

using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class CodeItem: Value, IFilteredCodeItem, IMemberAccess, IReadOnlyMemberAccess
    {
        public static readonly TypeOfAccess DefaultTypeOfAccess = TypeOfAccess.Protected;
        
        public abstract KindOfCodeEntity Kind { get;}
        
        public bool IsAnonymous { get; set; }

        public StrongIdentifierValue Name
        { 
            get
            {
                return _name;
            }

            set
            {
                if(_name == value)
                {
                    return;
                }

                _name = value;

                EmitOnNameChangedHandlers(value);
            }
        }

        public List<InheritanceItem> InheritanceItems { get; set; } = new List<InheritanceItem>();
        public CodeFile CodeFile { get; set; }
        public CodeItem ParentCodeEntity { get; set; }
        public List<CodeItem> SubItems { get; set; } = new List<CodeItem>();

        public StrongIdentifierValue Holder
        { 
            get
            {
                return _holder;
            }

            set
            {
                if(_holder == value)
                {
                    return;
                }

                _holder = value;

                OnHolderChanged();
            }
        }

        /// <inheritdoc/>
        public TypeOfAccess TypeOfAccess 
        { 
            get
            {
                return _typeOfAccess;
            }

            set
            {
                if(_typeOfAccess == value)
                {
                    return;
                }

                _typeOfAccess = value;

                OnTypeOfAccessChanged();
            }
        }

        protected virtual void OnHolderChanged()
        {
        }

        protected virtual void OnTypeOfAccessChanged()
        {
        }

        public void AddOnNameChangedHandler(IOnNameChangedCodeItemHandler handler)
        {
            lock (_onNameChangedHandlersLockObj)
            {
                if (_onNameChangedHandlers.Contains(handler))
                {
                    return;
                }

                _onNameChangedHandlers.Add(handler);
            }
        }

        public void RemoveOnNameChangedHandler(IOnNameChangedCodeItemHandler handler)
        {
            lock (_onNameChangedHandlersLockObj)
            {
                if (_onNameChangedHandlers.Contains(handler))
                {
                    _onNameChangedHandlers.Remove(handler);
                }
            }
        }

        private void EmitOnNameChangedHandlers(StrongIdentifierValue value)
        {
            lock (_onNameChangedHandlersLockObj)
            {
                foreach (var handler in _onNameChangedHandlers)
                {
                    handler.Invoke(value);
                }
            }
        }

        private object _onNameChangedHandlersLockObj = new object();
        private List<IOnNameChangedCodeItemHandler> _onNameChangedHandlers = new List<IOnNameChangedCodeItemHandler>();

        private StrongIdentifierValue _name;
        private StrongIdentifierValue _holder;
        private TypeOfAccess _typeOfAccess = TypeOfAccess.Local;

        public List<CodeItemDirective> Directives { get; private set; } = new List<CodeItemDirective>();

        public List<ActivatingItem> ActivatingConditions { get; private set; } = new List<ActivatingItem>();
        public List<RuleInstance> DeactivatingConditions { get; private set; } = new List<RuleInstance>();

        public List<IdleActionItem> IdleActionItems { get; private set; } = new List<IdleActionItem>();

        public Value Priority { get; set; }

        public List<StrongIdentifierValue> ImportsList { get; private set; } = new List<StrongIdentifierValue>();

        public virtual bool IsApp => false;
        public virtual App AsApp => null;

        public virtual bool IsAppInstanceCodeItem => false;
        public virtual AppInstanceCodeItem AsAppInstanceCodeItem => null;

        public virtual bool IsAction => false;
        public virtual ActionDef AsAction => null;

        public virtual bool IsState => false;
        public virtual StateDef AsState => null;

        public virtual bool IsOperator => false;
        public virtual Operator AsOperator => null;

        public virtual bool IsInlineTrigger => false;
        public virtual InlineTrigger AsInlineTrigger => null;

        public virtual bool IsLinguisticVariable => false;
        public virtual LinguisticVariable AsLinguisticVariable => null;

        public virtual bool IsFunction => false;
        public virtual Function AsFunction => null;

        public virtual bool IsNamedFunction => false;
        public virtual NamedFunction AsNamedFunction => null;

        public virtual bool IsConstructor => false;
        public virtual Constructor AsConstructor => null;

        public virtual bool IsPreConstructor => false;
        public virtual PreConstructor AsPreConstructor => null;

        public virtual bool IsField => false;
        public virtual Field AsField => null;

        public virtual bool IsProperty => false;
        public virtual Property AsProperty => null;

        public virtual bool IsMutuallyExclusiveStatesSet => false;
        public virtual MutuallyExclusiveStatesSet AsMutuallyExclusiveStatesSet => null;

        public virtual bool IsRelationDescription => false;
        public virtual RelationDescription AsRelationDescription => null;

        public virtual bool IsSynonym => false;
        public virtual Synonym AsSynonym => null;

        public virtual bool IsIdleActionItem => false;
        public virtual IdleActionItem AsIdleActionItem => null;

        public virtual bool IsBaseHtnTask => false;
        public virtual BaseHtnTask AsBaseHtnTask => null;

        public virtual bool IsBaseCompoundHtnTask => false;
        public virtual BaseCompoundHtnTask AsBaseCompoundHtnTask => null;

        public virtual bool IsRootHtnTask => false;
        public virtual RootHtnTask AsRootHtnTask => null;

        public virtual bool IsStrategicHtnTask => false;
        public virtual StrategicHtnTask AsStrategicHtnTask => null;

        public virtual bool IsTacticalHtnTask => false;
        public virtual TacticalHtnTask AsTacticalHtnTask => null;

        public virtual bool IsCompoundHtnTask => false;
        public virtual CompoundHtnTask AsCompoundHtnTask => null;

        public virtual bool IsReplacedCompoundHtnTask => false;
        public virtual ReplacedCompoundHtnTask AsReplacedCompoundHtnTask => null;

        public virtual bool IsCompoundTaskHtnCase => false;
        public virtual CompoundHtnTaskCase AsCompoundTaskHtnCase => null;

        public virtual bool IsBasePrimitiveHtnTask => false;
        public virtual BasePrimitiveHtnTask AsBasePrimitiveHtnTask => null;

        public virtual bool IsPrimitiveTask => false;
        public virtual PrimitiveHtnTask AsPrimitiveTask => null;

        public virtual bool IsBeginCompoundHtnTask => false;
        public virtual BeginCompoundHtnTask AsBeginCompoundHtnTask => null;

        public virtual bool IsEndCompoundHtnTask => false;
        public virtual EndCompoundHtnTask AsEndCompoundHtnTask => null;

        public virtual bool IsNopPrimitiveHtnTask => false;
        public virtual NopPrimitiveHtnTask AsNopPrimitiveHtnTask => null;

        public virtual bool IsJumpPrimitiveHtnTask => false;
        public virtual JumpPrimitiveHtnTask AsJumpPrimitiveHtnTask => null;

        public virtual bool IsIReturnable => false;
        public virtual IReturnable AsIReturnable => null;

        public virtual bool IsIVarDecl => false;
        public virtual IVarDecl AsIVarDecl => null;

        public virtual bool IsBaseExecutableExpression => false;
        public virtual BaseExecutableExpression AsBaseExecutableExpression => null;

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.CodeItem;

        /// <inheritdoc/>
        public override bool IsCodeItem => true;

        /// <inheritdoc/>
        public override CodeItem AsCodeItem => this;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            throw new NotImplementedException("85F04D55-E19D-4EEA-9AFD-3EE74792515B");
        }

        /// <inheritdoc/>
        public override string ToSystemString()
        {
            throw new NotImplementedException("AE189C64-304A-4894-B646-7D9D27F49DB4");
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode(CheckDirtyOptions options)
        {
            Name?.CheckDirty(options);

            var result = base.CalculateLongHashCode(options);

            if(Name != null)
            {
                result ^= Name.GetLongHashCode();
            }

            if (Holder != null)
            {
                result ^= LongHashCodeWeights.BaseModalityWeight ^ Holder.GetLongHashCode(options);
            }

            return result;
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneCodeItem(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> context)
        {
            return CloneCodeItem(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public CodeItem CloneCodeItem()
        {
            var context = new Dictionary<object, object>();
            return CloneCodeItem(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public abstract CodeItem CloneCodeItem(Dictionary<object, object> context);

        protected void AppendCodeItem(CodeItem source, Dictionary<object, object> context)
        {
            Name = source.Name?.Clone(context);
            InheritanceItems = source.InheritanceItems?.Select(p => p.Clone(context)).ToList();

            CodeFile = source.CodeFile;
            ParentCodeEntity = source.ParentCodeEntity;
            SubItems = source.SubItems?.Select(p => p.CloneCodeItem(context)).ToList();

            TypeOfAccess = source.TypeOfAccess;

            if (source.Holder == null)
            {
                Holder = null;
            }
            else
            {
                Holder = source.Holder;
            }

            Directives = source.Directives?.Select(p => p.CloneCodeItemDirective(context)).ToList();

            ActivatingConditions = source.ActivatingConditions?.Select(p => p.Clone(context)).ToList();
            DeactivatingConditions = source.DeactivatingConditions?.Select(p => p.Clone(context)).ToList();
            IdleActionItems = source.IdleActionItems?.Select(p => p.Clone(context)).ToList();

            Priority = source.Priority?.CloneValue(context);

            ImportsList = source.ImportsList?.Select(p => p.Clone(context)).ToList();

            AppendAnnotations(source, context);
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<Annotation> result)
        {
            base.DiscoverAllAnnotations(result);

            Name?.DiscoverAllAnnotations(result);

            if (!InheritanceItems.IsNullOrEmpty())
            {
                foreach (var item in InheritanceItems)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }

            if (!SubItems.IsNullOrEmpty())
            {
                foreach (var item in SubItems)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }

            if (Holder != null)
            {
                Holder.DiscoverAllAnnotations(result);
            }
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}"); 
            sb.AppendLine($"{spaces}{nameof(IsAnonymous)} = {IsAnonymous}");
            sb.PrintObjProp(n, nameof(Name), Name);

            sb.PrintObjListProp(n, nameof(InheritanceItems), InheritanceItems);

            sb.PrintBriefObjProp(n, nameof(CodeFile), CodeFile);
            sb.PrintBriefObjProp(n, nameof(ParentCodeEntity), ParentCodeEntity);
            sb.PrintObjListProp(n, nameof(SubItems), SubItems);

            sb.AppendLine($"{spaces}{nameof(TypeOfAccess)} = {TypeOfAccess}");

            sb.PrintObjProp(n, nameof(Holder), Holder);

            sb.PrintObjListProp(n, nameof(Directives), Directives);
            sb.PrintObjListProp(n, nameof(ActivatingConditions), ActivatingConditions); 
            sb.PrintObjListProp(n, nameof(DeactivatingConditions), DeactivatingConditions);
            sb.PrintObjListProp(n, nameof(IdleActionItems), IdleActionItems);

            sb.PrintObjProp(n, nameof(Priority), Priority);

            sb.PrintObjListProp(n, nameof(ImportsList), ImportsList);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(IsAnonymous)} = {IsAnonymous}");
            sb.PrintShortObjProp(n, nameof(Name), Name);

            sb.PrintShortObjListProp(n, nameof(InheritanceItems), InheritanceItems);

            sb.PrintBriefObjProp(n, nameof(CodeFile), CodeFile);
            sb.PrintBriefObjProp(n, nameof(ParentCodeEntity), ParentCodeEntity);
            sb.PrintShortObjListProp(n, nameof(SubItems), SubItems);

            sb.AppendLine($"{spaces}{nameof(TypeOfAccess)} = {TypeOfAccess}");

            sb.PrintShortObjProp(n, nameof(Holder), Holder);

            sb.PrintShortObjListProp(n, nameof(Directives), Directives);
            sb.PrintShortObjListProp(n, nameof(ActivatingConditions), ActivatingConditions);
            sb.PrintShortObjListProp(n, nameof(DeactivatingConditions), DeactivatingConditions);
            sb.PrintShortObjListProp(n, nameof(IdleActionItems), IdleActionItems);

            sb.PrintShortObjProp(n, nameof(Priority), Priority);

            sb.PrintShortObjListProp(n, nameof(ImportsList), ImportsList);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(IsAnonymous)} = {IsAnonymous}");
            sb.PrintBriefObjProp(n, nameof(Name), Name);
            sb.AppendLine($"{spaces}{nameof(TypeOfAccess)} = {TypeOfAccess}");

            sb.PrintBriefObjProp(n, nameof(Holder), Holder);

            sb.PrintBriefObjListProp(n, nameof(Directives), Directives);
            sb.PrintBriefObjListProp(n, nameof(ActivatingConditions), ActivatingConditions);
            sb.PrintBriefObjListProp(n, nameof(DeactivatingConditions), DeactivatingConditions);
            sb.PrintBriefObjListProp(n, nameof(IdleActionItems), IdleActionItems);

            sb.PrintBriefObjProp(n, nameof(Priority), Priority);

            sb.PrintBriefObjListProp(n, nameof(ImportsList), ImportsList);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
