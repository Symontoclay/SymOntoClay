using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public class AdjLogicalMeaningsSource : LogicalMeaningsSource
    {
        public AdjLogicalMeaningsSource()
            :base(@"Resources\LogicalMeanings\adj")
        {
        }
    }
}
