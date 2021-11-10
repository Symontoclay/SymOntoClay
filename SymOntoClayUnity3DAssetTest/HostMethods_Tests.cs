using NUnit.Framework;
using SymOntoClay.UnityAsset.Core.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.UnityAsset.Core.Tests
{
    public class HostMethods_Tests
    {
        [Test]
        [Parallelizable]
        public void Case1()
        {
            throw new NotImplementedException();
        }

        [Test]
        [Parallelizable]
        public void Case2_a()
        {
            var text = @"app PeaceKeeper
{
    on Init =>
    {
        'Begin' >> @>log;

        @@host.`rotate`(30);

        'End' >> @>log;
    }
}";

            var hostListener = new HostMethods_Tests_HostListener();

            BehaviorTestEngineInstance.Run(text,
                (n, message) => {
                    //_logger.Log($"n = {n}; message = {message}");
                }, hostListener);

            /*
             n = 1; message = 'Begin'
10-11-2021 13:59:10.63296 TestSandbox.Program+<>c <TstTestRunnerWithHostListener>b__9_0 LOG n = 2; message = RotateImpl Begin
10-11-2021 13:59:10.63561 TestSandbox.Program+<>c <TstTestRunnerWithHostListener>b__9_0 LOG n = 3; message = 30
10-11-2021 13:59:10.73554 TestSandbox.Program+<>c <TstTestRunnerWithHostListener>b__9_0 LOG n = 4; message = 'End'
             */
        }

        [Test]
        [Parallelizable]
        public void Case2_b()
        {
            throw new NotImplementedException();
        }
    }
}
