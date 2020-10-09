/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
    }
}
