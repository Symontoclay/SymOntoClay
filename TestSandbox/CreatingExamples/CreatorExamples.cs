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

            var text = @"app PeaceKeeper
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
}";

            Example("FuncExample_1", text);

            text = @"app PeaceKeeper
{
    fun a()
    {
        '`a` has been called!' >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        a();
        'End' >> @>log;
    }
}";

            Example("FuncExample_1_a", text);

            text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        a(1);
        'End' >> @>log;
    }
}";

            Example("FuncExample_2", text);

            text = @"fun a(@param_1)
{
    '`a` (any) has been called!' >> @>log;
    @param_1 >> @>log;
}

app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
        a(param_1: 1);
        'End' >> @>log;
    }
}";

            Example("FuncExample_2_a", text);

            text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        a(@param_1: 1);
        'End' >> @>log;
    }
}";

            Example("FuncExample_3", text);

            text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        a(param_1: 1);
        'End' >> @>log;
    }
}";

            Example("FuncExample_4", text);

            text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        @param_1 = 12;

        a(@param_1);
        'End' >> @>log;
    }
}";

            Example("FuncExample_5", text);

            text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` (1) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1, @param_2)
    {
        '`a` (2) has been called!' >> @>log;
        @param_1 >> @>log;
        @param_2 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        @param_1 = 12;

        a(@param_1);
        'End' >> @>log;
    }
}";

            Example("FuncExample_6", text);

            text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` (1) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1, @param_2)
    {
        '`a` (2) has been called!' >> @>log;
        @param_1 >> @>log;
        @param_2 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        @param_1 = 12;

        a(@param_1);
        a(3, 'Hi');
        'End' >> @>log;
    }
}";

            Example("FuncExample_7", text);

            text = @"app PeaceKeeper
{
    fun a(@param_1, @param_2 = 15)
    {
        '`a` (2) has been called!' >> @>log;
        @param_1 >> @>log;
        @param_2 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        @param_1 = 12;

        a(@param_1);
        'End' >> @>log;
    }
}";

            Example("FuncExample_8", text);

            text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` (any) has been called!' >> @>log;
    }

    fun a(@param_1: string)
    {
        '`a` (string) has been called!' >> @>log;
    }

    fun a(@param_1: number)
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        a(param_1: 1);
        'End' >> @>log;
    }
}";

            Example("FuncExample_9", text);

            text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` (any) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1: (number | string))
    {
        '`a` (number | string) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        a(param_1: 1);
        a(param_1: 'Hi');
        'End' >> @>log;
    }
}";

            Example("FuncExample_10_a", text);

            text = @"app PeaceKeeper
{
    fun a(@param_1)
    {
        '`a` (any) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    fun a(@param_1: number | string)
    {
        '`a` (number | string) has been called!' >> @>log;
        @param_1 >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        a(param_1: 1);
        a(param_1: 'Hi');
        'End' >> @>log;
    }
}";

            Example("FuncExample_10_b", text);

            _logger.Log("End");
        }

        //"FuncExample_"
        //Example(, text);
    }
}
