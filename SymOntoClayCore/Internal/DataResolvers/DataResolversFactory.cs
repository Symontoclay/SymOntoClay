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
        private TriggersResolver _triggersResolver;


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
    }
}
