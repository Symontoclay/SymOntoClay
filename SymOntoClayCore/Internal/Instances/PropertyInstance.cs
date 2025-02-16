using SymOntoClay.Common.Disposing;
using SymOntoClay.Common;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Text;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Monitor.Common.Models;
using System.Xml.Linq;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Common.CollectionsHelpers;
using System.Linq;

namespace SymOntoClay.Core.Internal.Instances
{
    public class PropertyInstance : BaseComponent, IFilteredCodeItem,
        ISymOntoClayDisposable, IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToHumanizedString, IMonitoredHumanizedObject, IObjectWithLongHashCodes
    {
        public PropertyInstance(Property codeItem, IEngineContext context)
            : base(context.Logger)
        {
            Name = codeItem.Name;
            Holder = codeItem.Holder;
            CodeItem = codeItem;
            _inheritanceResolver = context.DataResolversFactory.GetInheritanceResolver();

            _anyTypeName = context.CommonNamesStorage.AnyTypeName;
        }

        private InheritanceResolver _inheritanceResolver;
        public StrongIdentifierValue Name { get; private set; }
        public StrongIdentifierValue Holder { get; private set; }
        public Property CodeItem { get; private set; }

        private StrongIdentifierValue _anyTypeName;

        public void SetValueDirectly(IMonitorLogger logger, Value value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            logger.Info("2C6EBD07-1417-4C62-90E1-441DB3CFFF73", $"value = {value}");

            if(_value == value)
            {
                return;
            }

            CheckFitVariableAndValue(logger, value, localCodeExecutionContext);

            _value = value;

            //throw new NotImplementedException("7D2B796B-C889-44B3-82D3-73A69884D2CD");

            OnChanged?.Invoke(Name);
        }

        private void CheckFitVariableAndValue(IMonitorLogger logger, Value value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            if (value.IsNullValue)
            {
                return;
            }

            var typesList = CodeItem.TypesList;

            if (typesList.IsNullOrEmpty())
            {
                return;
            }

            if(typesList.All(p => p == _anyTypeName))
            {
                return;
            }

            var isFit = _inheritanceResolver.IsFit(logger, typesList, value, localCodeExecutionContext);

            if (isFit)
            {
                return;
            }

            throw new Exception($"The value '{value.ToHumanizedString()}' does not fit to variable {CodeItem.ToHumanizedString()}");
        }

        private Value _value;

        public event Action<StrongIdentifierValue> OnChanged;

        /// <inheritdoc/>
        public TypeOfAccess TypeOfAccess => CodeItem.TypeOfAccess;

        /// <inheritdoc/>
        public ulong GetLongHashCode()
        {
            return CodeItem.GetLongHashCode();
        }

        /// <inheritdoc/>
        public ulong GetLongConditionalHashCode()
        {
            return CodeItem.GetLongConditionalHashCode();
        }

        /// <inheritdoc/>
        public bool HasConditionalSections => CodeItem.HasConditionalSections;

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            base.OnDisposed();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(0u);
        }

        /// <inheritdoc/>
        public string ToString(uint n)
        {
            return this.GetDefaultToStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToString.PropertiesToString(uint n)
        {
            return PropertiesToString(n);
        }

        protected virtual string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            //sb.AppendLine($"{spaces}{nameof(KindOfInstance)} = {KindOfInstance}");
            sb.PrintObjProp(n, nameof(Name), Name);
            sb.PrintObjProp(n, nameof(Holder), Holder);
            //sb.AppendLine($"{spaces}{nameof(_instanceState)} = {_instanceState}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToShortString()
        {
            return ToShortString(0u);
        }

        /// <inheritdoc/>
        public string ToShortString(uint n)
        {
            return this.GetDefaultToShortStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToShortString.PropertiesToShortString(uint n)
        {
            return PropertiesToShortString(n);
        }

        protected virtual string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            //sb.AppendLine($"{spaces}{nameof(KindOfInstance)} = {KindOfInstance}");
            sb.PrintShortObjProp(n, nameof(Name), Name);
            sb.PrintShortObjProp(n, nameof(Holder), Holder);
            //sb.AppendLine($"{spaces}{nameof(_instanceState)} = {_instanceState}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToBriefString()
        {
            return ToBriefString(0u);
        }

        /// <inheritdoc/>
        public string ToBriefString(uint n)
        {
            return this.GetDefaultToBriefStringInformation(n);
        }

        /// <inheritdoc/>
        string IObjectToBriefString.PropertiesToBriefString(uint n)
        {
            return PropertiesToBriefString(n);
        }

        protected virtual string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();

            //sb.AppendLine($"{spaces}{nameof(KindOfInstance)} = {KindOfInstance}");
            sb.PrintBriefObjProp(n, nameof(Name), Name);
            sb.PrintBriefObjProp(n, nameof(Holder), Holder);
            //sb.AppendLine($"{spaces}{nameof(_instanceState)} = {_instanceState}");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string ToHumanizedString(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return ToHumanizedString(opt);
        }

        /// <inheritdoc/>
        public string ToHumanizedString(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        /// <inheritdoc/>
        public string ToHumanizedLabel(HumanizedOptions options = HumanizedOptions.ShowAll)
        {
            var opt = new DebugHelperOptions()
            {
                HumanizedOptions = options
            };

            return ToHumanizedLabel(opt);
        }

        /// <inheritdoc/>
        public string ToHumanizedLabel(DebugHelperOptions options)
        {
            return NToHumanizedString();
        }

        /// <inheritdoc/>
        public string ToHumanizedString(IMonitorLogger logger)
        {
            return NToHumanizedString();
        }

        private string NToHumanizedString()
        {
            return Name.NameValue;
        }

        /// <inheritdoc/>
        public MonitoredHumanizedLabel ToLabel(IMonitorLogger logger)
        {
            return new MonitoredHumanizedLabel()
            {
                Label = NToHumanizedString()
            };
        }
    }
}
