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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.CollectionsHelpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public abstract class FunctionValue : Value, IExecutable
    {
        public List<FunctionArgumentInfo> Arguments { get; set; } = new List<FunctionArgumentInfo>();

        /// <inheritdoc/>
        IList<IFunctionArgument> IExecutable.Arguments => _iArgumentsList;

        public List<AstStatement> Statements { get; set; } = new List<AstStatement>();

        /// <inheritdoc/>
        public CompiledFunctionBody CompiledFunctionBody { get; set; }

        /// <inheritdoc/>
        public CodeEntity CodeEntity { get; set; }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.FunctionValue;

        /// <inheritdoc/>
        public override bool IsFunctionValue => true;

        /// <inheritdoc/>
        public bool IsSystemDefined => false;

        /// <inheritdoc/>
        public ISystemHandler SystemHandler => null;

        /// <inheritdoc/>
        public override FunctionValue AsFunctionValue => this;

        private Dictionary<StrongIdentifierValue, FunctionArgumentInfo> _argumentsDict;

        private IList<IFunctionArgument> _iArgumentsList;

        /// <inheritdoc/>
        public bool ContainsArgument(StrongIdentifierValue name)
        {
            return _argumentsDict.ContainsKey(name);
        }

        public FunctionArgumentInfo GetArgument(StrongIdentifierValue name)
        {
            if(_argumentsDict.ContainsKey(name))
            {
                return _argumentsDict[name];
            }

            return null;
        }

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override ulong CalculateLongHashCode()
        {
            var result = base.CalculateLongHashCode();

            if (Arguments.IsNullOrEmpty())
            {
                _argumentsDict = new Dictionary<StrongIdentifierValue, FunctionArgumentInfo>();
                _iArgumentsList = new List<IFunctionArgument>();
            }
            else
            {
                _iArgumentsList = Arguments.Cast<IFunctionArgument>().ToList();

                _argumentsDict = Arguments.ToDictionary(p => p.Name, p => p);

                foreach (var argument in Arguments)
                {
                    result ^= argument.GetLongConditionalHashCode();
                }
            }

            result ^= CompiledFunctionBody.GetLongHashCode();

            return result;
        }

        /// <summary>
        /// Clones the instance and returns cloned instance.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public abstract FunctionValue CloneFunctionValue();

        /// <summary>
        /// Clones the instance using special context and returns cloned instance.
        /// </summary>
        /// <param name="context">Special context for providing references continuity.</param>
        /// <returns>Cloned instance.</returns>
        public abstract FunctionValue CloneFunctionValue(Dictionary<object, object> context);

        protected void AppendFuction(FunctionValue source, Dictionary<object, object> context)
        {
            Arguments = source.Arguments?.Select(p => p.Clone(context)).ToList();

            Statements = source.Statements.Select(p => p.CloneAstStatement(context)).ToList();

            CompiledFunctionBody = source.CompiledFunctionBody.Clone(context);

            CodeEntity = source.CodeEntity?.Clone(context);

            AppendAnnotations(this, context);
        }

        /// <inheritdoc/>
        public override void DiscoverAllAnnotations(IList<RuleInstance> result)
        {
            base.DiscoverAllAnnotations(result);

            if(!Arguments.IsNullOrEmpty())
            {
                foreach (var item in Arguments)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }

            if (!Statements.IsNullOrEmpty())
            {
                foreach (var item in Statements)
                {
                    item.DiscoverAllAnnotations(result);
                }
            }
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintObjListProp(n, nameof(Arguments), Arguments);

            sb.PrintObjListProp(n, nameof(Statements), Statements);
            sb.PrintObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.PrintBriefObjProp(n, nameof(CodeEntity), CodeEntity);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintShortObjListProp(n, nameof(Arguments), Arguments);

            sb.PrintShortObjListProp(n, nameof(Statements), Statements);
            sb.PrintShortObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.PrintBriefObjProp(n, nameof(CodeEntity), CodeEntity);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.PrintBriefObjListProp(n, nameof(Arguments), Arguments);

            sb.PrintBriefObjListProp(n, nameof(Statements), Statements);
            sb.PrintBriefObjProp(n, nameof(CompiledFunctionBody), CompiledFunctionBody);

            sb.PrintBriefObjProp(n, nameof(CodeEntity), CodeEntity);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}