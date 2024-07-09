using SymOntoClay.Core.Internal.Serialization.Functors;
using SymOntoClay.Core.Internal.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using SymOntoClay.Threading;
using System;
using System.Security.Cryptography;
using System.Threading;

namespace TestSandbox.Threads
{
    public class AsyncActiveOnceObjectHandler
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        public void Run()
        {
            _logger.Info("C8AFAFBA-F6BA-4D20-A913-D5B3E6E87BFF", "Begin");

            //TstFunctorWithResultCase();
            StringFunctorWithoutResultCase();
            //GeneralCase();

            _logger.Info("86A4093A-C550-46B0-954C-337793C1AE51", "End");
        }

        private void TstFunctorWithResultCase()
        {
            using var cancellationTokenSource = new CancellationTokenSource();

            using var threadPool = new CustomThreadPool(0, 20, cancellationTokenSource.Token);

            var commonActiveContext = new ActiveObjectCommonContext();

            var activeContext = new ActiveObjectContext(commonActiveContext, cancellationTokenSource.Token);

            var functor = new TstFunctorWithResult(_logger, () => { return 16; }, activeContext, threadPool);
            
            var methodResponse = functor.ToMethodResponse();

            commonActiveContext.Lock();

            activeContext.WaitWhenAllIsNotWaited();

            Thread.Sleep(1000);

            functor.Run();

            Thread.Sleep(1000);

            _logger.Info("714FB9D2-4136-4484-8BB3-D75E3D2DE475", "UnLock");

            commonActiveContext.UnLock();

            Thread.Sleep(1000);

            _logger.Info("A408BA10-917F-4A6F-88A6-561CD2969D7A", $"functor.Result = {functor.Result}");
            _logger.Info("E3CF4817-CA2F-4824-85B6-AD4DFCB657C8", $"methodResponse.Result = {methodResponse.Result}");
        }

        private void StringFunctorWithoutResultCase()
        {
            using var cancellationTokenSource = new CancellationTokenSource();

            using var threadPool = new CustomThreadPool(0, 20, cancellationTokenSource.Token);

            var commonActiveContext = new ActiveObjectCommonContext();

            var activeContext = new ActiveObjectContext(commonActiveContext, cancellationTokenSource.Token);

            var textOnceFunctor = new LoggedFunctorWithoutResult<string>(_logger, "Some query", (logger, text) => {
                logger.Info("02A2BB60-A4D1-4F1D-9A94-31644F054D79", text);
            }, activeContext, threadPool);

            commonActiveContext.Lock();

            activeContext.WaitWhenAllIsNotWaited();

            Thread.Sleep(1000);

            textOnceFunctor.Run();

            Thread.Sleep(1000);

            _logger.Info("35AE2744-A26B-4723-9733-5E01CF891391", "UnLock");

            commonActiveContext.UnLock();

            Thread.Sleep(1000);
        }

        private void GeneralCase()
        {
            using var cancellationTokenSource = new CancellationTokenSource();

            using var threadPool = new CustomThreadPool(0, 20, cancellationTokenSource.Token);

            var commonActiveContext = new ActiveObjectCommonContext();

            var activeContext = new ActiveObjectContext(commonActiveContext, cancellationTokenSource.Token);

            var asyncActiveOnceObject = new AsyncActiveOnceObject(activeContext, threadPool, _logger)
            {
                OnceMethod = OnceMethod
            };

            _logger.Info("FDF2B05C-751E-4283-AF78-F104A1E9FF5C", $"asyncActiveOnceObject.IsWaited (0) = {asyncActiveOnceObject.IsWaited}");
            _logger.Info("64431E64-D229-45E1-B89D-5F1D4687C19C", $"asyncActiveOnceObject.IsActive (0) = {asyncActiveOnceObject.IsActive}");

            commonActiveContext.Lock();

            activeContext.WaitWhenAllIsNotWaited();

            Thread.Sleep(1000);

            var taskValue = asyncActiveOnceObject.Start();

            _logger.Info("7D94A600-ADFB-45DF-9CEA-9AF8613B43FB", $"taskValue = {taskValue}");

            _logger.Info("2C72CEAD-CF9B-4AE2-B08F-D49A996DAEF0", $"asyncActiveOnceObject.IsWaited (2) = {asyncActiveOnceObject.IsWaited}");
            _logger.Info("9336B302-7F9D-49A1-9C94-E00995C0A383", $"asyncActiveOnceObject.IsActive (2) = {asyncActiveOnceObject.IsActive}");

            Thread.Sleep(1000);

            _logger.Info("90490637-82D7-4A98-8DBB-43CF419059EB", "UnLock");

            commonActiveContext.UnLock();

            Thread.Sleep(1000);

            _logger.Info("FC88A68A-AF2A-4EEF-9319-0F6218177B60", $"asyncActiveOnceObject.IsWaited (3) = {asyncActiveOnceObject.IsWaited}");
            _logger.Info("97C4C937-7E72-4D08-896C-5347AC7BD8EB", $"asyncActiveOnceObject.IsActive (3) = {asyncActiveOnceObject.IsActive}");

            Thread.Sleep(1000);
        }

        private void OnceMethod(CancellationToken cancellationToken)
        {
            _logger.Info("B126224F-FE70-454F-B568-A6BB65506B35", "Run!!!!");

            throw new NotImplementedException("7E694A2A-45A2-41FE-A8C1-A3261E57BB0B");
        }
    }
}
