/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using NLog;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.Core.Internal.Parsing.Internal.ExprLinking;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class LogicalQueryNode: AnnotatedItem, IAstNode
    {
        public KindOfLogicalQueryNode Kind { get; set; } = KindOfLogicalQueryNode.Unknown;
        public KindOfOperatorOfLogicalQueryNode KindOfOperator { get; set; } = KindOfOperatorOfLogicalQueryNode.Unknown;
        public StrongIdentifierValue Name { get; set; }
        public LogicalQueryNode Left { get; set; }
        public LogicalQueryNode Right { get; set; }
        public IList<LogicalQueryNode> ParamsList { get; set; }
        public bool IsGroup { get; set; }
        public Value Value { get; set; }
        public bool IsQuestion { get; set; }

        //public bool HasQuestionVars
        //{
        //    get
        //    {
        //        if(IsQuestion)
        //        {
        //            return true;
        //        }

        //        if(Left != null && Left.HasQuestionVars)
        //        {
        //            return true;
        //        }

        //        if(Right != null && Right.HasQuestionVars)
        //        {
        //            return true;
        //        }

        //        if (ParamsList != null && ParamsList.Any(p => p.HasQuestionVars))
        //        {
        //            return true;
        //        }

        //        return false;
        //    }
        //}

        IAstNode IAstNode.Left { get => Left; set => Left = (LogicalQueryNode)value; }
        IAstNode IAstNode.Right { get => Right; set => Right = (LogicalQueryNode)value; }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public LogicalQueryNode Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public LogicalQueryNode Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (LogicalQueryNode)context[this];
            }

            var result = new LogicalQueryNode();
            context[this] = result;

            result.Kind = Kind;
            result.KindOfOperator = KindOfOperator;
            result.Name = Name?.Clone(context);
            result.Left = Left?.Clone(context);
            result.Right = Right?.Clone(context);
            result.ParamsList = ParamsList?.Select(p => p.Clone(context)).ToList();
            result.IsGroup = IsGroup;
            result.Value = Value?.CloneValue(context);
            result.IsQuestion = IsQuestion;

            result.AppendAnnotations(this, context);

            return result;
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            Name?.DiscoverAllAnnotations(result);
            Left?.DiscoverAllAnnotations(result);
            Right?.DiscoverAllAnnotations(result);

            if(!ParamsList.IsNullOrEmpty())
            {
                foreach(var item in ParamsList)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }

            Value?.DiscoverAllAnnotations(result);
        }

        public void DiscoverAllInheritanceRelations(IList<LogicalQueryNode> result)
        {
            switch(Kind)
            {
                case KindOfLogicalQueryNode.Relation:
                    if(ParamsList.Count == 1)
                    {
                        var param = ParamsList.Single();

                        if(param.Kind == KindOfLogicalQueryNode.Entity)
                        {
                            result.Add(this);
                        }                        
                    }
                    break;

                case KindOfLogicalQueryNode.BinaryOperator:
                    Left.DiscoverAllInheritanceRelations(result);
                    Right.DiscoverAllInheritanceRelations(result);
                    break;

                case KindOfLogicalQueryNode.UnaryOperator:
                    Left.DiscoverAllInheritanceRelations(result);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Kind), Kind, string.Empty);
            }
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            sb.PrintObjProp(n, nameof(Name), Name);

            sb.PrintObjProp(n, nameof(Left), Left);
            sb.PrintObjProp(n, nameof(Right), Right);
            sb.PrintObjListProp(n, nameof(ParamsList), ParamsList);
            
            sb.AppendLine($"{spaces}{nameof(IsGroup)} = {IsGroup}");

            sb.PrintObjProp(n, nameof(Value), Value);

            sb.AppendLine($"{spaces}{nameof(IsQuestion)} = {IsQuestion}");

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            sb.PrintShortObjProp(n, nameof(Name), Name);

            sb.PrintShortObjProp(n, nameof(Left), Left);
            sb.PrintShortObjProp(n, nameof(Right), Right);
            sb.PrintShortObjListProp(n, nameof(ParamsList), ParamsList);

            sb.AppendLine($"{spaces}{nameof(IsGroup)} = {IsGroup}");

            sb.PrintShortObjProp(n, nameof(Value), Value);

            sb.AppendLine($"{spaces}{nameof(IsQuestion)} = {IsQuestion}");

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(Kind)} = {Kind}");
            sb.AppendLine($"{spaces}{nameof(KindOfOperator)} = {KindOfOperator}");

            sb.PrintBriefObjProp(n, nameof(Name), Name);

            sb.PrintExisting(n, nameof(Left), Left);
            sb.PrintExisting(n, nameof(Right), Right);
            sb.PrintExistingList(n, nameof(ParamsList), ParamsList);

            sb.AppendLine($"{spaces}{nameof(IsGroup)} = {IsGroup}");

            sb.PrintBriefObjProp(n, nameof(Value), Value);

            sb.AppendLine($"{spaces}{nameof(IsQuestion)} = {IsQuestion}");

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
