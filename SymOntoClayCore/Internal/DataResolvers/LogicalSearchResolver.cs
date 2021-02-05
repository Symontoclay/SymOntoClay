/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var usedKeysList = queryExecutingCard.UsedKeysList.Distinct().ToList();

#if DEBUG
            //Log($"usedKeysList.Count = {usedKeysList.Count}");
            //foreach(var usedKey in usedKeysList)
            //{
            //    Log($"usedKey = {usedKey}");
            //    Log($"_context.Dictionary.GetName(usedKey) = {_context.Dictionary.GetName(usedKey)}");
            //}
#endif

            result.UsedKeysList = usedKeysList;

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
