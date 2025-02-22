using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Common.Disposing;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances
{
    public class VarInstance : BaseComponent, IFilteredCodeItem,
        ISymOntoClayDisposable, IObjectToString, IObjectToShortString, IObjectToBriefString, IObjectToHumanizedString, IMonitoredHumanizedObject, IObjectWithLongHashCodes
    {
        public VarInstance(StrongIdentifierValue varName, TypeOfAccess typeOfAccess, IMainStorageContext context)
            : base(context.Logger)
        {
            var varItem = new Var();
            varItem.Name = varName;

            varItem.TypeOfAccess = typeOfAccess;

            Name = varName;
            Holder = varItem.Holder;
            CodeItem = varItem;
            TypesList = varItem.TypesList;
        }

        public VarInstance(Var varItem, IMainStorageContext context)
            : base(context.Logger)
        {
            Name = varItem.Name;
            Holder = varItem.Holder;
            CodeItem = varItem;
            TypesList = varItem.TypesList;
        }

        public VarInstance(Field field, IMainStorageContext context)
            : base(context.Logger)
        {
            Name = field.Name;
            Holder = field.Holder;
            CodeItem = field;
            TypesList = field.TypesList;
        }

        public StrongIdentifierValue Name { get; private set; }
        public StrongIdentifierValue Holder { get; private set; }
        public CodeItem CodeItem { get; private set; }

        public List<StrongIdentifierValue> TypesList { get; private set; }

        public Value Value
        {
            get
            {
                lock (_lockObj)
                {
                    return _value;
                }
            }
        }

        /// <inheritdoc/>
        public void SetValue(IMonitorLogger logger, Value value)
        {
            lock (_lockObj)
            {
                if (_value == value)
                {
                    return;
                }

                _value = value;

                OnChanged?.Invoke(Name);
            }
        }

        private Value _value = new NullValue();

        public event Action<StrongIdentifierValue> OnChanged;

        private readonly object _lockObj = new object();

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

            //sb.AppendLine($"{spaces}{nameof(KindOfProperty)} = {KindOfProperty}");
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

            //sb.AppendLine($"{spaces}{nameof(KindOfProperty)} = {KindOfProperty}");
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

            //sb.AppendLine($"{spaces}{nameof(KindOfProperty)} = {KindOfProperty}");
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
