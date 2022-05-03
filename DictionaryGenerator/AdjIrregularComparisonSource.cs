using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public class AdjIrregularComparisonSource: BaseIrregularComparisonSource
    {
        public AdjIrregularComparisonSource()
            : base(@"Resources\MyDicts\adj.txt")
        {
        }
    }
}
