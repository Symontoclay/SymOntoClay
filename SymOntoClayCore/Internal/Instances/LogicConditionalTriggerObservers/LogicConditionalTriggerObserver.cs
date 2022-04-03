using SymOntoClay.Core.Internal.CodeModel.ConditionOfTriggerExpr;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Instances.LogicConditionalTriggerObservers
{
    public class LogicConditionalTriggerObserver : BaseComponent, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public LogicConditionalTriggerObserver(IEngineContext context, IStorage parentStorage, TriggerConditionNode condition)
            : base(context.Logger)
        {
            _context = context;
            _parentStorage = parentStorage;
            _condition = condition;

            throw new NotImplementedException();
        }

        private readonly IEngineContext _context;
        private readonly IStorage _parentStorage;
        private readonly TriggerConditionNode _condition;

        public event Action OnChanged;

        /// <inheritdoc/>
        protected override void OnDisposed()
        {
            //_storage.LogicalStorage.OnChanged -= LogicalStorage_OnChanged;

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
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
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
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
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
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            return sb.ToString();
        }
    }
}
