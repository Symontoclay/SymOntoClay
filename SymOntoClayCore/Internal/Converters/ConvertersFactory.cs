using SymOntoClay.Core.Internal.DataResolvers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Converters
{
    public class ConvertersFactory: BaseLoggedComponent, IConvertersFactory
    {
        public ConvertersFactory(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IMainStorageContext _context;

        private ConverterFactToImperativeCode _converterFactToImperativeCode;
        private readonly object _converterFactToImperativeCodeLockObj = new object();

        /// <inheritdoc/>
        public ConverterFactToImperativeCode GetConverterFactToImperativeCode()
        {
            lock (_converterFactToImperativeCodeLockObj)
            {
                if (_converterFactToImperativeCode == null)
                {
                    _converterFactToImperativeCode = new ConverterFactToImperativeCode(_context);
                }

                return _converterFactToImperativeCode;
            }
        }
    }
}
