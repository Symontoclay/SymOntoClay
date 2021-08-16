/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class ErrorsProcessing_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
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
        'Begin' >> @>log;
        a(param_1: 1);
        'End' >> @>log;
    }

    on {: see(I, $x) :} ($x >> @x) => 
    {
        'on Fired' >> @>log;
        @x >> @>log;
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
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "on Fired");
                            break;

                        case 5:
                            Assert.AreEqual(message, "#a");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case2_a()
        {
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

        'End of `Init`' >> @>log;
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
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "catch");
                            break;

                        case 5:
                            Assert.AreEqual(message, "End of `Init`");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case2_a_1()
        {
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

        'End of `Init`' >> @>log;
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
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End of `Init`");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case2_b()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1) =>
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;

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
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End of `a`");
                            break;

                        case 5:
                            Assert.AreEqual(message, "End");
                            break;

                        case 6:
                            Assert.AreEqual(message, "End of `Init`");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case2_b_1()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1) =>
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;

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
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End of `a`");
                            break;

                        case 5:
                            Assert.AreEqual(message, "End");
                            break;

                        case 6:
                            Assert.AreEqual(message, "End of `Init`");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case2_c()
        {
            var text = @"app PeaceKeeper
{
    fun a(@param_1) =>
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;

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
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End of `a`");
                            break;

                        case 5:
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
        else
        {
            'else' >> @>log;
        }

        'End of `Init`' >> @>log;
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
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "catch");
                            break;

                        case 5:
                            Assert.AreEqual(message, "End of `Init`");
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
            var text = @"app PeaceKeeper
{
    fun a(@param_1) =>
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;

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
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End of `a`");
                            break;

                        case 5:
                            Assert.AreEqual(message, "End");
                            break;

                        case 6:
                            Assert.AreEqual(message, "else");
                            break;

                        case 7:
                            Assert.AreEqual(message, "End of `Init`");
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
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "catch");
                            break;

                        case 5:
                            Assert.AreEqual(message, "ensure");
                            break;

                        case 6:
                            Assert.AreEqual(message, "End of `Init`");
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
            var text = @"app PeaceKeeper
{
    fun a(@param_1) =>
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;

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
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End of `a`");
                            break;

                        case 5:
                            Assert.AreEqual(message, "End");
                            break;

                        case 6:
                            Assert.AreEqual(message, "else");
                            break;

                        case 7:
                            Assert.AreEqual(message, "ensure");
                            break;

                        case 8:
                            Assert.AreEqual(message, "End of `Init`");
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
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "catch(@e)");
                            break;

                        case 5:
                            Assert.AreEqual(true, message.Contains("ERROR:"));
                            Assert.AreEqual(true, message.Contains("{:"));
                            Assert.AreEqual(true, message.Contains("see(i,#a)"));
                            Assert.AreEqual(true, message.Contains(":}"));
                            break;

                        case 6:
                            Assert.AreEqual(message, "ensure");
                            break;

                        case 7:
                            Assert.AreEqual(message, "End of `Init`");
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
            var text = @"app PeaceKeeper
{
    fun a(@param_1) =>
    {
        '`a` has been called!' >> @>log;
        @param_1 >> @>log;

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
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End of `a`");
                            break;

                        case 5:
                            Assert.AreEqual(message, "End");
                            break;

                        case 6:
                            Assert.AreEqual(message, "else");
                            break;

                        case 7:
                            Assert.AreEqual(message, "ensure");
                            break;

                        case 8:
                            Assert.AreEqual(message, "End of `Init`");
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
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "catch(@e)");
                            break;

                        case 5:
                            Assert.AreEqual(true, message.Contains("ERROR:"));
                            Assert.AreEqual(true, message.Contains("{:"));
                            Assert.AreEqual(true, message.Contains("see(i,#a)"));
                            Assert.AreEqual(true, message.Contains(":}"));
                            break;

                        case 6:
                            Assert.AreEqual(message, "ensure");
                            break;

                        case 7:
                            Assert.AreEqual(message, "End of `Init`");
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
                            Assert.AreEqual(message, "1");
                            break;

                        case 4:
                            Assert.AreEqual(message, "catch(@e) where {: see(I, $x) :}");
                            break;

                        case 5:
                            Assert.AreEqual(true, message.Contains("ERROR:"));
                            Assert.AreEqual(true, message.Contains("{:"));
                            Assert.AreEqual(true, message.Contains("see(i,#a)"));
                            Assert.AreEqual(true, message.Contains(":}"));
                            break;

                        case 6:
                            Assert.AreEqual(message, "ensure");
                            break;

                        case 7:
                            Assert.AreEqual(message, "End of `Init`");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case8()
        {
            var text = @"app PeaceKeeper
{
    on Init =>
    {
        try
        {
            'Begin' >> @>log;
            error {: see(I, #a) :};
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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "catch(@e) where {: see(I, $x) :}");
                            break;

                        case 3:
                            Assert.AreEqual(true, message.Contains("ERROR:"));
                            Assert.AreEqual(true, message.Contains("{:"));
                            Assert.AreEqual(true, message.Contains("see(i,#a)"));
                            Assert.AreEqual(true, message.Contains(":}"));
                            break;

                        case 4:
                            Assert.AreEqual(message, "ensure");
                            break;

                        case 5:
                            Assert.AreEqual(message, "End of `Init`");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case9_a()
        {
            var text = @"app PeaceKeeper
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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "End of `Init`");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case9_b()
        {
            var text = @"app PeaceKeeper
{
    on Init =>
    {
        try
        {
            'Begin' >> @>log;
            'End' >> @>log;        
        }
        else
        {
            'else' >> @>log;
        }

        'End of `Init`' >> @>log;
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
                            Assert.AreEqual(message, "End");
                            break;

                        case 3:
                            Assert.AreEqual(message, "else");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End of `Init`");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case10_a()
        {
            var text = @"app PeaceKeeper
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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "ensure");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End of `Init`");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case10_b()
        {
            var text = @"app PeaceKeeper
{
    on Init =>
    {
        try
        {
            'Begin' >> @>log;
            'End' >> @>log;        
        }
        ensure
        {
            'ensure' >> @>log;
        }

        'End of `Init`' >> @>log;
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
                            Assert.AreEqual(message, "End");
                            break;

                        case 3:
                            Assert.AreEqual(message, "ensure");
                            break;

                        case 4:
                            Assert.AreEqual(message, "End of `Init`");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case11_a()
        {
            var text = @"app PeaceKeeper
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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "ensure");
                            break;

                        case 3:
                            Assert.AreEqual(message, "End of `Init`");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }

        [Test]
        [Parallelizable]
        public void Case11_b()
        {
            var text = @"app PeaceKeeper
{
    on Init =>
    {
        try
        {
            'Begin' >> @>log;
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

            Assert.AreEqual(BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    switch (n)
                    {
                        case 1:
                            Assert.AreEqual(message, "Begin");
                            break;

                        case 2:
                            Assert.AreEqual(message, "End");
                            break;

                        case 3:
                            Assert.AreEqual(message, "else");
                            break;

                        case 4:
                            Assert.AreEqual(message, "ensure");
                            break;

                        case 5:
                            Assert.AreEqual(message, "End of `Init`");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(n), n, null);
                    }
                }), true);
        }
    }
}
