/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$`y` = #`Peter`"));
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "19546649-1B48-4451-9E52-B6EF95F0DBDD");
                    }
                }));

            Assert.AreEqual(3, maxN);
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

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("`a` has been called!", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "67CD119D-D886-4A26-9597-0B1FF266AFE5");
                    }
                }));

            Assert.AreEqual(3, maxN);
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

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("`a` has been called!", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "5C4FAE01-28DD-4A1E-A1BD-3A34F9C25D44");
                    }
                }));

            Assert.AreEqual(3, maxN);
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

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("`a` has been called!", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "50EAE04E-D3C4-4136-ACDB-37128EBA781D");
                    }
                }));

            Assert.AreEqual(3, maxN);
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

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) => {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("`a` has been called!", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "15E63EE9-1DC6-4211-B243-ABEE310AF8C9");
                    }
                }));

            Assert.AreEqual(3, maxN);
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

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "E1E7543A-CF20-4414-A70B-2623D944EED4");
                    }
                }));

            Assert.AreEqual(3, maxN);
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

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$`y` = #`Peter`"));
                            Assert.AreEqual(true, message.Contains("$`x` = #`Tom`"));
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "2B7CBFC1-4BFE-4841-AAFE-59D33D0AD621");
                    }
                }));

            Assert.AreEqual(3, maxN);
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

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$`y` = #`Peter`"));
                            Assert.AreEqual(true, message.Contains("$`x` = #`Tom`"));
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "BB2C25FC-DAA4-475E-8746-8F6D686F2508");
                    }
                }));

            Assert.AreEqual(3, maxN);
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

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "531AEF18-A2A4-4E43-A5F7-531512511E7A");
                    }
                }));

            Assert.AreEqual(3, maxN);
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

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$`z` = `can`(`bird`,`fly`)"));
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "3E0B82D8-EDDC-401A-BDAD-26E4D678FEA2");
                    }
                }));

            Assert.AreEqual(3, maxN);
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

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$`z` = `can`(`bird`,`fly`)"));
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "621B263A-9D23-4D2F-9845-92FA9620D41F");
                    }
                }));

            Assert.AreEqual(3, maxN);
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

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("3", message);
                            return true;

                        case 2:
                            Assert.AreEqual("1", message);
                            return true;

                        case 3:
                            Assert.AreEqual("0", message);
                            return true;

                        case 4:
                            Assert.AreEqual("2", message);
                            return true;

                        case 5:
                            Assert.AreEqual("0", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "843C18B7-546E-4FCB-A5C8-88C181A930E9");
                    }
                }));

            Assert.AreEqual(5, maxN);
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

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstance(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("Begin", message);
                            return true;

                        case 2:
                            Assert.AreEqual("1", message);
                            return true;

                        case 3:
                            Assert.AreEqual("End", message);
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, "A96E9588-5CA6-48BD-95C8-77A8C6626604");
                    }
                }));

            Assert.AreEqual(3, maxN);
        }
    }
}
