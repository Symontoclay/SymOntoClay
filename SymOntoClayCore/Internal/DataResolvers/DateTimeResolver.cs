using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class DateTimeResolver : BaseResolver
    {
        public DateTimeResolver(IMainStorageContext context)
            : base(context)
        {
            _dateTimeProvider = context.DateTimeProvider;
        }

        private readonly IDateTimeProvider _dateTimeProvider;


        public long? ConvertTimeValueToTicks(Value duration)
        {

        }

        public float GetCurrentSeconds()
        {
            return _dateTimeProvider.CurrentTiks * _dateTimeProvider.SecondsMultiplicator;
        }
    }
}
