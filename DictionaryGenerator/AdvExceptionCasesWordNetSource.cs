using System;
using System.Collections.Generic;
using System.Text;

namespace DictionaryGenerator
{
    public class AdvExceptionCasesWordNetSource : BaseExceptionCasesWordNetSource
    {
        public AdvExceptionCasesWordNetSource()
            : base(@"Resources\WordNet\dict\adv.exc")
        {
        }
    }
}
