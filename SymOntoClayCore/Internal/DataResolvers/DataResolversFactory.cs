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

using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class DataResolversFactory : BaseLoggedComponent, IDataResolversFactory
    {
        public DataResolversFactory(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IMainStorageContext _context;

        private BaseResolver _baseResolver;
        private readonly object _baseResolverLockObj = new object();

        private ChannelsResolver _channelsResolver;
        private readonly object _channelsResolverLockObj = new object();

        private InheritanceResolver _inheritanceResolver;
        private readonly object _inheritanceResolverLockObj = new object();

        private LogicalValueLinearResolver _logicalValueLinearResolver;
        private readonly object _logicalValueLinearResolverLockObj = new object();

        private OperatorsResolver _operatorsResolver;
        private readonly object _operatorsResolverLockObj = new object();

        private NumberValueLinearResolver _numberValueLinearResolver;
        private readonly object _numberValueLinearResolverLockObj = new object();

        private StrongIdentifierLinearResolver _strongIdentifierLinearResolver;
        private readonly object _strongIdentifierLinearResolverLockObj = new object();

        private TriggersResolver _triggersResolver;
        private readonly object _triggersResolverLockObj = new object();

        private VarsResolver _varsResolver;
        private readonly object _varsResolverLockObj = new object();

        private LogicalSearchResolver _logicalSearchResolver;
        private readonly object _logicalSearchResolverLockObj = new object();

        private FuzzyLogicResolver _fuzzyLogicResolver;
        private readonly object _fuzzyLogicResolverLockObj = new object();

        private MethodsResolver _methodsResolver;
        private readonly object _methodsResolverLockObj = new object();

        private ConstructorsResolver _constructorsResolver;
        private readonly object _constructorsResolverLockObj = new object();

        private CodeItemDirectivesResolver _codeItemDirectivesResolver;
        private readonly object _codeItemDirectivesResolverLockObj = new object();

        private StatesResolver _statesResolver;
        private readonly object _statesResolverLockObj = new object();

        private ToSystemBoolResolver _toSystemBoolResolver;
        private readonly object _toSystemBoolResolverLockObj = new object();

        private RelationsResolver _relationsResolver;
        private readonly object _relationsResolverLockObj = new object();

        private LogicalValueModalityResolver _logicalValueModalityResolver;
        private readonly object _logicalValueModalityResolverLockObj = new object();

        private SynonymsResolver _synonymsResolver;
        private readonly object _synonymsResolverLockObj = new object();

        private IdleActionsResolver _idleActionsResolver;
        private readonly object _idleActionsResolverLockObj = new object();

        private AnnotationsResolver _annotationsResolver;
        private readonly object _annotationsResolverLockObj = new object();

        private ValueResolvingHelper _valueResolvingHelper;
        private readonly object _valueResolvingHelperLockObj = new object();

        private MetadataResolver _metadataResolver;
        private readonly object _metadataResolverLockObj = new object();

        private LogicalSearchVarResultsItemInvertor _logicalSearchVarResultsItemInvertor;
        private readonly object _logicalSearchVarResultsItemInvertorLockObj = new object();

        private DateTimeResolver _dateTimeResolver;
        private readonly object _dateTimeResolverLockObj = new object();

        /// <inheritdoc/>
        public BaseResolver GetBaseResolver()
        {
            lock(_baseResolverLockObj)
            {
                if(_baseResolver == null)
                {
                    _baseResolver = new BaseResolver(_context);
                }

                return _baseResolver;
            }
        }

        /// <inheritdoc/>
        public ChannelsResolver GetChannelsResolver()
        {
            lock (_channelsResolverLockObj)
            {
                if (_channelsResolver == null)
                {
                    _channelsResolver = new ChannelsResolver(_context);
                }

                return _channelsResolver;
            }
        }

        /// <inheritdoc/>
        public InheritanceResolver GetInheritanceResolver()
        {
            lock (_inheritanceResolverLockObj)
            {
                if (_inheritanceResolver == null)
                {
                    _inheritanceResolver = new InheritanceResolver(_context);
                }

                return _inheritanceResolver;
            }
        }

        /// <inheritdoc/>
        public LogicalValueLinearResolver GetLogicalValueLinearResolver()
        {
            lock (_logicalValueLinearResolverLockObj)
            {
                if (_logicalValueLinearResolver == null)
                {
                    _logicalValueLinearResolver = new LogicalValueLinearResolver(_context);
                }

                return _logicalValueLinearResolver;
            }
        }

        /// <inheritdoc/>
        public OperatorsResolver GetOperatorsResolver()
        {
            lock (_operatorsResolverLockObj)
            {
                if (_operatorsResolver == null)
                {
                    _operatorsResolver = new OperatorsResolver(_context);
                }

                return _operatorsResolver;
            }
        }

        /// <inheritdoc/>
        public NumberValueLinearResolver GetNumberValueLinearResolver()
        {
            lock (_numberValueLinearResolverLockObj)
            {
                if (_numberValueLinearResolver == null)
                {
                    _numberValueLinearResolver = new NumberValueLinearResolver(_context);
                }

                return _numberValueLinearResolver;
            }
        }

        /// <inheritdoc/>
        public StrongIdentifierLinearResolver GetStrongIdentifierLinearResolver()
        {
            lock (_strongIdentifierLinearResolverLockObj)
            {
                if (_strongIdentifierLinearResolver == null)
                {
                    _strongIdentifierLinearResolver = new StrongIdentifierLinearResolver(_context);
                }

                return _strongIdentifierLinearResolver;
            }
        }

        /// <inheritdoc/>
        public TriggersResolver GetTriggersResolver()
        {
            lock (_triggersResolverLockObj)
            {
                if (_triggersResolver == null)
                {
                    _triggersResolver = new TriggersResolver(_context);
                }

                return _triggersResolver;
            }
        }

        /// <inheritdoc/>
        public VarsResolver GetVarsResolver()
        {
            lock (_varsResolverLockObj)
            {
                if (_varsResolver == null)
                {
                    _varsResolver = new VarsResolver(_context);
                }

                return _varsResolver;
            }
        }

        /// <inheritdoc/>
        public LogicalSearchResolver GetLogicalSearchResolver()
        {
            lock (_logicalSearchResolverLockObj)
            {
                if (_logicalSearchResolver == null)
                {
                    _logicalSearchResolver = new LogicalSearchResolver(_context);
                }

                return _logicalSearchResolver;
            }
        }

        /// <inheritdoc/>
        public FuzzyLogicResolver GetFuzzyLogicResolver()
        {
            lock (_fuzzyLogicResolverLockObj)
            {
                if (_fuzzyLogicResolver == null)
                {
                    _fuzzyLogicResolver = new FuzzyLogicResolver(_context);
                }

                return _fuzzyLogicResolver;
            }
        }

        /// <inheritdoc/>
        public MethodsResolver GetMethodsResolver()
        {
            lock (_methodsResolverLockObj)
            {
                if (_methodsResolver == null)
                {
                    _methodsResolver = new MethodsResolver(_context);
                }

                return _methodsResolver;
            }
        }

        /// <inheritdoc/>
        public ConstructorsResolver GetConstructorsResolver()
        {
            lock(_constructorsResolverLockObj)
            {
                if(_constructorsResolver == null)
                {
                    _constructorsResolver = new ConstructorsResolver(_context);
                }

                return _constructorsResolver;
            }
        }

        /// <inheritdoc/>
        public CodeItemDirectivesResolver GetCodeItemDirectivesResolver()
        {
            lock (_codeItemDirectivesResolverLockObj)
            {
                if (_codeItemDirectivesResolver == null)
                {
                    _codeItemDirectivesResolver = new CodeItemDirectivesResolver(_context);
                }

                return _codeItemDirectivesResolver;
            }
        }

        /// <inheritdoc/>
        public StatesResolver GetStatesResolver()
        {
            lock (_statesResolverLockObj)
            {
                if (_statesResolver == null)
                {
                    _statesResolver = new StatesResolver(_context);
                }

                return _statesResolver;
            }
        }

        /// <inheritdoc/>
        public ToSystemBoolResolver GetToSystemBoolResolver()
        {
            lock (_toSystemBoolResolverLockObj)
            {
                if (_toSystemBoolResolver == null)
                {
                    _toSystemBoolResolver = new ToSystemBoolResolver(_context.Logger);
                }

                return _toSystemBoolResolver;
            }
        }

        /// <inheritdoc/>
        public RelationsResolver GetRelationsResolver()
        {
            lock (_relationsResolverLockObj)
            {
                if (_relationsResolver == null)
                {
                    _relationsResolver = new RelationsResolver(_context);
                }

                return _relationsResolver;
            }
        }

        /// <inheritdoc/>
        public LogicalValueModalityResolver GetLogicalValueModalityResolver()
        {
            lock(_logicalValueModalityResolverLockObj)
            {
                if (_logicalValueModalityResolver == null)
                {
                    _logicalValueModalityResolver = new LogicalValueModalityResolver(_context);
                }

                return _logicalValueModalityResolver;
            }
        }

        /// <inheritdoc/>
        public SynonymsResolver GetSynonymsResolver()
        {
            lock(_synonymsResolverLockObj)
            {
                if(_synonymsResolver == null)
                {
                    _synonymsResolver = new SynonymsResolver(_context);
                }

                return _synonymsResolver;
            }
        }

        /// <inheritdoc/>
        public IdleActionsResolver GetIdleActionsResolver()
        {
            lock(_idleActionsResolverLockObj)
            {
                if(_idleActionsResolver == null)
                {
                    _idleActionsResolver = new IdleActionsResolver(_context);
                }

                return _idleActionsResolver;
            }
        }

        /// <inheritdoc/>
        public AnnotationsResolver GetAnnotationsResolver()
        {
            lock(_annotationsResolverLockObj)
            {
                if(_annotationsResolver == null)
                {
                    _annotationsResolver = new AnnotationsResolver(_context);
                }

                return _annotationsResolver;
            }
        }

        /// <inheritdoc/>
        public ValueResolvingHelper GetValueResolvingHelper()
        {
            lock(_valueResolvingHelperLockObj)
            {
                if(_valueResolvingHelper == null)
                {
                    _valueResolvingHelper = new ValueResolvingHelper(_context);
                }

                return _valueResolvingHelper;
            }
        }

        /// <inheritdoc/>
        public MetadataResolver GetMetadataResolver()
        {
            lock(_metadataResolverLockObj)
            {
                if(_metadataResolver == null)
                {
                    _metadataResolver = new MetadataResolver(_context);
                }

                return _metadataResolver;
            }
        }

        /// <inheritdoc/>
        public LogicalSearchVarResultsItemInvertor GetLogicalSearchVarResultsItemInvertor()
        {
            lock(_logicalSearchVarResultsItemInvertorLockObj)
            {
                if(_logicalSearchVarResultsItemInvertor == null)
                {
                    _logicalSearchVarResultsItemInvertor = new LogicalSearchVarResultsItemInvertor(_context);
                }

                return _logicalSearchVarResultsItemInvertor;
            }
        }

        /// <inheritdoc/>
        public DateTimeResolver GetDateTimeResolver()
        {
            lock(_dateTimeResolverLockObj)
            {
                if(_dateTimeResolver == null)
                {
                    _dateTimeResolver = new DateTimeResolver(_context);
                }

                return _dateTimeResolver;
            }
        }
    }
}
