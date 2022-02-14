/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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
    public class DataResolversFactory: BaseLoggedComponent, IDataResolversFactory
    {
        public DataResolversFactory(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IMainStorageContext _context;
        private readonly object _lockObj = new object();

        private ChannelsResolver _channelsResolver;
        private InheritanceResolver _inheritanceResolver;
        private LogicalValueLinearResolver _logicalValueLinearResolver;
        private OperatorsResolver _operatorsResolver;
        private NumberValueLinearResolver _numberValueLinearResolver;
        private StrongIdentifierLinearResolver _strongIdentifierLinearResolver;
        private TriggersResolver _triggersResolver;
        private VarsResolver _varsResolver;
        private LogicalSearchResolver _logicalSearchResolver;
        private FuzzyLogicResolver _fuzzyLogicResolver;
        private MethodsResolver _methodsResolver;

        /// <inheritdoc/>
        public ChannelsResolver GetChannelsResolver()
        {
            lock(_lockObj)
            {
                if(_channelsResolver == null)
                {
                    _channelsResolver = new ChannelsResolver(_context);
                }

                return _channelsResolver;
            }
        }

        /// <inheritdoc/>
        public InheritanceResolver GetInheritanceResolver()
        {
            lock (_lockObj)
            {
                if(_inheritanceResolver == null)
                {
                    _inheritanceResolver = new InheritanceResolver(_context);
                }

                return _inheritanceResolver;
            }
        }

        /// <inheritdoc/>
        public LogicalValueLinearResolver GetLogicalValueLinearResolver()
        {
            lock (_lockObj)
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
            lock (_lockObj)
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
            lock (_lockObj)
            {
                if(_numberValueLinearResolver == null)
                {
                    _numberValueLinearResolver = new NumberValueLinearResolver(_context);
                }

                return _numberValueLinearResolver;
            }
        }

        /// <inheritdoc/>
        public StrongIdentifierLinearResolver GetStrongIdentifierLinearResolver()
        {
            lock (_lockObj)
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
            lock(_lockObj)
            {
                if(_triggersResolver == null)
                {
                    _triggersResolver = new TriggersResolver(_context);
                }

                return _triggersResolver;
            }
        }

        /// <inheritdoc/>
        public VarsResolver GetVarsResolver()
        {
            lock (_lockObj)
            {
                if(_varsResolver == null)
                {
                    _varsResolver = new VarsResolver(_context);
                }

                return _varsResolver;
            }
        }

        /// <inheritdoc/>
        public LogicalSearchResolver GetLogicalSearchResolver()
        {
            lock (_lockObj)
            {
                if(_logicalSearchResolver == null)
                {
                    _logicalSearchResolver = new LogicalSearchResolver(_context);
                }

                return _logicalSearchResolver;
            }
        }

        /// <inheritdoc/>
        public FuzzyLogicResolver GetFuzzyLogicResolver()
        {
            lock (_lockObj)
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
            lock (_lockObj)
            {
                if (_methodsResolver == null)
                {
                    _methodsResolver = new MethodsResolver(_context);
                }

                return _methodsResolver;
            }
        }
    }
}
