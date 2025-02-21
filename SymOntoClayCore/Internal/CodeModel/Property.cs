using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;
using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Ast.Statements;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class Property : CodeItem
    {
        public Property()
        {
            TypeOfAccess = DefaultTypeOfAccess;
        }
        
        /// <inheritdoc/>
        public override KindOfCodeEntity Kind => KindOfCodeEntity.Property;

        /// <inheritdoc/>
        public override bool IsProperty => true;

        /// <inheritdoc/>
        public override Property AsProperty => this;

        public KindOfProperty KindOfProperty { get; set; } = KindOfProperty.Auto;

        public List<StrongIdentifierValue> TypesList { get; set; } = new List<StrongIdentifierValue>();

        public AstExpression DefaultValue { get; set; }

        public List<AstStatement> GetStatements { get; set; } = new List<AstStatement>();
        public CompiledFunctionBody GetCompiledFunctionBody { get; set; }
        
        /// <inheritdoc/>
        public override CodeItem CloneCodeItem(Dictionary<object, object> context)
        {
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="Clone"]/*' />
        public Property Clone()
        {
            var context = new Dictionary<object, object>();
            return Clone(context);
        }

        /// <include file = "..\CommonDoc.xml" path='extradoc/method[@name="CloneWithContext"]/*' />
        public Property Clone(Dictionary<object, object> context)
        {
            if (context.ContainsKey(this))
            {
                return (Property)context[this];
            }

            var result = new Property();
            context[this] = result;

            result.KindOfProperty = KindOfProperty;
            result.DefaultValue = DefaultValue?.CloneAstExpression(context);
            result.TypesList = TypesList?.Select(p => p.Clone(context)).ToList();

            result.GetStatements = GetStatements.Select(p => p.CloneAstStatement(context)).ToList();
            result.GetCompiledFunctionBody = GetCompiledFunctionBody?.Clone(context);

            result.AppendCodeItem(this, context);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{KindOfProperty} = {KindOfProperty}");

            sb.PrintObjProp(n, nameof(DefaultValue), DefaultValue);
            sb.PrintObjListProp(n, nameof(TypesList), TypesList);

            sb.PrintObjListProp(n, nameof(GetStatements), GetStatements);
            sb.PrintObjProp(n, nameof(GetCompiledFunctionBody), GetCompiledFunctionBody);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{KindOfProperty} = {KindOfProperty}");

            sb.PrintShortObjProp(n, nameof(DefaultValue), DefaultValue);
            sb.PrintShortObjListProp(n, nameof(TypesList), TypesList);

            sb.PrintShortObjListProp(n, nameof(GetStatements), GetStatements);
            sb.PrintShortObjProp(n, nameof(GetCompiledFunctionBody), GetCompiledFunctionBody);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            sb.AppendLine($"{spaces}{KindOfProperty} = {KindOfProperty}");

            sb.PrintBriefObjProp(n, nameof(DefaultValue), DefaultValue);
            sb.PrintBriefObjListProp(n, nameof(TypesList), TypesList);


            sb.PrintBriefObjListProp(n, nameof(GetStatements), GetStatements);
            sb.PrintBriefObjProp(n, nameof(GetCompiledFunctionBody), GetCompiledFunctionBody);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedString(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        /// <inheritdoc/>
        public override string ToHumanizedLabel(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        /// <inheritdoc/>
        public override MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return new MonitoredHumanizedLabel()
            {
                Label = NToHumanizedString()
            };
        }

        private string TypesListToHumanizedString()
        {
            if (TypesList.IsNullOrEmpty())
            {
                return "(any)";
            }

            return $"({string.Join(" | ", TypesList.Select(p => p.NameValue))})";
        }

        private string NToHumanizedString()
        {
            return $"prop {Name.NameValue}: {TypesListToHumanizedString()}";
        }
    }
}
