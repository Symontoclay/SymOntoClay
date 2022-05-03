using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public class VerbsExceptionCasesWordNetSource: BaseExceptionCasesWordNetSource
    {
        public VerbsExceptionCasesWordNetSource()
            : base(@"Resources\WordNet\dict\verb.exc")
        {
        }
    }
}
