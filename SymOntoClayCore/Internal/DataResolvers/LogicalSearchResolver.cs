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
            optionsOfFillExecutingCard.Logger = Logger;
            optionsOfFillExecutingCard.EntityDictionary = _context.Dictionary;
#endif

#if DEBUG
            Log($"optionsOfFillExecutingCard = {optionsOfFillExecutingCard}");
#endif

            var result = new LogicalSearchResult();

            var dataSource = new ConsolidatedDataSource(GetStoragesList(options.LocalCodeExecutionContext.Storage));

            var queryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            var queryExpression = options.QueryExpression;

            queryExpression.FillExecutingCard(queryExecutingCard, dataSource, optionsOfFillExecutingCard);

#if DEBUG
            Log($"@!@!@!@!@!@!@! queryExecutingCard = {queryExecutingCard}");
#endif

            var resultItemsList = new List<LogicalSearchResultItem>();

            foreach (var resultOfQueryToRelation in queryExecutingCard.ResultsOfQueryToRelationList)
            {
                var resultItem = new LogicalSearchResultItem();
                resultItem.ResultOfVarOfQueryToRelationList = resultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                resultItemsList.Add(resultItem);
            }

            result.Items = resultItemsList;

#if DEBUG
            Log("End");
#endif

            return result;
        }
    }
}
