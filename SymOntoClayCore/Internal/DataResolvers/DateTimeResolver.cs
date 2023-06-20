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

        /// <summary>
        /// Gets the current quasi time in quasi seconds.
        /// </summary>
        /// <returns>Quasi seconds elapsed since the engine was launched.</returns>
        public float GetCurrentSeconds()
        {
            return _dateTimeProvider.CurrentTiks * _dateTimeProvider.SecondsMultiplicator;
        }
    }
}
