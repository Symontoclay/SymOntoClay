/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.CreatingExamples
{
    public class CreatorExamples_Error_Processing_07_06_2021 : BaseCreatorExamples
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            var text = @"app PeaceKeeper
{
    fun a(@param_1) =>
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;

        error {: see(I, #a) :};

        'End of `a`' >> @>log;
    }

    on Init =>
    {
        try
        {
            'Begin' >> @>log;
            a(param_1: 1);
            'End' >> @>log;        
        }
        catch
        {
            'catch' >> @>log;
        }
        catch(@e)
        {
            'catch(@e)' >> @>log;
            @e >> @>log;
        }
        catch(@e) where {: hit(enemy, I) :}
        {
            'catch(@e) where {: hit(enemy, I) :}' >> @>log;
        }
        catch(@e) where {: see(I, $x) :}
        {
            'catch(@e) where {: see(I, $x) :}' >> @>log;
            @e >> @>log;
        }
        else
        {
            'else' >> @>log;
        }
        ensure
        {
            'ensure' >> @>log;
        }

        'End of `Init`' >> @>log;
    }
}";

            Example("ErrorProcessingExample_1", text);

            text = @"app PeaceKeeper
{
    fun a(@param_1) =>
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;

        error {: see(I, #a) :};

        'End of `a`' >> @>log;
    }

    on Init =>
    {
        try
        {
            'Begin' >> @>log;
            a(param_1: 1);
            'End' >> @>log;        
        }
        catch
        {
            'catch' >> @>log;
        }
        catch(@e)
        {
            'catch(@e)' >> @>log;
            @e >> @>log;
        }
        catch(@e) where {: hit(enemy, I) :}
        {
            'catch(@e) where {: hit(enemy, I) :}' >> @>log;
        }
        catch(@e) where {: see(I, $x) :}
        {
            'catch(@e) where {: see(I, $x) :}' >> @>log;
            @e >> @>log;
        }
        else
        {
            'else' >> @>log;
        }
        ensure
        {
            'ensure' >> @>log;
        }

        'End of `Init`' >> @>log;
    }

    on {: see(I, $x) :} ($x >> @x) => 
    {
        'on Fired' >> @>log;
        @x >> @>log;
    }
}";

            Example("ErrorProcessingExample_2", text);

            text = @"app PeaceKeeper
{
    fun a(@param_1) =>
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;

        error {: see(I, #a) :};

        'End of `a`' >> @>log;
    }

    on Init =>
    {
        try
        {
            'Begin' >> @>log;
            a(param_1: 1);
            'End' >> @>log;        
        }
        catch
        {
            'catch' >> @>log;
        }

        'End of `Init`' >> @>log;
    }
}";

            Example("ErrorProcessingExample_3", text);

            text = @"app PeaceKeeper
{
    fun a(@param_1) =>
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;

        error {: see(I, #a) :};

        'End of `a`' >> @>log;
    }

    on Init =>
    {
        try
        {
            'Begin' >> @>log;
            a(param_1: 1);
            'End' >> @>log;        
        }
        catch
        {
            'catch' >> @>log;
        }
        else
        {
            'else' >> @>log;
        }

        'End of `Init`' >> @>log;
    }
}";

            Example("ErrorProcessingExample_4", text);

            text = @"app PeaceKeeper
{
    fun a(@param_1) =>
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;

        error {: see(I, #a) :};

        'End of `a`' >> @>log;
    }

    on Init =>
    {
        try
        {
            'Begin' >> @>log;
            a(param_1: 1);
            'End' >> @>log;        
        }
        catch
        {
            'catch' >> @>log;
        }
        else
        {
            'else' >> @>log;
        }
        ensure
        {
            'ensure' >> @>log;
        }

        'End of `Init`' >> @>log;
    }
}";

            Example("ErrorProcessingExample_5", text);

            text = @"app PeaceKeeper
{
    fun a(@param_1) =>
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;

        error {: see(I, #a) :};

        'End of `a`' >> @>log;
    }

    on Init =>
    {
        try
        {
            'Begin' >> @>log;
            a(param_1: 1);
            'End' >> @>log;        
        }

        'End of `Init`' >> @>log;
    }
}";

            Example("ErrorProcessingExample_6", text);

            text = @"app PeaceKeeper
{
    on Init =>
    {
        try
        {
            'Begin' >> @>log;
            error {: see(I, #a) :};
            'End' >> @>log;        
        }
        else
        {
            'else' >> @>log;
        }

        'End of `Init`' >> @>log;
    }
}";

            Example("ErrorProcessingExample_7", text);

            text = @"app PeaceKeeper
{
    on Init =>
    {
        try
        {
            'Begin' >> @>log;
            error {: see(I, #a) :};
            'End' >> @>log;        
        }
        ensure
        {
            'ensure' >> @>log;
        }

        'End of `Init`' >> @>log;
    }
}";

            Example("ErrorProcessingExample_8", text);

            text = @"app PeaceKeeper
{
    on Init =>
    {
        try
        {
            'Begin' >> @>log;
            error {: see(I, #a) :};
            'End' >> @>log;        
        }
        else
        {
            'else' >> @>log;
        }
        ensure
        {
            'ensure' >> @>log;
        }

        'End of `Init`' >> @>log;
    }
}";

            Example("ErrorProcessingExample_9", text);

            _logger.Log("End");
        }

        //"ErrorProcessingExample_"

        /*text = @"";

        Example("ErrorProcessingExample_", text);*/
    }
}
