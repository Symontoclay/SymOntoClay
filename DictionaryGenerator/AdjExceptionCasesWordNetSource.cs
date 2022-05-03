using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public class AdjExceptionCasesWordNetSource : BaseExceptionCasesWordNetSource
    {
        public AdjExceptionCasesWordNetSource()
            : base(@"Resources\WordNet\dict\adj.exc")
        {
        }
    }
}
