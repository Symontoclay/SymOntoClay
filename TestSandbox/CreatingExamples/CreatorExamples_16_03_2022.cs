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

using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;

namespace TestSandbox.CreatingExamples
{
    public class CreatorExamples_16_03_2022 : BaseCreatorExamples
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImplementation();

        public void Run()
        {
            _logger.Info("D88573AD-4B66-40B6-B0FA-C405CD2F3C95", "Begin");

            Variables();
            MemberAccessModifiers();
            Field();
            ImperativeLogicOperators();
            ArithmeticOperators();
            LogicConditionalTriggers();
            RepeatStatement();
            WhileStatement();
            ReturnStatement();//in process
            IfElifElseStatement();//in process
            ContinueLoopStatement();//in process
            BreakLoopStatement();//in process

            _logger.Info("995F344C-14E9-48F9-8EA6-A3EBC4AE7F8C", "End");
        }

        private void Variables()
        {
            var prefix = "Variables";
            var n = 3;

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
        var @a: number = 2;
        @a >> @>log;
        var @b: number;
        @b >> @>log;
        var @c;
        @c >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);
        }

        private void MemberAccessModifiers()
        {
            var prefix = "MemberAccessModifiers";

            var n = 1;

            var text = @"class Cls1
{
protected:
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }
}

app PeaceKeeper is Cls1
{
private:
    on Init =>
    {
        'Begin' >> @>log;
        a();
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"class Cls1
{
private:
	{: male(#Tom) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}
}

app PeaceKeeper is Cls1
{
private:
    on Init =>
    {
        'Begin' >> @>log;
        select {: son($x, $y) :} >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"class Cls1
{
protected:
	{: male(#Tom) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}
}

app PeaceKeeper is Cls1
{
private:
    on Init =>
    {
        'Begin' >> @>log;
        select {: son($x, $y) :} >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);
        }

        private void Field()
        {
            var prefix = "Field";

            var n = 1;

            var text = @"app PeaceKeeper
{
    @b;

    on Init =>
    {
        'Begin' >> @>log;
        @b >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
    var @b: number = 2;

    on Init =>
    {
        'Begin' >> @>log;
        @b >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);
        }

        private void ImperativeLogicOperators()
        {
            var prefix = "ImperativeLogicOperators";

            var n = 1;

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
        
        @a = 3;

        if(@a <= 0 | @a is 3 | @a > 5)
        {
            'Yes!' >> @>log;
        } else {
            'Else Yes!' >> @>log;
        }

        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"linvar age for range (0, 150]
{
    terms:
	    `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
        
        @a = 16;

        if(@a <= teenager)
        {
            'Yes!' >> @>log;
        } else {
            'Else Yes!' >> @>log;
        }

        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);
        }

        private void ArithmeticOperators()
        {
            var prefix = "ArithmeticOperators";

            var n = 1;

            var text = @"app PeaceKeeper
{
private:
    on Init =>
    {
        'Begin' >> @>log;
        1 + 1 >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
private:
    @a = 2;

    on Init =>
    {
        @b = 3;

        'Begin' >> @>log;
        @a + @b >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
private:
    on Init =>
    {
        'Begin' >> @>log;
        1 + NULL >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
private:
    on Init =>
    {
        'Begin' >> @>log;
        1 + 'Hi' >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
private:
    on Init =>
    {
        'Begin' >> @>log;
        3 - 1 >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
private:
    on Init =>
    {
        'Begin' >> @>log;
        3 * 4 >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
private:
    on Init =>
    {
        'Begin' >> @>log;
        12 / 4 >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
private:
    on Init =>
    {
        'Begin' >> @>log;
        12 / 0 >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
        
        (3 + 5) * 2 >> @>log;

        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
private:
    on Init =>
    {
        'Begin' >> @>log;
        @a = 2;
        2 * -@a >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
private:
    on Init =>
    {
        'Begin' >> @>log;
        @a = -2;
        2 * -@a >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
private:
    on Init =>
    {
        'Begin' >> @>log;
        @a = -2;
        2 * +@a >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
private:
    on Init =>
    {
        'Begin' >> @>log;
        @a = 2;
        2 * +@a >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);
        }

        private void LogicConditionalTriggers()
        {
            var prefix = "LogicConditional_Trigger";

            var n = 3;

            var text = @"app PeaceKeeper
{
    @a = #`gun 1`;

    on Init =>
    {
        'Begin' >> @>log;
        insert {: see(I, @a) :};
    }

    on {: see(I, @a) :} => 
    {
	    'D' >> @>log;
	}
}";

            Example(CreateName(prefix, n), text);
        }





        private void RepeatStatement()
        {
            var prefix = "RepeatStatement";

            var n = 1;

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
        
        @a = 10;

        repeat
        {
            @a >> @>log;
            @a = @a - 1;

            if(@a > 5)
            {
                continue;
            }

            'End of while iteration' >> @>log;

            break;
        }

        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);
        }

        private void WhileStatement()
        {
            var prefix = "WhileStatement";

            var n = 1;

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
        
        @a = 10;

        while (@a > 0)
        {
            @a >> @>log;
            @a = @a - 1;
        }

        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);
        }

        private void ReturnStatement()
        {
            var prefix = "ReturnStatement";

            var n = 1;

            var text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
        return;
        '`a` has been ended!' >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        a();
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
        return;
        '`a` has been ended!' >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        a() >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
        return 1;
        '`a` has been ended!' >> @>log;
    }

    on Init =>
    {
        'Begin' >> @>log;
        a() >> @>log;
        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);
        }

        private void IfElifElseStatement()
        {
            var prefix = "IfElifElseStatement";

            var n = 1;

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
        
        @a = 1;

        if(@a)
        {
            'Yes!' >> @>log;
        }

        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
    {: >: { see(I, #`Barrel 1`) } :}

    on Init =>
    {
        'Begin' >> @>log;
        
        if({: >: { see(I, #`Barrel 1`) } :})
        {
            'Yes!' >> @>log;
        }

        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
    {: >: { see(I, #`Barrel 1`) } :}

    on Init =>
    {
        'Begin' >> @>log;
        
        if({: >: { see(I, #`Barrel 1`) } :})
        {
            'Yes!' >> @>log;
        } else {
            'Else Yes!' >> @>log;
        }

        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
    {: >: { see(I, #`Barrel 0`) } :}

    on Init =>
    {
        'Begin' >> @>log;
        
        if({: >: { see(I, #`Barrel 0`) } :})
        {
            'Yes!' >> @>log;
        } elif ({: >: { see(I, #`Barrel 1`) } :}) {
            'Elif 1 Yes!' >> @>log;
        }

        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);

            n++;

            text = @"app PeaceKeeper
{
    {: >: { see(I, #`Barrel 3`) } :}

    on Init =>
    {
        'Begin' >> @>log;
        
        if({: >: { see(I, #`Barrel 0`) } :})
        {
            'Yes!' >> @>log;
        } elif ({: >: { see(I, #`Barrel 1`) } :}) {
            'Elif 1 Yes!' >> @>log;
        }elif ({: >: { see(I, #`Barrel 2`) } :}) {
            'Elif 2 Yes!' >> @>log;
        }else{
            'Else Yes!' >> @>log;
        }

        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);
        }

        private void ContinueLoopStatement()
        {
            var prefix = "ContinueLoopStatement";

            var n = 1;

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
        
        @a = 10;

        repeat
        {
            @a >> @>log;
            @a = @a - 1;

            if(@a > 5)
            {
                continue;
            }

            'End of while iteration' >> @>log;

            break;
        }

        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);
        }

        private void BreakLoopStatement()
        {
            var prefix = "BreakLoopStatement";

            var n = 1;

            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;
        
        @a = 10;

        while (@a > 0)
        {
            @a >> @>log;
            @a = @a - 1;

            if(@a > 5)
            {
                break;
            }
        }

        'End' >> @>log;
    }
}";

            Example(CreateName(prefix, n), text);
        }
    }
}
