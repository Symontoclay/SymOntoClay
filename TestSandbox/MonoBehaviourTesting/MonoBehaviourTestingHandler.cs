using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.MonoBehaviourTesting
{
    public class MonoBehaviourTestingHandler
    {
        private readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            var componentsList = new List<TstMonoBehaviour>() { new TstHumanoidNPC(), new TstWord(), /*new TstGameObject()*/ };
            ExecuteList(componentsList);

            _logger.Log("End");
        }

        private void ExecuteList(List<TstMonoBehaviour> componentsList)
        {
            _logger.Log("Begin");

            foreach(var component in componentsList)
            {
                component.Awake();
            }

            foreach (var component in componentsList)
            {
                component.Start();
            }

            Thread.Sleep(5000);

            for (var i = 1; i <= 100; i++)
            {
                foreach (var component in componentsList)
                {
                    component.Update();
                }

                Thread.Sleep(100);
            }

            Thread.Sleep(50000);

            foreach (var component in componentsList)
            {
                component.Stop();
            }

            _logger.Log("End");
        }
    }
}
