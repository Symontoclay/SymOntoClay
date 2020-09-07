using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public class LogicalSearchResolver : BaseResolver
    {
        public LogicalSearchResolver(IMainStorageContext context)
            : base(context)
        {
        }

        public LogicalSearchResult Run(LogicalSearchOptions options)
        {
#if DEBUG
            Log($"options = {options}");
#endif

            var optionsOfFillExecutingCard = new OptionsOfFillExecutingCard();
            optionsOfFillExecutingCard.EntityIdOnly = options.EntityIdOnly;
            optionsOfFillExecutingCard.UseAccessPolicy = !options.IgnoreAccessPolicy;

#if DEBUG
            Log($"optionsOfFillExecutingCard = {optionsOfFillExecutingCard}");
#endif

            var queryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            var queryExpression = options.QueryExpression;

            queryExpression.FillExecutingCard(queryExecutingCard, dataSource, optionsOfFillExecutingCard);

#if DEBUG
            Log($"@!@!@!@!@!@!@! queryExecutingCard = {queryExecutingCard}");
#endif

            throw new NotImplementedException();
        }
    }
}
