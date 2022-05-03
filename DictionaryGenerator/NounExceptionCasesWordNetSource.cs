using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public class NounExceptionCasesWordNetSource : BaseExceptionCasesWordNetSource
    {
        public NounExceptionCasesWordNetSource()
            : base(@"Resources\WordNet\dict\noun.exc")
        {
        }
    }
}
