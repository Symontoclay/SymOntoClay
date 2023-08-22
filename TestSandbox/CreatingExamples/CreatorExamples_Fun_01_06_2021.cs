/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.CreatingExamples
{
    public class CreatorExamples_Fun_01_06_2021: BaseCreatorExamples
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        public void Run()
        {
            _logger.Info("16FFA5DA-8F6D-40B2-8B81-13BD17DDAF93", "Begin");

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

            _logger.Info("DF0D3C24-4F20-4395-8CF6-D3F06A5AB64F", "End");
        }

    }
}
