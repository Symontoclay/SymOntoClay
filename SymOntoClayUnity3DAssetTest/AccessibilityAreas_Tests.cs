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

using NUnit.Framework;
using SymOntoClay.BaseTestLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class AccessibilityAreas_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            var text = @"app PeaceKeeper
{
private:
	{: male(#Tom) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    'Begin' >> @>log;
	    select {: son(@a, $y) :} >> @>log;
		'End' >> @>log;
	}

private:
	@a = #Tom;
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message.Contains("<yes>"), true);
                            Assert.AreEqual(message.Contains("$y = #peter"), true);
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            var text = @"app PeaceKeeper
{
private:
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }

    on Enter =>
    {
        'Begin' >> @>log;
        a();
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case3()
        {
            var text = @"class Cls1
{
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }
}

app PeaceKeeper is Cls1
{
private:
    on Enter =>
    {
        'Begin' >> @>log;
        a();
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case3_a()
        {
            var text = @"class Cls1
{
public:
    fun a() => 
    {
        '`a` has been called!' >> @>log;
    }
}

app PeaceKeeper is Cls1
{
private:
    on Enter =>
    {
        'Begin' >> @>log;
        a();
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case3_b()
        {
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
    on Enter =>
    {
        'Begin' >> @>log;
        a();
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "`a` has been called!");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case4()
        {
            var text = @"class Cls1
{
private:
	{: male(#Tom) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}
}

app PeaceKeeper is Cls1
{
private:
    on Enter =>
    {
        'Begin' >> @>log;
        select {: son($x, $y) :} >> @>log;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message.Contains("<no>"), true);
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case4_a()
        {
            var text = @"class Cls1
{
protected:
	{: male(#Tom) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}
}

app PeaceKeeper is Cls1
{
private:
    on Enter =>
    {
        'Begin' >> @>log;
        select {: son($x, $y) :} >> @>log;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message.Contains("<yes>"), true);
                            Assert.AreEqual(message.Contains("$y = #peter"), true);
                            Assert.AreEqual(message.Contains("$x = #tom"), true);
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case4_b()
        {
            var text = @"class Cls1
{
public:
	{: male(#Tom) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}
}

app PeaceKeeper is Cls1
{
private:
    on Enter =>
    {
        'Begin' >> @>log;
        select {: son($x, $y) :} >> @>log;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message.Contains("<yes>"), true);
                            Assert.AreEqual(message.Contains("$y = #peter"), true);
                            Assert.AreEqual(message.Contains("$x = #tom"), true);
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case5()
        {
            var text = @"class Cls1
{
private:
    {: can(bird, fly) :}
    {: bird(#Alisa_12) :}
}

app PeaceKeeper is Cls1
{
private:
    on Enter =>
    {
        'Begin' >> @>log;
        select {: $z(#Alisa_12, $x) :} >> @>log;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message.Contains("<no>"), true);
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case5_a()
        {
            var text = @"class Cls1
{
protected:
    {: can(bird, fly) :}
    {: bird(#Alisa_12) :}
}

app PeaceKeeper is Cls1
{
private:
    on Enter =>
    {
        'Begin' >> @>log;
        select {: $z(#Alisa_12, $x) :} >> @>log;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message.Contains("<yes>"), true);
                            Assert.AreEqual(message.Contains("$z = can(bird,fly)"), true);
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case5_b()
        {
            var text = @"class Cls1
{
public:
    {: can(bird, fly) :}
    {: bird(#Alisa_12) :}
}

app PeaceKeeper is Cls1
{
private:
    on Enter =>
    {
        'Begin' >> @>log;
        select {: $z(#Alisa_12, $x) :} >> @>log;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message.Contains("<yes>"), true);
                            Assert.AreEqual(message.Contains("$z = can(bird,fly)"), true);
                            break;

                        case 3:
                            Assert.AreEqual(message, "End");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case6()
        {
            var text = @"class cls0
{
    fun SomeFun0()
    {
        @b >> @>log;
    }

    private:
        @b = 0;
}

class cls1 is cls0
{
    fun SomeFun1()
    {
        @b >> @>log;

        SomeFun0();
    }

    private:
        @b = 1;
}

class cls2 is cls0
{
    fun SomeFun2()
    {
        @b >> @>log;

        SomeFun0();
    }

    private:
        @b = 2;
}

app PeaceKeeper is cls1, cls2
{
    on Enter
    {
        @b >> @>log;

        SomeFun1();
        SomeFun2();
    }

    private:
        @b = 3;
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("3", message);
                            break;

                        case 2:
                            Assert.AreEqual("1", message);
                            break;

                        case 3:
                            Assert.AreEqual("0", message);
                            break;

                        case 4:
                            Assert.AreEqual("2", message);
                            break;

                        case 5:
                            Assert.AreEqual("0", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case7()
        {
            var text = @"class cls0
{
    private:
        @b = 0;
}

class cls1 is cls0
{
    private:
        @b = 1;
}

app PeaceKeeper
{
    on Enter
    {
        'Begin' >> @>log;
        @a = new cls1;
        @a.@b >> @>log;
        'End' >> @>log;
    }
}";

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            break;

                        case 2:
                            Assert.AreEqual("1", message);
                            break;

                        case 3:
                            Assert.AreEqual("End", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }
    }
}
