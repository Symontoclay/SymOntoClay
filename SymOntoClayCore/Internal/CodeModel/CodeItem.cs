/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class CodeItem: AnnotatedItem, IMemberAccess, IReadOnlyMemberAccess
    {
        public abstract KindOfCodeEntity Kind { get;}
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

                OnNameChanged?.Invoke(value);
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

        public event Action<StrongIdentifierValue> OnNameChanged;

        private StrongIdentifierValue _name;
        private StrongIdentifierValue _holder;
        private TypeOfAccess _typeOfAccess = TypeOfAccess.Local;

        public virtual bool IsRuleInstance => false;
        public virtual RuleInstance AsRuleInstance => null;

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

        public virtual bool IsNamedFunction => false;
        public virtual NamedFunction AsNamedFunction => null;

        public virtual bool IsField => false;
        public virtual Field AsField => null;

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

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public abstract CodeItem CloneCodeItem();

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="cloneContext">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public abstract CodeItem CloneCodeItem(Dictionary<object, object> cloneContext);

        protected void AppendCodeItem(CodeItem source, Dictionary<object, object> cloneContext)
        {
            Name = source.Name?.Clone(cloneContext);
            InheritanceItems = source.InheritanceItems?.Select(p => p.Clone(cloneContext)).ToList();

            CodeFile = source.CodeFile;
            ParentCodeEntity = source.ParentCodeEntity;
            SubItems = source.SubItems?.Select(p => p.CloneCodeItem(cloneContext)).ToList();

            TypeOfAccess = source.TypeOfAccess;

            if (source.Holder == null)
            {
                Holder = null;
            }
            else
            {
                Holder = source.Holder;
            }

            AppendAnnotations(source, cloneContext);
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
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

            sb.PrintObjProp(n, nameof(Name), Name);

            sb.PrintObjListProp(n, nameof(InheritanceItems), InheritanceItems);

            sb.PrintBriefObjProp(n, nameof(CodeFile), CodeFile);
            sb.PrintBriefObjProp(n, nameof(ParentCodeEntity), ParentCodeEntity);
            sb.PrintObjListProp(n, nameof(SubItems), SubItems);

            sb.AppendLine($"{spaces}{nameof(TypeOfAccess)} = {TypeOfAccess}");

            sb.PrintObjProp(n, nameof(Holder), Holder);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");

            sb.PrintShortObjProp(n, nameof(Name), Name);

            sb.PrintShortObjListProp(n, nameof(InheritanceItems), InheritanceItems);

            sb.PrintBriefObjProp(n, nameof(CodeFile), CodeFile);
            sb.PrintBriefObjProp(n, nameof(ParentCodeEntity), ParentCodeEntity);
            sb.PrintShortObjListProp(n, nameof(SubItems), SubItems);

            sb.AppendLine($"{spaces}{nameof(TypeOfAccess)} = {TypeOfAccess}");

            sb.PrintShortObjProp(n, nameof(Holder), Holder);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.PrintBriefObjProp(n, nameof(Name), Name);
            sb.AppendLine($"{spaces}{nameof(TypeOfAccess)} = {TypeOfAccess}");

            sb.PrintBriefObjProp(n, nameof(Holder), Holder);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
