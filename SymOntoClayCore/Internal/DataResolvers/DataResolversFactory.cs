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

using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class DataResolversFactory : BaseContextComponent, IDataResolversFactory
    {
        public DataResolversFactory(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;

            _baseResolver = new BaseResolver(_context);
            _baseContextComponents.Add(_baseResolver);

            _channelsResolver = new ChannelsResolver(_context);
            _baseContextComponents.Add(_channelsResolver);

            _inheritanceResolver = new InheritanceResolver(_context);
            _baseContextComponents.Add(_inheritanceResolver);

            _logicalValueLinearResolver = new LogicalValueLinearResolver(_context);
            _baseContextComponents.Add(_logicalValueLinearResolver);

            _operatorsResolver = new OperatorsResolver(_context);
            _baseContextComponents.Add(_operatorsResolver);

            _numberValueLinearResolver = new NumberValueLinearResolver(_context);
            _baseContextComponents.Add(_numberValueLinearResolver);

            _strongIdentifierLinearResolver = new StrongIdentifierLinearResolver(_context);
            _baseContextComponents.Add(_strongIdentifierLinearResolver);

            _triggersResolver = new TriggersResolver(_context);
            _baseContextComponents.Add(_triggersResolver);

            _varsResolver = new VarsResolver(_context);
            _baseContextComponents.Add(_varsResolver);

            _propertiesResolver = new PropertiesResolver(_context);
            _baseContextComponents.Add(_propertiesResolver);

            _logicalSearchResolver = new LogicalSearchResolver(_context);
            _baseContextComponents.Add(_logicalSearchResolver);

            _fuzzyLogicResolver = new FuzzyLogicResolver(_context);
            _baseContextComponents.Add(_fuzzyLogicResolver);

            _methodsResolver = new MethodsResolver(_context);
            _baseContextComponents.Add(_methodsResolver);

            _constructorsResolver = new ConstructorsResolver(_context);
            _baseContextComponents.Add(_constructorsResolver);

            _codeItemDirectivesResolver = new CodeItemDirectivesResolver(_context);
            _baseContextComponents.Add(_codeItemDirectivesResolver);

            _statesResolver = new StatesResolver(_context);
            _baseContextComponents.Add(_statesResolver);

            _toSystemBoolResolver = new ToSystemBoolResolver(_context.Logger);
            _baseContextComponents.Add(_toSystemBoolResolver);

            _relationsResolver = new RelationsResolver(_context);
            _baseContextComponents.Add(_relationsResolver);

            _logicalValueModalityResolver = new LogicalValueModalityResolver(_context);
            _baseContextComponents.Add(_logicalValueModalityResolver);

            _synonymsResolver = new SynonymsResolver(_context);
            _baseContextComponents.Add(_synonymsResolver);

            _idleActionsResolver = new IdleActionsResolver(_context);
            _baseContextComponents.Add(_idleActionsResolver);

            _annotationsResolver = new AnnotationsResolver(_context);
            _baseContextComponents.Add(_annotationsResolver);

            _valueResolvingHelper = new ValueResolvingHelper(_context);
            _baseContextComponents.Add(_valueResolvingHelper);

            _metadataResolver = new MetadataResolver(_context);
            _baseContextComponents.Add(_metadataResolver);

            _logicalSearchVarResultsItemInvertor = new LogicalSearchVarResultsItemInvertor(_context);
            _baseContextComponents.Add(_logicalSearchVarResultsItemInvertor);

            _dateTimeResolver = new DateTimeResolver(_context);
            _baseContextComponents.Add(_dateTimeResolver);

            _strongIdentifierExprValueResolver = new StrongIdentifierExprValueResolver(_context);
            _baseContextComponents.Add(_strongIdentifierExprValueResolver);
        }

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            foreach (var item in _baseContextComponents)
            {
                item.LinkWithOtherBaseContextComponents();
            }
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();

            foreach (var item in _baseContextComponents)
            {
                item.Init();
            }
        }

        private readonly IMainStorageContext _context;

        private readonly List<IBaseContextComponent> _baseContextComponents = new List<IBaseContextComponent>();

        private readonly BaseResolver _baseResolver;
        private readonly ChannelsResolver _channelsResolver;
        private readonly InheritanceResolver _inheritanceResolver;
        private readonly LogicalValueLinearResolver _logicalValueLinearResolver;
        private readonly OperatorsResolver _operatorsResolver;
        private readonly NumberValueLinearResolver _numberValueLinearResolver;
        private readonly StrongIdentifierLinearResolver _strongIdentifierLinearResolver;
        private readonly TriggersResolver _triggersResolver;
        private readonly VarsResolver _varsResolver;
        private readonly PropertiesResolver _propertiesResolver;
        private readonly LogicalSearchResolver _logicalSearchResolver;
        private readonly FuzzyLogicResolver _fuzzyLogicResolver;
        private readonly MethodsResolver _methodsResolver;
        private readonly ConstructorsResolver _constructorsResolver;
        private readonly CodeItemDirectivesResolver _codeItemDirectivesResolver;
        private readonly StatesResolver _statesResolver;
        private readonly ToSystemBoolResolver _toSystemBoolResolver;
        private readonly RelationsResolver _relationsResolver;
        private readonly LogicalValueModalityResolver _logicalValueModalityResolver;
        private readonly SynonymsResolver _synonymsResolver;
        private readonly IdleActionsResolver _idleActionsResolver;
        private readonly AnnotationsResolver _annotationsResolver;
        private readonly ValueResolvingHelper _valueResolvingHelper;
        private readonly MetadataResolver _metadataResolver;
        private readonly LogicalSearchVarResultsItemInvertor _logicalSearchVarResultsItemInvertor;
        private readonly DateTimeResolver _dateTimeResolver;
        private readonly StrongIdentifierExprValueResolver _strongIdentifierExprValueResolver;

        /// <inheritdoc/>
        public BaseResolver GetBaseResolver()
        {
            return _baseResolver;
        }

        /// <inheritdoc/>
        public ChannelsResolver GetChannelsResolver()
        {
            return _channelsResolver;
        }

        /// <inheritdoc/>
        public InheritanceResolver GetInheritanceResolver()
        {
            return _inheritanceResolver;
        }

        /// <inheritdoc/>
        public LogicalValueLinearResolver GetLogicalValueLinearResolver()
        {
            return _logicalValueLinearResolver;
        }

        /// <inheritdoc/>
        public OperatorsResolver GetOperatorsResolver()
        {
            return _operatorsResolver;
        }

        /// <inheritdoc/>
        public NumberValueLinearResolver GetNumberValueLinearResolver()
        {
            return _numberValueLinearResolver;
        }

        /// <inheritdoc/>
        public StrongIdentifierLinearResolver GetStrongIdentifierLinearResolver()
        {
            return _strongIdentifierLinearResolver;
        }

        /// <inheritdoc/>
        public TriggersResolver GetTriggersResolver()
        {
            return _triggersResolver;
        }

        /// <inheritdoc/>
        public VarsResolver GetVarsResolver()
        {
            return _varsResolver;
        }

        /// <inheritdoc/>
        public PropertiesResolver GetPropertiesResolver()
        {
            return _propertiesResolver;
        }
        
        /// <inheritdoc/>
        public LogicalSearchResolver GetLogicalSearchResolver()
        {
            return _logicalSearchResolver;
        }

        /// <inheritdoc/>
        public FuzzyLogicResolver GetFuzzyLogicResolver()
        {
            return _fuzzyLogicResolver;
        }

        /// <inheritdoc/>
        public MethodsResolver GetMethodsResolver()
        {
            return _methodsResolver;
        }

        /// <inheritdoc/>
        public ConstructorsResolver GetConstructorsResolver()
        {
            return _constructorsResolver;
        }

        /// <inheritdoc/>
        public CodeItemDirectivesResolver GetCodeItemDirectivesResolver()
        {
            return _codeItemDirectivesResolver;
        }

        /// <inheritdoc/>
        public StatesResolver GetStatesResolver()
        {
            return _statesResolver;
        }

        /// <inheritdoc/>
        public ToSystemBoolResolver GetToSystemBoolResolver()
        {
            return _toSystemBoolResolver;
        }

        /// <inheritdoc/>
        public RelationsResolver GetRelationsResolver()
        {
            return _relationsResolver;
        }

        /// <inheritdoc/>
        public LogicalValueModalityResolver GetLogicalValueModalityResolver()
        {
            return _logicalValueModalityResolver;
        }

        /// <inheritdoc/>
        public SynonymsResolver GetSynonymsResolver()
        {
            return _synonymsResolver;
        }

        /// <inheritdoc/>
        public IdleActionsResolver GetIdleActionsResolver()
        {
            return _idleActionsResolver;
        }

        /// <inheritdoc/>
        public AnnotationsResolver GetAnnotationsResolver()
        {
            return _annotationsResolver;
        }

        /// <inheritdoc/>
        public ValueResolvingHelper GetValueResolvingHelper()
        {
            return _valueResolvingHelper;
        }

        /// <inheritdoc/>
        public MetadataResolver GetMetadataResolver()
        {
            return _metadataResolver;
        }

        /// <inheritdoc/>
        public LogicalSearchVarResultsItemInvertor GetLogicalSearchVarResultsItemInvertor()
        {
            return _logicalSearchVarResultsItemInvertor;
        }

        /// <inheritdoc/>
        public DateTimeResolver GetDateTimeResolver()
        {
            return _dateTimeResolver;
        }

        /// <inheritdoc/>
        public StrongIdentifierExprValueResolver GetStrongIdentifierExprValueResolver()
        {
            return _strongIdentifierExprValueResolver;
        }
    }
}
