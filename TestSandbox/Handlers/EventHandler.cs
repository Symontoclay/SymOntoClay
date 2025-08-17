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
using System;

namespace TestSandbox.Handlers
{
    public class EventHandler
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImplementation();
        
        event Func<int, int> OnEv;

        public void Run()
        {
            _logger.Info("2DB9BB89-78E8-418C-8C6C-709687A9E410", "Begin");

            OnEv += Handler1;
            OnEv += Handler2;
            OnEv += Handler3;

            var rez = OnEv(12);

            _logger.Info("E4F886BF-4F22-4DC5-98B6-4A1E2A5BBC35", $"rez = {rez}");

            foreach(var item in OnEv.GetInvocationList())
            {
                var r = item.DynamicInvoke(16);

                _logger.Info("13594AB6-B098-4CB7-A324-41BC9E3206D7", $"r = {r}");
            }

            _logger.Info("54AF39EC-8861-4DC7-B2A6-35C31C5C5123", "End");
        }

        private int Handler1(int value)
        {
            _logger.Info("A160679D-1FE1-45D3-B367-ADBBDC9C3ED0", $"value = {value}");

            return 1;
        }

        private int Handler2(int value)
        {
            _logger.Info("A1EB0163-C9A0-4DFF-AC64-8878C4181D4D", $"value = {value}");

            return 2;
        }

        private int Handler3(int value)
        {
            _logger.Info("4775F876-EB82-4153-8936-8A331BE61901", $"value = {value}");

            return 3;
        }
    }
}
