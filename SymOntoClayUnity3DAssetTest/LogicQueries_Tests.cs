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

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class LogicQueries_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            var text = @"app PeaceKeeper
{
	{: male(#Tom) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case1_a()
        {
            var text = @"synonym procreator for parent;

app PeaceKeeper
{
	{: male(#Tom) :}
	{: procreator(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case1_b()
        {
            var text = @"synonym procreator for parent;
synonym #Tom for #Person123;

app PeaceKeeper
{
	{: male(#Tom) :}
	{: procreator(#Peter, #Person123) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case2()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	{: male(#Tom) :}
	{: parent(#Peter, #Tom) :}
	
	on Enter => {
	    select {: son($x, $y) :} >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case3()
        {
            var text = @"app PeaceKeeper
{
	{: male(#Tom) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son(#Tom, $y) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case3_a()
        {
            var text = @"app PeaceKeeper
{
	{: male(#Tom) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}
    @a = #Tom;
	on Enter => {
	    select {: son(@a, $y) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(false, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case4()
        {
            var text = @"app PeaceKeeper
{
	{: male(#Tom) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    ? {: son(#Tom, $y) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(false, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case5()
        {
            var text = @"app PeaceKeeper
{
    {: can(bird, fly) :}
    {: bird(#Alisa_12) :}

    on Enter =>
    {
        ? {: can(#Alisa_12, $x) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = fly"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case6()
        {
            var text = @"app PeaceKeeper
{
        {: can(bird, fly) :}
        {: bird(#Alisa_12) :}

    on Enter =>
    {
        ? {: can(#Alisa_12, fly) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case7()
        {
            var text = @"app PeaceKeeper
{
    {: can(bird, fly) :}
    {: bird(#Alisa_12) :}

    on Enter =>
    {
        ? {: $z(#Alisa_12, $x) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$z = can(bird,fly)"));
                            Assert.AreEqual(false, message.Contains("$x"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case8()
        {
            var text = @"app PeaceKeeper
{
    {: >: { see(I, #`Barrel 1`) } :}

    on Enter =>
    {
        ? {: see(I, #`Barrel 1`) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case9()
        {
            var text = @"app PeaceKeeper
{
    {: barrel(#a) :}
    {: see(I, #a) :}

    on Enter =>
    {
        select {: see(I, barrel) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case9_a()
        {
            var text = @"app PeaceKeeper
{
    {: is (#a, barrel) :}
    {: see(I, #a) :}

    on Enter =>
    {
        select {: see(I, barrel) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case10()
        {
            var text = @"app PeaceKeeper
{
    {: animal(cat) :}

    on Enter =>
    {
        select {: { cat is animal } :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case10_a()
        {
            var text = @"app PeaceKeeper
{
    {: { animal(cat) } :}

    on Enter =>
    {
        select {: { cat is animal } :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case10_b()
        {
            var text = @"app PeaceKeeper
{
    {: animal(cat) :}

    on Enter =>
    {
        select {: cat is animal :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case10_c()
        {
            var text = @"app PeaceKeeper
{
    {: animal(cat) :}

    on Enter =>
    {
        select {: >: { cat is animal } :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case11()
        {
            var text = @"app PeaceKeeper
{
    {: animal(cat) :}
    {: is (#a, barrel) :}
    {: see(I, #a) :}

    on Enter =>
    {
        select {: $z($x, $y) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$z = see(i,#a)"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case11_a()
        {
            var text = @"app PeaceKeeper
{
    {: animal(cat) :}
    {: is (#a, barrel) :}
    {: see(I, #a) :}

    on Enter =>
    {
        select {: $z($x) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = cat; $z = animal"));
                            Assert.AreEqual(true, message.Contains("$x = #a; $z = barrel"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case12()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 50) :}

    on Enter =>
    {
        select {: age(#Tom, `teenager`) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case12_a()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 15) :}

    on Enter =>
    {
        select {: age(#Tom, `teenager`) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case12_b()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 12) :}

    on Enter =>
    {
        select {: age(#Tom, very `teenager`) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case12_c()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 12) :}

    on Enter =>
    {
        select {: age(#Tom, very teenager) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case12_d()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 12) :}

    on Enter =>
    {
        select {: age(#Tom, `very` `teenager`) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case12_e()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, very `teenager`) :}

    on Enter =>
    {
        select {: age(#Tom, 12) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case12_f()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, `teenager`) :}

    on Enter =>
    {
        select {: age(#Tom, 12) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case12_g()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, teenager) :}

    on Enter =>
    {
        select {: age(#Tom, 12) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case12_h()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, `teenager`) :}

    on Enter =>
    {
        select {: age(#Tom, 50) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case12_j()
        {
            var text = @"linvar age for range (0, 150]
{
	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 15) :}

    on Enter =>
    {
        select {: age(#Tom, `teenager`) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case12_k()
        {
            var text = @"linvar age
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 15) :}

    on Enter =>
    {
        select {: age(#Tom, `teenager`) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case12_i()
        {
            var text = @"linvar age
{
	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 15) :}

    on Enter =>
    {
        select {: age(#Tom, `teenager`) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case12_l()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation color;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 15) :}

    on Enter =>
    {
        select {: age(#Tom, `teenager`) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case13()
        {
            var text = @"app PeaceKeeper
{
    {: >: {distance($x, $y)} -> { distance(I, $x, $y) } :}
    {: age(#Tom, 50) :}
    {: distance(I, #Tom, 12) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & distance(#Tom, $y) & $x is not $y :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = 12"));
                            Assert.AreEqual(true, message.Contains("$x = 50"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case13_a()
        {
            var text = @"app PeaceKeeper
{
    {: >: {distance($x, $y)} -> { distance(I, $x, $y) } :}
    {: age(#Tom, 12) :}
    {: distance(I, #Tom, 12) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & distance(#Tom, $y) & $x is not $y :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case14()
        {
            var text = @"app PeaceKeeper
{
    {: >: {distance($x, $y)} -> { distance(I, $x, $y) } :}
    {: distance(I, #Tom, 12) :}

    on Enter =>
    {
        select {: distance(#Tom, $x) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = 12"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case15()
        {
            var text = @"app PeaceKeeper
{
    {: >: {distance($x, $y)} -> { distance(I, $x, $y) } :}
    {: distance(I, #Tom, 12) :}

    on Enter =>
    {
        select {: distance(#Tom, $x) & $x is 12 :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = 12"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case15_a()
        {
            var text = @"app PeaceKeeper
{
    {: >: {distance($x, $y)} -> { distance(I, $x, $y) } :}
    {: distance(I, #Tom, 50) :}

    on Enter =>
    {
        select {: distance(#Tom, $x) & $x is 12 :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case16()
        {
            var text = @"app PeaceKeeper
{
    {: >: {distance($x, $y)} -> { distance(I, $x, $y) } :}
    {: distance(I, #Tom, 50) :}

    on Enter =>
    {
        select {: distance(#Tom, $x) & $x > 5 :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = 50"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case16_a()
        {
            var text = @"app PeaceKeeper
{
    {: >: {distance($x, $y)} -> { distance(I, $x, $y) } :}
    {: distance(I, #Tom, 5) :}

    on Enter =>
    {
        select {: distance(#Tom, $x) & $x > 5 :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case16_b()
        {
            var text = @"app PeaceKeeper
{
    {: >: {distance($x, $y)} -> { distance(I, $x, $y) } :}
    {: distance(I, #Tom, 4) :}

    on Enter =>
    {
        select {: distance(#Tom, $x) & $x > 5 :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case17()
        {
            var text = @"app PeaceKeeper
{
    {: >: {distance($x, $y)} -> { distance(I, $x, $y) } :}
    {: distance(I, #Tom, 50) :}

    on Enter =>
    {
        select {: distance(#Tom, $x) & $x >= 5 :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = 50"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case17_a()
        {
            var text = @"app PeaceKeeper
{
    {: >: {distance($x, $y)} -> { distance(I, $x, $y) } :}
    {: distance(I, #Tom, 5) :}

    on Enter =>
    {
        select {: distance(#Tom, $x) & $x >= 5 :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = 5"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case17_b()
        {
            var text = @"app PeaceKeeper
{
    {: >: {distance($x, $y)} -> { distance(I, $x, $y) } :}
    {: distance(I, #Tom, 4) :}

    on Enter =>
    {
        select {: distance(#Tom, $x) & $x >= 5 :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case18()
        {
            var text = @"app PeaceKeeper
{
    {: >: {distance($x, $y)} -> { distance(I, $x, $y) } :}
    {: distance(I, #Tom, 50) :}

    on Enter =>
    {
        select {: distance(#Tom, $x) & $x < 5 :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case18_a()
        {
            var text = @"app PeaceKeeper
{
    {: >: {distance($x, $y)} -> { distance(I, $x, $y) } :}
    {: distance(I, #Tom, 5) :}

    on Enter =>
    {
        select {: distance(#Tom, $x) & $x < 5 :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case18_b()
        {
            var text = @"app PeaceKeeper
{
    {: >: {distance($x, $y)} -> { distance(I, $x, $y) } :}
    {: distance(I, #Tom, 4) :}

    on Enter =>
    {
        select {: distance(#Tom, $x) & $x < 5 :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = 4"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case19()
        {
            var text = @"app PeaceKeeper
{
    {: >: {distance($x, $y)} -> { distance(I, $x, $y) } :}
    {: distance(I, #Tom, 50) :}

    on Enter =>
    {
        select {: distance(#Tom, $x) & $x <= 5 :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case19_a()
        {
            var text = @"app PeaceKeeper
{
    {: >: {distance($x, $y)} -> { distance(I, $x, $y) } :}
    {: distance(I, #Tom, 5) :}

    on Enter =>
    {
        select {: distance(#Tom, $x) & $x <= 5 :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = 5"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case19_b()
        {
            var text = @"app PeaceKeeper
{
    {: >: {distance($x, $y)} -> { distance(I, $x, $y) } :}
    {: distance(I, #Tom, 4) :}

    on Enter =>
    {
        select {: distance(#Tom, $x) & $x <= 5 :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = 4"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case20()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 50) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x > `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = 50"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case20_a()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 15) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x > `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case20_b()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 4) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x > `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case21()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 50) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x >= `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = 50"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case21_a()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 15) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x >= `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = 15"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case21_b()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 4) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x >= `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case22()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 50) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x < `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case22_a()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 15) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x < `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case22_b()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 4) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x < `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = 4"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case23()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 50) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x <= `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case23_a()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 15) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x <= `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = 15"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case23_b()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 4) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x <= `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = 4"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case24()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 50) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x > very `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = 50"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case24_a()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 15) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x > very `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case24_b()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 4) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x > very `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case25()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 50) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x >= very `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = 50"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case25_a()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 15) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x >= very `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = 15"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case25_b()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 4) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x >= very `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case26()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 50) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x < very `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case26_a()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 15) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x < very `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case26_b()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 4) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x < very `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = 4"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case27()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 50) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x <= very `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case27_a()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 15) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x <= very `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = 15"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case27_b()
        {
            var text = @"linvar age for range (0, 150]
{
	constraints:
		for relation age;

	terms:
        `teenager` = Trapezoid(10, 12, 17, 20);
}

app PeaceKeeper
{
    {: age(#Tom, 4) :}

    on Enter =>
    {
        select {: age(#Tom, $x) & $x <= very `teenager` :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = 4"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case28()
        {
            var text = @"app PeaceKeeper
{
    on Enter =>
    {
        ? {: bird ($x) :} >> @>log;
        insert {: >: { bird (#1234) } :};
        ? {: bird ($x) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return true;

                        case 2:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = #1234"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(2, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case29()
        {
            var text = @"app PeaceKeeper
{
    {: gun(M4A1) :}

    on Enter
    {
        insert {: $x = act(M4A1, shoot) & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :};
    }

    on {: hear(I, $x) & gun($x) & distance(I, $x, $y) :} ($x >> @x, $y >> @y)
    {
        @x >> @>log;
        @y >> @>log;
        '!!!M4A1!!!!' >> @>log;
    }
}";

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceTimeoutBased(text,
                (n, message) =>
                {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual("m4a1", message);
                            break;

                        case 2:
                            Assert.AreEqual(true, message.StartsWith("15.588"));
                            break;

                        case 3:
                            Assert.AreEqual("!!!M4A1!!!!", message);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));
        }

        [Test]
        [Parallelizable]
        public void Case30()
        {
            var text = @"app PeaceKeeper
{
	{: male(#Tom) o: 1 :}
	{: parent(#Peter, #Tom) o: 1 :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y) o: 1 :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case30_a()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	{: male(#Tom) o: middle :}
	{: parent(#Peter, #Tom) o: middle :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y)  o: middle :} >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case30_b()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	{: male(#Tom) o: 0.5 :}
	{: parent(#Peter, #Tom) o: 0.5 :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y)  o: middle :} >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case30_c()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	{: male(#Tom) o: 0.5 :}
	{: parent(#Peter, #Tom) o: 0.5 :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y)  o: very middle :} >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case30_d()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

 	{: male(#Tom) o: very middle :}
	{: parent(#Peter, #Tom) o: very middle :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y)  o: very middle :} >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case30_e()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	{: male(#Tom) o: very middle :}
	{: parent(#Peter, #Tom) o: very middle :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y)  o: middle :} >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case30_f()
        {
            var text = @"app PeaceKeeper
{
	{: male(#Tom) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y)  o: middle :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case31()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	{: male(#Tom) o: very middle :}
	{: parent(#Peter, #Tom) o: very middle :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y)  o: { _ is middle } :} >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case31_1()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	{: male(#Tom) o: very middle :}
	{: parent(#Peter, #Tom) o: very middle :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y)  o: { _ is 0.5 } :} >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case31_2()
        {
            var text = @"app PeaceKeeper
{
	{: male(#Tom) o: very middle :}
	{: parent(#Peter, #Tom) o: very middle :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y)  o: { _ is very middle } :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case31_a()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	{: male(#Tom) o: very middle :}
	{: parent(#Peter, #Tom) o: very middle :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y)  o: { _ is not middle } :} >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case31_b()
        {
            var text = @"app PeaceKeeper
{
	{: male(#Tom) o: very middle :}
	{: parent(#Peter, #Tom) o: very middle :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y)  o: { _ is not low } :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case31_c()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	{: male(#Tom) o: very middle :}
	{: parent(#Peter, #Tom) o: very middle :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y)  o: { _ > low } :} >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case31_d()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	{: male(#Tom) o: very middle :}
	{: parent(#Peter, #Tom) o: very middle :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y)  o: { _ >= low } :} >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case31_e()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	{: male(#Tom) o: very middle :}
	{: parent(#Peter, #Tom) o: very middle :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y)  o: { _ < high } :} >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case31_f()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	{: male(#Tom) o: very middle :}
	{: parent(#Peter, #Tom) o: very middle :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y)  o: { _ <= high } :} >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case31_g()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	{: male(#Tom) o: very middle :}
	{: parent(#Peter, #Tom) o: very middle :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y)  o: { not ( _ < high ) } :} >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case31_g_1()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	{: male(#Tom) o: very middle :}
	{: parent(#Peter, #Tom) o: very middle :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y)  o: { ! ( _ < high ) } :} >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case31_h()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	{: male(#Tom) o: very middle :}
	{: parent(#Peter, #Tom) o: very middle :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y)  o: { _ < high and _ > low } :} >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case31_h_1()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	{: male(#Tom) o: very middle :}
	{: parent(#Peter, #Tom) o: very middle :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y)  o: { _ < high and _ > low } :} >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case31_j()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	{: male(#Tom) o: very middle :}
	{: parent(#Peter, #Tom) o: very middle :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y)  o: { _ < high or _ > low } :} >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case31_j_1()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	{: male(#Tom) o: very middle :}
	{: parent(#Peter, #Tom) o: very middle :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: son($x, $y)  o: { _ < high | _ > low } :} >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case32()
        {
            var text = @"app PeaceKeeper
{
    {: gun(M16) :}
	{: $x = act(M16, shoot) & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}

	on Enter => {
	    select {: hear(I, $x) & gun($x) & distance(I, $x, $y) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = 15.588457107543945"));
                            Assert.AreEqual(true, message.Contains("$x = m16"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case32_a()
        {
            var text = @"app PeaceKeeper
{
    {: gun(M16) :}
	{: $x = (act(M16, shoot) & act(M4A1, shoot)) & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}

	on Enter => {
	    select {: hear(I, $x) & gun($x) & distance(I, $x, $y) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = 15.588457107543945"));
                            Assert.AreEqual(true, message.Contains("$x = m16"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case33()
        {
            var text = @"app PeaceKeeper
{
    {: gun(M16) :}
	{: $x = {: act(M16, shoot) :} & hear(I, $x) & distance(I, $x, 15.588457107543945) & direction($x, 12) & point($x, #@[15.588457107543945, 12]) :}

	on Enter => {
	    select {: hear(I, $x) & gun($x) & distance(I, $x, $y) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = 15.588457107543945"));
                            Assert.AreEqual(true, message.Contains("$x = m16"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case34()
        {
            var text = @"app PeaceKeeper
{
	{: >: { (act(#player,walk)) & hear(i,(act(#player,walk))) & distance(i,(act(#player,walk)),14.051362991333) & direction((act(#player,walk)),15.83357) & point((act(#player,walk)),#@[14.05136, 15.83357]) } :}

	on Enter => {
	    select {: >: { hear(i,q5) } :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case34_a()
        {
            var text = @"app PeaceKeeper
{
	{: >: { (act(q13,shoot)) & hear(i,(act(q13,shoot))) & distance(i,(act(q13,shoot)),4.35098171234131) & direction((act(q13,shoot)),21.56101) & point((act(q13,shoot)),#@[4.350982, 21.56101]) } :}

	on Enter => {
	    select {: >: { hear(i,q13) } :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case35()
        {
            var text = @"app PeaceKeeper
{
	{: soldier(#Tom) :}
	{: state(#Tom, alive) :}
	{: see(I, #Tom) :}
	{: { enemy($x) } -> { soldier($x) } :}

	on Enter => {
	    select {: see(I, $x) & enemy($x) & state($x, alive) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case36()
        {
            var text = @"app PeaceKeeper
{
	{: female(#Mary) :}
	{: male(#Tom) :}
	{: male(#Mark) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: male($x) & !son($x, $y) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #mark"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case36_a()
        {
            var text = @"app PeaceKeeper
{
	{: female(#Mary) :}
	{: male(#Tom) :}
	{: male(#Mark) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: !son($x, $y) & male($x) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$y = #peter"));
                            Assert.AreEqual(true, message.Contains("$x = #mark"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case37()
        {
            var text = @"app PeaceKeeper
{
	{: female(#Mary) :}
	{: male(#Tom) :}
	{: male(#Mark) :}
	{: male(null) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: male($x) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            Assert.AreEqual(true, message.Contains("$x = #mark"));
                            Assert.AreEqual(true, message.Contains("$x = NULL"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case37_a()
        {
            var text = @"app PeaceKeeper
{
	{: female(#Mary) :}
	{: male(#Tom) :}
	{: male(#Mark) :}
	{: male(null) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: male($x) & $x is null :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = NULL"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case37_a_1()
        {
            var text = @"app PeaceKeeper
{
    import 'stdlib';

	{: female(#Mary) :}
	{: male(#Tom) :}
	{: male(#Mark) :}
	{: male(null) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: male($x) & $x is not null :} >> @>log;
	}
}";

            var maxN = 0;

            Assert.AreEqual(true, BehaviorTestEngineRunner.RunMinimalInstanceWithImportStandardLibrary(text,
                (n, message) =>
                {
                    maxN = n;

                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            Assert.AreEqual(true, message.Contains("$x = #mark"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case37_b()
        {
            var text = @"app PeaceKeeper
{
	{: female(#Mary) :}
	{: male(#Tom) :}
	{: male(#Mark) :}
	{: male(null) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: male($x) & @_target is NULL :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            Assert.AreEqual(true, message.Contains("$x = #mark"));
                            Assert.AreEqual(true, message.Contains("$x = NULL"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case37_b_1()
        {
            var text = @"app PeaceKeeper
{
	{: female(#Mary) :}
	{: male(#Tom) :}
	{: male(#Mark) :}
	{: male(null) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
        @_target = 1;

	    select {: male($x) & @_target is NULL :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case37_c()
        {
            var text = @"app PeaceKeeper
{
	{: female(#Mary) :}
	{: male(#Tom) :}
	{: male(#Mark) :}
	{: male(null) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    select {: male($x) & @_target is not NULL :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<no>"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case37_c_1()
        {
            var text = @"app PeaceKeeper
{
	{: female(#Mary) :}
	{: male(#Tom) :}
	{: male(#Mark) :}
	{: male(null) :}
	{: parent(#Peter, #Tom) :}
	{: {son($x, $y)} -> { male($x) & parent($y, $x)} :}

	on Enter => {
	    @_target = 1;

	    select {: male($x) & @_target is not NULL :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = #tom"));
                            Assert.AreEqual(true, message.Contains("$x = #mark"));
                            Assert.AreEqual(true, message.Contains("$x = NULL"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }

        [Test]
        [Parallelizable]
        public void Case38()
        {
            var text = @"{: { enemy($x) } -> { soldier($x) } :}

{: waypoint(#wp1) :}
{: soldier(#enemy1) :}
{: soldier(#enemy2) :}

app PeaceKeeper
{
    on Enter =>
	{
		select {: waypoint($x) :} >> @>log;
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
                            Assert.AreEqual(true, message.Contains("<yes>"));
                            Assert.AreEqual(true, message.Contains("$x = #wp1"));
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }));

            Assert.AreEqual(1, maxN);
        }
    }
}
