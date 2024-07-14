using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using System.Threading.Tasks;

namespace TestSandbox.MethodResponsing
{
    public class MethodResponsingHandler
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        public void Run()
        {
            _logger.Info("62C75AA7-3C1E-422D-BD21-785D941E21BE", "Begin");

            CaseWithIntResult();
            //CaseWithoutResult();

            _logger.Info("07159A1E-0F63-4EAA-B40A-1E2075310C0A", "End");
        }

        private void CaseWithIntResult()
        {
            var methodResponse = SomeIntMethod();

            methodResponse.Task.Wait();

            var result = methodResponse.Result;

            _logger.Info("0E0F952F-1992-4FFA-8387-B78FA674328F", $"result = {result}");
        }

        private IMethodResponse<int> SomeIntMethod()
        {
            _logger.Info("D70E947A-25D2-4DEA-9493-0BDDAAA67D9D", "Run");

            return new TstIntMethodResponse(Task<int>.FromResult(16));
        }

        private void CaseWithoutResult()
        {
            var methodResponse = SomeMethod();
        }

        private IMethodResponse SomeMethod()
        {
            _logger.Info("0AFED997-8094-419F-BFE5-D8A2EA410026", "Run");

            return new TstMethodResponse(Task.CompletedTask);
        }
    }
}
