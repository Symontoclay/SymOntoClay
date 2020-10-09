/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
            //Log($"options = {options}");
#endif

            var optionsOfFillExecutingCard = new OptionsOfFillExecutingCard();
            optionsOfFillExecutingCard.EntityIdOnly = options.EntityIdOnly;
            optionsOfFillExecutingCard.UseAccessPolicy = !options.IgnoreAccessPolicy;
            optionsOfFillExecutingCard.UseInheritance = options.UseInheritance;
            optionsOfFillExecutingCard.InheritanceResolver = _context.DataResolversFactory.GetInheritanceResolver();
            optionsOfFillExecutingCard.LocalCodeExecutionContext = options.LocalCodeExecutionContext;


#if DEBUG
            optionsOfFillExecutingCard.Logger = Logger;
            optionsOfFillExecutingCard.EntityDictionary = _context.Dictionary;
#endif

#if DEBUG
            //Log($"optionsOfFillExecutingCard = {optionsOfFillExecutingCard}");
#endif

            var result = new LogicalSearchResult();

            var dataSource = new ConsolidatedDataSource(GetStoragesList(options.LocalCodeExecutionContext.Storage));

            var queryExecutingCard = new QueryExecutingCardForIndexedPersistLogicalData();

            var queryExpression = options.QueryExpression;

            queryExpression.FillExecutingCard(queryExecutingCard, dataSource, optionsOfFillExecutingCard);

#if DEBUG
            //Log($"@!@!@!@!@!@!@! queryExecutingCard = {queryExecutingCard}");
#endif
            result.IsSuccess = queryExecutingCard.IsSuccess;

            var resultItemsList = new List<LogicalSearchResultItem>();

            foreach (var resultOfQueryToRelation in queryExecutingCard.ResultsOfQueryToRelationList)
            {
                var resultItem = new LogicalSearchResultItem();
                resultItem.ResultOfVarOfQueryToRelationList = resultOfQueryToRelation.ResultOfVarOfQueryToRelationList;
                resultItemsList.Add(resultItem);
            }

            result.Items = resultItemsList;

#if DEBUG
            //Log("End");
#endif

            return result;
        }
    }
}
