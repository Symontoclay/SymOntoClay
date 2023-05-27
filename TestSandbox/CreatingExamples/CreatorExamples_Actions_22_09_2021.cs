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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.CreatingExamples
{
    public class CreatorExamples_Actions_22_09_2021 : BaseCreatorExamples
    {
        private static readonly IEntityLogger _logger = new LoggerNLogImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
 
        Go();

        'End' >> @>log;
    }
}

action Go 
{
    on Init =>
    {
        'Init Go' >> @>log;
    }

    op () => 
    {
        'Begin Go' >> @>log;
        'End Go' >> @>log;
    }

    on {: see(I, $x) :} ($x >> @x) => 
    {
        'on Fired' >> @>log;
        @x >> @>log;
        complete action;
    }
}";

            Example("ActionExample_1", text);

            _logger.Log("End");
        }
    }
}
