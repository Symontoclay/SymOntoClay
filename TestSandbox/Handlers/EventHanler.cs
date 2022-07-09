using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class EventHanler
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        event Func<int, int> OnEv;

        public void Run()
        {
            _logger.Log("Begin");

            OnEv += Handler1;
            OnEv += Handler2;
            OnEv += Handler3;

            var rez = OnEv(12);

            _logger.Log($"rez = {rez}");

            foreach(var item in OnEv.GetInvocationList())
            {
                var r = item.DynamicInvoke(16);

                _logger.Log($"r = {r}");
            }

            _logger.Log("End");
        }

        private int Handler1(int value)
        {
            _logger.Log($"value = {value}");

            return 1;
        }

        private int Handler2(int value)
        {
            _logger.Log($"value = {value}");

            return 2;
        }

        private int Handler3(int value)
        {
            _logger.Log($"value = {value}");

            return 3;
        }
    }
}
