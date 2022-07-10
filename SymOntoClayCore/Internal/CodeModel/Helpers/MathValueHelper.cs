using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel.Helpers
{
    public static class MathValueHelper
    {
        public static NumberValue Max(IEnumerable<NumberValue> collection)
        {
            if (collection.Count() == 1)
            {
                return collection.Single();
            }

            if (collection.All(p => !p.SystemValue.HasValue))
            {
                return collection.First();
            }

            var notNullCollection = collection.Where(p => p.SystemValue.HasValue);

            if (notNullCollection.Count() == 1)
            {
                return notNullCollection.Single();
            }

            var maxSystemValue = notNullCollection.Select(p => p.SystemValue).Max();

            return notNullCollection.First(p => p.SystemValue == maxSystemValue);
        }
    }
}
