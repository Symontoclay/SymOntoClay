/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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
    public class CreatorExamples_States_27_03_2022 : BaseCreatorExamples
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        public void Run()
        {
            _logger.Info("BE5C3F2F-4D07-42D9-8E2F-FCB7623A0548", "Begin");

            var prefix = "States";

            var n = 1;

            var text = @"app PeaceKeeper
{
    set Idling as default state;

    on Init =>
    {
        'Begin' >> @>log;        
        'End' >> @>log;
    }
}

states { Idling, Attacking }

state Idling
{
    on Enter
    {
        'Begin Idling Enter' >> @>log;
        'End Idling Enter' >> @>log;
    }
}

state Attacking
{
    enter on:
        {: see(I, enemy) :}

    leave on:
        {: see(I, barrel) :}

    on Enter
    {
        'Begin Attacking Enter' >> @>log;

        'End Attacking Enter' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
    set Idling as state;

    on Init =>
    {
        'Begin' >> @>log;        
        'End' >> @>log;
    }
}

state Idling
{
    on Enter
    {
        'Begin Enter' >> @>log;
        'End Enter' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
    set Idling as default state;
    set Patrolling as state;

    on Init =>
    {
        'Begin' >> @>log;        
        'End' >> @>log;
    }
}

state Idling
{
    on Enter
    {
        'Begin Idling Enter' >> @>log;
        'End Idling Enter' >> @>log;
    }
}

state Patrolling
{
    on Enter
    {
        'Begin Patrolling Enter' >> @>log;

        set Idling as state;

        'End Patrolling Enter' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{ 
    set Patrolling as state;
}

state Idling
{
    on Enter
    {
        'Begin Idling Enter' >> @>log;
        'End Idling Enter' >> @>log;
    }
}

state Patrolling
{
    on Enter
    {
        'Begin Patrolling Enter' >> @>log;

        set Idling as default state;
        complete state;

        'End Patrolling Enter' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
    set Idling as default state;
    set Patrolling as state;

    on Init =>
    {
        'Begin' >> @>log;        
        'End' >> @>log;
    }

    on {: attack(I, enemy) :}
    {
        'D' >> @>log;
    }
}

state Idling
{
    on Enter
    {
        'Begin Idling Enter' >> @>log;
        'End Idling Enter' >> @>log;
    }
}

state Patrolling
{
    on Enter
    {
        'Begin Patrolling Enter' >> @>log;

        break state {: attack(I, enemy) :};

        'End Patrolling Enter' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
    set Idling as default state;
    set Patrolling as state;

    on Init =>
    {
        'Begin' >> @>log;        
        'End' >> @>log;
    }

    on {: attack(I, enemy) :}
    {
        'D' >> @>log;
    }
}

state Idling
{
    on Enter
    {
        'Begin Idling Enter' >> @>log;
        'End Idling Enter' >> @>log;
    }
}

state Patrolling
{
    on Enter
    {
        'Begin Patrolling Enter' >> @>log;

        break state;

        'End Patrolling Enter' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
    set Idling as default state;

    {: male(#Tom) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}
}

state Idling
{
    on Enter
    {
        'Begin Idling Enter' >> @>log;
        
        select {: son($x, $y) :} >> @>log;

        'End Idling Enter' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
    set Idling as default state;

    on Init =>
    {
        'Begin' >> @>log;
        
        select {: son($x, $y) :} >> @>log;

        'End' >> @>log;
    }
}

state Idling
{
    {: male(#Tom) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

    on Enter
    {
        'Begin Idling Enter' >> @>log;
        
        'End Idling Enter' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
    set Idling as default state;
}

state Idling
{
    on Enter
    {
        'Begin Idling Enter' >> @>log;
        ? {: bird ($x) :} >> @>log;
        insert {: >: { bird (#1234) } :};
        ? {: bird ($x) :} >> @>log;
        'End Idling Enter' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            _logger.Info("8828AA38-3BE4-49B2-B543-8C9AFBE397AB", "End");
        }

        /*
        var text = @"";

        Example(CreateName(prefix, n), text);

        n++;

        text = @"";

        Example(CreateName(prefix, n), text);
 */
    }
}
