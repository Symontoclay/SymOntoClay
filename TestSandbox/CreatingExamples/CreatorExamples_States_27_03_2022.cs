using SymOntoClay.CoreHelper.DebugHelpers;
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
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

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
	{: parent(#Piter, #Tom) :}
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
	{: parent(#Piter, #Tom) :}
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

            _logger.Log("End");
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
