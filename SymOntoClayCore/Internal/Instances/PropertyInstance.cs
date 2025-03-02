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
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.EventsInterfaces;
using SymOntoClay.Core.Internal.CommonNames;
using SymOntoClay.Core.Internal.CodeModel.Helpers;

namespace SymOntoClay.Core.Internal.Instances
{
    public class PropertyInstance : BaseComponent, IFilteredCodeItem,
        ISymOntoClayDisposable, IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToHumanizedString, IMonitoredHumanizedObject, IObjectWithLongHashCodes
    {
        public PropertyInstance(Property codeItem, IInstance instance, IEngineContext context)
            : base(context.Logger)
        {
            Name = codeItem.Name;
            Holder = codeItem.Holder;

#if DEBUG
            Info("32E2E46B-EC84-416D-A78B-74DB54EA509F", $"Holder = {Holder}");
#endif

            CodeItem = codeItem;
            _instance = instance;
            _context = context;

#if DEBUG
            var selfName = context.CommonNamesStorage.SelfName;
            Info("E6A2620B-B34A-4ACB-B256-F2B9E1D0252C", $"selfName = {selfName}");
#endif

            _inheritanceResolver = context.DataResolversFactory.GetInheritanceResolver();

            _anyTypeName = context.CommonNamesStorage.AnyTypeName;

            var kindOfCodeEntity = codeItem.KindOfProperty;

            switch(kindOfCodeEntity)
            {
                case KindOfProperty.Readonly:
                    _propertyGetMethodExecutable = new PropertyGetMethodExecutable(this, context);
                    break;

                case KindOfProperty.GetSet:
                    _propertyGetMethodExecutable = new PropertyGetMethodExecutable(this, context);
                    break;
            }
        }

        private IInstance _instance;
        private IEngineContext _context;

        private InheritanceResolver _inheritanceResolver;

        public KindOfProperty KindOfProperty => CodeItem.KindOfProperty;
        public StrongIdentifierValue Name { get; private set; }
        public StrongIdentifierValue Holder { get; private set; }
        public Property CodeItem { get; private set; }

        private PropertyGetMethodExecutable _propertyGetMethodExecutable;

        public IExecutable GetMethodExecutable => _propertyGetMethodExecutable;

        private StrongIdentifierValue _anyTypeName;

        public void SetValue(IMonitorLogger logger, Value value, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            //logger.Info("2C6EBD07-1417-4C62-90E1-441DB3CFFF73", $"value = {value}");

            if(_value == value)
            {
                return;
            }

            CheckFitVariableAndValue(logger, value, localCodeExecutionContext);

            _value = value;

            switch(KindOfProperty)
            {
                case KindOfProperty.Auto:
                    {
                        var fact = BuildPropertyFactInstance(Name, _value);

#if DEBUG
                        Info("C02870F3-6784-4F04-8B2E-D010373AED9D", $"fact = {fact.ToHumanizedString()}");
#endif

                        _context.Storage.GlobalStorage.LogicalStorage.Append(logger, fact);
                    }
                    break;
                    //throw new NotImplementedException("5B236B9D-E24B-49C9-A6F1-2FCDA19E767C");

                default:
                    break;
            }

            //throw new NotImplementedException("7D2B796B-C889-44B3-82D3-73A69884D2CD");

            EmitOnChangedHandlers(Name);
        }

        private RuleInstance BuildPropertyFactInstance(StrongIdentifierValue propertyName, Value propertyValue)
        {
#if DEBUG
            Info("FF8A6D2E-34E0-4562-ADD6-252D430D7147", $"propertyName = {propertyName}");
            Info("6D7CA946-20E4-4738-9860-533B89DC5E25", $"propertyValue = {propertyValue}");
#endif

            var result = new RuleInstance();
            var primaryPart = new PrimaryRulePart();
            result.PrimaryPart = primaryPart;

            var sayRelation = new LogicalQueryNode()
            {
                Kind = KindOfLogicalQueryNode.Relation,
                Name = propertyName
            };

            primaryPart.Expression = sayRelation;

            sayRelation.ParamsList = new List<LogicalQueryNode>()
            {
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Entity,
                    Name = NameHelper.CreateName("i")//_instance.Name
                },
                new LogicalQueryNode()
                {
                    Kind = KindOfLogicalQueryNode.Value,
                    Value = propertyValue
                }
            };

            return result;
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

        public Value GetValue()
        {
            return _value;
        }

        private Value _value = NullValue.Instance;

        public void AddOnChangedHandler(IOnChangedPropertyHandler handler)
        {
            lock (_onChangedHandlersLockObj)
            {
                if (_onChangedHandlers.Contains(handler))
                {
                    return;
                }

                _onChangedHandlers.Add(handler);
            }
        }

        public void RemoveOnChangedHandler(IOnChangedPropertyHandler handler)
        {
            lock (_onChangedHandlersLockObj)
            {
                if (_onChangedHandlers.Contains(handler))
                {
                    _onChangedHandlers.Remove(handler);
                }
            }
        }

        private void EmitOnChangedHandlers(StrongIdentifierValue value)
        {
            lock (_onChangedHandlersLockObj)
            {
                foreach (var hander in _onChangedHandlers)
                {
                    hander.Invoke(value);
                }
            }
        }

        private object _onChangedHandlersLockObj = new object();
        private List<IOnChangedPropertyHandler> _onChangedHandlers = new List<IOnChangedPropertyHandler>();

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

            sb.AppendLine($"{spaces}{nameof(KindOfProperty)} = {KindOfProperty}");
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

            sb.AppendLine($"{spaces}{nameof(KindOfProperty)} = {KindOfProperty}");
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

            sb.AppendLine($"{spaces}{nameof(KindOfProperty)} = {KindOfProperty}");
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
