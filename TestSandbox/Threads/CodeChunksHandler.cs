using SymOntoClay.ActiveObject.CodeChunks;
using SymOntoClay.ActiveObject.CodeChunks.Implementation;
using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using SymOntoClay.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace TestSandbox.Threads
{
    public class CodeChunksHandlerSyncMethodForCase9LocalContext
    {
    }

    public class CodeChunksHandlerSyncMethodWithResultForCase9LocalContext
    {
    }

    public class CodeChunksHandlerCase3LocalContext
    {
        public int Property1 { get; set; }
    }

    public class CodeChunksHandlerGlobalContext
    {
        public string GlobalStr { get; set; }
    }

    public class CodeChunksHandlerLocalContext
    {
        public int Property1 { get; set; }
    }

    public class CodeChunksHandler
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        public void Run()
        {
            _logger.Info("E7E3E621-38BD-4B54-B041-FD0C74FE5BBF", "Begin");

            //Case10();
            Case9();
            //Case8();
            //Case7();
            //Case6();
            //Case5();
            //Case4();
            //Case3();
            //Case2();
            //Case1();

            _logger.Info("B7E2F374-D3BC-48B5-B0F2-357590821EAE", "End");
        }

        //private void Case10()
        //{
        //    //using var cancellationTokenSource = new CancellationTokenSource();

        //    //using var threadPool = new CustomThreadPool(0, 20, cancellationTokenSource.Token);

        //    //var commonActiveContext = new ActiveObjectCommonContext();

        //    //var activeContext = new ActiveObjectContext(commonActiveContext, cancellationTokenSource.Token);

        //    //var instance = new TstInstance(_logger, activeContext, threadPool);

        //    //var methodResult = instance.Method2(11);

        //    //var result = methodResult.Result;

        //    //_logger.Info("A560B79C-93FD-42C3-9B75-1AFBE2679486", $"result = {result}");

        //    //Thread.Sleep(10000);
        //}

        //private void Case9()
        //{
        //    //using var cancellationTokenSource = new CancellationTokenSource();

        //    //using var threadPool = new CustomThreadPool(0, 20, cancellationTokenSource.Token);

        //    //var commonActiveContext = new ActiveObjectCommonContext();

        //    //var activeContext = new ActiveObjectContext(commonActiveContext, cancellationTokenSource.Token);

        //    //var instance = new TstInstance(_logger, activeContext, threadPool);

        //    //var result = instance.Method1();
        //    //result.Wait();

        //    //Thread.Sleep(10000);
        //}

        private void Case9()
        {
            var codeChunksContext = new CodeChunksContext<IMonitorLogger, CodeChunksHandler>(_logger, this);

            codeChunksContext.CreateCodeChunk("AE6FB64B-40B3-4BF5-92E3-99B6F624C84C", (IMonitorLogger loggerValue, CodeChunksHandler instance) =>
            {
                loggerValue.Info("29B0879D-CAD1-4B61-8D65-B235FCC10D5E", "Chunk1");
            });

            codeChunksContext.CreateSyncCall("B7F7C4B0-EF14-46D9-9CC1-9B307A6F3F07", ISyncMethodResponse (IMonitorLogger loggerValue, CodeChunksHandler instance) => {
                loggerValue.Info("E47E339B-BDFD-4C2E-88D2-8C1DD60ED4A1", "Chunk2");

                return instance.SyncMethodForCase9(loggerValue, instance);
            });

            codeChunksContext.CreateSyncCall<int>("7CADB8B3-A559-4B4F-A8F7-9B90A24C1DDA", ISyncMethodResponse<int> (IMonitorLogger loggerValue, CodeChunksHandler instance) => {
                loggerValue.Info("389A53AE-EBA5-406A-B762-58E2A14DDDE4", "Pre Chunk3");

                return instance.SyncMethodWithResultForCase9(loggerValue, instance);
            }, (IMonitorLogger loggerValue, CodeChunksHandler instance, int methodResult) => {
                loggerValue.Info("457BEA9F-A4D6-487A-B07F-008BD4C1DB08", "Post Chunk3");
                loggerValue.Info("0DC492E4-121B-4976-A319-3469EB3FC0F9", $"methodResult = {methodResult}");
            });

            codeChunksContext.Run();
        }

        public ISyncMethodResponse SyncMethodForCase9(IMonitorLogger logger, CodeChunksHandler instance)
        {
            logger.Info("44EABFD7-4866-4C97-BC8C-8A6FE4477ACF", "Hi!");

            return LoggedCodeChunkSyncFunctorWithoutResult<CodeChunksHandler, CodeChunksHandlerSyncMethodForCase9LocalContext>.Run(logger, "3545CBB0-F938-4629-BE03-FCA366FD06B7", instance, (ICodeChunksContext<IMonitorLogger, CodeChunksHandler, CodeChunksHandlerSyncMethodForCase9LocalContext> codeChunksContext) =>
            {
                codeChunksContext.CreateCodeChunk("9F930DEE-289C-4596-A9BF-75D93FA236F3", (IMonitorLogger loggerValue, CodeChunksHandler instance, CodeChunksHandlerSyncMethodForCase9LocalContext localContextValue) =>
                {
                    loggerValue.Info("014B1DDD-8AD0-4381-B816-B17ED93478B5", "Chunk1 of Hi!");
                });
            }).ToMethodResponse();
        }

        public ISyncMethodResponse<int> SyncMethodWithResultForCase9(IMonitorLogger logger, CodeChunksHandler instance)
        {
            logger.Info("44EABFD7-4866-4C97-BC8C-8A6FE4477ACF", "Hello!");

            return LoggedCodeChunkSyncFunctorWithResult<CodeChunksHandler, CodeChunksHandlerSyncMethodWithResultForCase9LocalContext, int>.Run(logger, "3A7D152A-6291-4CA3-9F2C-D36675210D57", instance, (ICodeChunksContextWithResult<IMonitorLogger, CodeChunksHandler, CodeChunksHandlerSyncMethodWithResultForCase9LocalContext, int> codeChunksContext) => {
                codeChunksContext.CreateCodeChunk("8A4FACE3-0F2C-40B6-8B06-7A5E4C218D4C", (ICodeChunkWithResultAndSelfReference<IMonitorLogger, CodeChunksHandler, CodeChunksHandlerSyncMethodWithResultForCase9LocalContext, int> currentCodeChunk, IMonitorLogger loggerValue, CodeChunksHandler instanceValue, CodeChunksHandlerSyncMethodWithResultForCase9LocalContext localContextValue) => {
                    loggerValue.Info("014B1DDD-8AD0-4381-B816-B17ED93478B5", "Chunk1 of Hello!");

                    currentCodeChunk.Finish(16);
                });
            }).ToMethodResponse();
        }

        private void Case8()
        {
            using var cancellationTokenSource = new CancellationTokenSource();

            using var threadPool = new CustomThreadPool(0, 20, cancellationTokenSource.Token);

            var commonActiveContext = new ActiveObjectCommonContext();

            var activeContext = new ActiveObjectContext(commonActiveContext, cancellationTokenSource.Token);

            var globalContext = new CodeChunksHandlerGlobalContext();

            var serializationAnchor = new SerializationAnchor();

            var functor = new LoggedCodeChunkFunctorWithoutResult<CodeChunksHandlerGlobalContext, CodeChunksHandlerLocalContext>(_logger, "131A6E93-FB84-4622-AB7F-94E32282A971", globalContext, (ICodeChunksContext<IMonitorLogger, CodeChunksHandlerGlobalContext, CodeChunksHandlerLocalContext> codeChunksContext) =>
            {
                codeChunksContext.CreateCodeChunk("BCD0FE20-836A-4BB3-B6ED-E99CCA7CD058", (IMonitorLogger loggerValue, CodeChunksHandlerGlobalContext globalContextValue, CodeChunksHandlerLocalContext localContextValue) =>
                {
                    loggerValue.Info("5E3BD1EA-C110-4B5D-A70E-607CB02700B3", "Chunk1");
                });

                codeChunksContext.CreateCodeChunk("22AEFD1C-5805-4713-B4AC-00C0CAA2A704", (IMonitorLogger loggerValue, CodeChunksHandlerGlobalContext globalContextValue, CodeChunksHandlerLocalContext localContextValue) =>
                {
                    loggerValue.Info("493E54F2-963D-4030-9018-783688F0F003", "Chunk2");
                });
            }, activeContext, threadPool, serializationAnchor);

            //commonActiveContext.Lock();

            //activeContext.WaitWhenAllIsNotWaited();

            //Thread.Sleep(1000);

            functor.Run();

            //Thread.Sleep(1000);

            //_logger.Info("714FB9D2-4136-4484-8BB3-D75E3D2DE475", "UnLock");

            //commonActiveContext.UnLock();

            Thread.Sleep(10000);
        }

        private void Case7()
        {
            var codeChunksContext = new CodeChunksContext<IMonitorLogger>(_logger);

            codeChunksContext.CreateCodeChunk("611FD4E5-7BEB-400A-9F61-7107B1833CAA", (ICodeChunkWithSelfReference<IMonitorLogger> currentCodeChunk, IMonitorLogger loggerValue) =>
            {
                loggerValue.Info("3B65BA4D-4EAD-4929-98A0-9D17D8D58DE6", "Chunk1");

                foreach (var n in Enumerable.Range(1, 3))
                {
                    loggerValue.Info("B12A2B5A-961A-4D57-BEFD-0968DCA5CC59", $"Chunk1 n = {n}");

                    currentCodeChunk.CreateCodeChunk("60A5B35A-A374-4DB6-B027-36D2787D17DF", (IMonitorLogger loggerValue) =>
                    {
                        loggerValue.Info("86E4062C-1A28-42A8-A7DB-FFC552FAE01B", "Chunk1");
                    });
                }
            });

            codeChunksContext.CreateCodeChunk("14F48D04-4A50-465C-9B07-703BF5EC3FF0", (IMonitorLogger loggerValue) =>
            {
                loggerValue.Info("D0EEDE37-2168-4468-8CFC-DE79917B3539", "Chunk2");
            });

            codeChunksContext.Run();
        }

        private void Case6()
        {
            var codeChunksContext = new CodeChunksContextWithResult<IMonitorLogger, int>(_logger);

            codeChunksContext.CreateCodeChunk("AA297A78-BEBE-4903-B464-FED2C2B23654", (ICodeChunkWithResultAndSelfReference<IMonitorLogger, int> currentCodeChunk, IMonitorLogger loggerValue) =>
            {
                loggerValue.Info("6CC69DCB-4354-49B7-82FC-4714CAC5231F", "Chunk1");
                currentCodeChunk.Finish(16);
            });

            codeChunksContext.CreateCodeChunk("E66722E5-722E-4700-9546-227B1227D640", (ICodeChunkWithResultAndSelfReference<IMonitorLogger, int> currentCodeChunk, IMonitorLogger loggerValue) =>
            {
                loggerValue.Info("EE9CFE13-6380-4D25-B23B-F10A5311D3E8", "Chunk2");
            });

            codeChunksContext.Run();

            var result = codeChunksContext.Result;

            _logger.Info("EB65EFE2-5B6F-46A5-96EA-C018BDD1DA63", $"result = {result}");
        }

        private void Case5()
        {
            var codeChunksContext = new CodeChunksContext<IMonitorLogger>(_logger);

            codeChunksContext.CreateCodeChunk("80972024-5F7C-4BDA-841A-7D7B06EBCA1B", (ICodeChunkWithSelfReference<IMonitorLogger> currentCodeChunk, IMonitorLogger loggerValue) =>
            {
                loggerValue.Info("C9093211-E482-4261-BB94-60386BB20411", "Chunk1");
                currentCodeChunk.Finish();
            });

            codeChunksContext.CreateCodeChunk("33EEDEDE-B350-4514-AC5E-EF0AB0C47D40", (ICodeChunkWithSelfReference<IMonitorLogger> currentCodeChunk, IMonitorLogger loggerValue) =>
            {
                loggerValue.Info("D332AE06-A16F-45D9-B56F-7C48FB18FA53", "Chunk2");
            });

            codeChunksContext.Run();
        }

        private void Case4()
        {
            //using var cancellationTokenSource = new CancellationTokenSource();

            //using var threadPool = new CustomThreadPool(0, 20, cancellationTokenSource.Token);

            //var commonActiveContext = new ActiveObjectCommonContext();

            //var activeContext = new ActiveObjectContext(commonActiveContext, cancellationTokenSource.Token);

            //var localContext = new Case3LocalContext();

            //var codeChunksContext = new CodeChunksContext();

            //LoggedFunctorWithoutResult<CodeChunksContext, Case3LocalContext>.Run(_logger, codeChunksContext, localContext, (loggerValue, codeChunksContextValue, localContextValue) =>
            //{
            //    codeChunksContextValue.CreateCodeChunk("93002742-6593-421A-93E3-1590B97760C3", (ICodeChunkWithSelfReference currentCodeChunk) =>
            //    {
            //        loggerValue.Info("3ACE2DAA-4D07-4F5C-A0E9-F010E8DDF5BA", "Chunk1");

            //        loggerValue.Info("F08358E1-B020-437C-92FE-20D11321A1F8", $"localContext.Property1 = {localContext.Property1}");

            //        localContextValue.Property1 = 16;
            //    });

            //    codeChunksContext.CreateCodeChunk("63B032D6-BF39-42D9-B336-39DFD056A9FC", (ICodeChunkWithSelfReference currentCodeChunk) =>
            //    {
            //        loggerValue.Info("CC61FCC7-F8F5-48CB-898F-3F4697E984E3", "Chunk2");

            //        loggerValue.Info("307973EF-F423-4E3C-821B-27C7119FD04D", $"localContext.Property1 = {localContext.Property1}");
            //    });

            //    codeChunksContextValue.Run();
            //}, activeContext, threadPool);

            //Thread.Sleep(10000);
        }

        private void Case3()
        {
            var localContext = new CodeChunksHandlerCase3LocalContext();

            var codeChunksContext = new CodeChunksContext<IMonitorLogger, CodeChunksHandlerCase3LocalContext>(_logger, localContext);

            codeChunksContext.CreateCodeChunk("FEF1781D-D63A-4374-9977-F82B83EF90CC", (ICodeChunkWithSelfReference<IMonitorLogger, CodeChunksHandlerCase3LocalContext> currentCodeChunk, IMonitorLogger loggerValue, CodeChunksHandlerCase3LocalContext localContextValue) =>
            {
                loggerValue.Info("3ACE2DAA-4D07-4F5C-A0E9-F010E8DDF5BA", "Chunk1");

                loggerValue.Info("F08358E1-B020-437C-92FE-20D11321A1F8", $"localContextValue.Property1 = {localContextValue.Property1}");

                localContextValue.Property1 = 16;
            });

            codeChunksContext.CreateCodeChunk("9233793C-5BEE-4B1F-A4BD-37A5FC077638", (ICodeChunkWithSelfReference<IMonitorLogger, CodeChunksHandlerCase3LocalContext> currentCodeChunk, IMonitorLogger loggerValue, CodeChunksHandlerCase3LocalContext localContextValue) =>
            {
                loggerValue.Info("CC61FCC7-F8F5-48CB-898F-3F4697E984E3", "Chunk2");

                loggerValue.Info("307973EF-F423-4E3C-821B-27C7119FD04D", $"localContextValue.Property1 = {localContextValue.Property1}");
            });

            codeChunksContext.Run();
        }

        private void Case2()
        {
            var codeChunksContext = new CodeChunksContext<IMonitorLogger>(_logger);

            codeChunksContext.CreateCodeChunk("4A578C7D-3E87-4024-B58E-108E48889241", (ICodeChunkWithSelfReference<IMonitorLogger> currentCodeChunk, IMonitorLogger loggerValue) =>
            {
                loggerValue.Info("3ACE2DAA-4D07-4F5C-A0E9-F010E8DDF5BA", "Chunk1");
            });

            codeChunksContext.CreateCodeChunk("113A25E0-9217-4B67-A918-CC10200F2EDF", (ICodeChunkWithSelfReference<IMonitorLogger> currentCodeChunk, IMonitorLogger loggerValue) =>
            {
                loggerValue.Info("CC61FCC7-F8F5-48CB-898F-3F4697E984E3", "Chunk2");
            });

            codeChunksContext.Run();
        }

        private void Case1()
        {
            var codeChunksFactory = new CodeChunksContext<IMonitorLogger>(_logger);

            var codeChanksList = new List<CodeChunkWithSelfReference<IMonitorLogger>>()
            {
                new CodeChunkWithSelfReference<IMonitorLogger>("7D212BD2-40F4-4890-B990-BF917F17E91B", codeChunksFactory, _logger, (ICodeChunkWithSelfReference<IMonitorLogger> currentCodeChunk, IMonitorLogger loggerValue) => {
                    loggerValue.Info("31A6BA75-4BE2-4DE9-82AB-C4446EAD9F69", "Chunk1");
                }),
                new CodeChunkWithSelfReference<IMonitorLogger>("843943DB-A201-45C4-AFD7-149A32975B26", codeChunksFactory, _logger, (ICodeChunkWithSelfReference<IMonitorLogger> currentCodeChunk, IMonitorLogger loggerValue) => {
                    loggerValue.Info("28A8BD3A-C108-4AF3-81E9-2443A778F200", "Chunk2");
                })
            };

            foreach (var codeChunk in codeChanksList)
            {
                codeChunk.Run();
            }
        }
    }
}
