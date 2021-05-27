using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.CreatingExamples
{
    public class CreatorExamples: BaseCreatorExamples
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public CreatorExamples()
            : base(new BaseCreatorExamplesOptions())
        {
        }

        public void Run()
        {
            _logger.Log("Begin");

            Example("Func_1", @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        a();
        'End' >> @>log;
    }
}");

            _logger.Log("End");
        }
    }
}
