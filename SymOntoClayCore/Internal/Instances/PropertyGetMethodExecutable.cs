using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Common;
using SymOntoClay.Core.Internal.CodeExecution;
using System.Text;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;

namespace SymOntoClay.Core.Internal.Instances
{
    public class PropertyGetMethodExecutable: BaseComponent, IExecutable
    {
        public PropertyGetMethodExecutable(PropertyInstance propertyInstance, IEngineContext context)
            : base(context.Logger)
        {
            _propertyInstance = propertyInstance;

            //throw new NotImplementedException("80E6CE4D-09E5-4E86-9868-4206348BB910");
        }

        private PropertyInstance _propertyInstance;

        /// <inheritdoc/>
        public bool IsSystemDefined => false;

        /// <inheritdoc/>
        public IList<IFunctionArgument> Arguments => new List<IFunctionArgument>();

        /// <inheritdoc/>
        public CompiledFunctionBody CompiledFunctionBody => _propertyInstance.CodeItem.GetCompiledFunctionBody;

        /// <inheritdoc/>
        public CodeItem CodeItem => _propertyInstance.CodeItem;

        /// <inheritdoc/>
        public ISystemHandler SystemHandler => null;

        /// <inheritdoc/>
        public ILocalCodeExecutionContext OwnLocalCodeExecutionContext => null;

        /// <inheritdoc/>
        public StrongIdentifierValue Holder => _propertyInstance.Holder;

        /// <inheritdoc/>
        public bool NeedActivation => false;

        /// <inheritdoc/>
        public bool IsActivated => false;

        /// <inheritdoc/>
        public UsingLocalCodeExecutionContextPreferences UsingLocalCodeExecutionContextPreferences => UsingLocalCodeExecutionContextPreferences.Default;

        /// <inheritdoc/>
        public bool IsInstance => false;

        /// <inheritdoc/>
        public IInstance AsInstance => null;

        /// <inheritdoc/>
        public IExecutionCoordinator GetCoordinator(IMonitorLogger logger, IEngineContext context, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return null;
        }

        /// <inheritdoc/>
        public bool ContainsArgument(IMonitorLogger logger, StrongIdentifierValue name)
        {
            return false;
        }

        public IExecutable Activate(IMonitorLogger logger, IEngineContext context, ILocalCodeExecutionContext localCodeExecutionContext, IExecutionCoordinator executionCoordinator)
        {
            return this;
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
            //sb.PrintObjProp(n, nameof(Name), Name);
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
            //sb.PrintShortObjProp(n, nameof(Name), Name);
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
            //sb.PrintBriefObjProp(n, nameof(Name), Name);
            //sb.AppendLine($"{spaces}{nameof(_instanceState)} = {_instanceState}");

            return sb.ToString();
        }
    }
}
