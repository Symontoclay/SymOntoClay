using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using SymOntoClay.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TestSandbox.Threads
{
    public class CodeChunksHandler
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        public void Run()
        {
            _logger.Info("E7E3E621-38BD-4B54-B041-FD0C74FE5BBF", "Begin");

            Case10();
            //Case9();
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

        private void Case10()
        {
            using var cancellationTokenSource = new CancellationTokenSource();

            using var threadPool = new CustomThreadPool(0, 20, cancellationTokenSource.Token);

            var commonActiveContext = new ActiveObjectCommonContext();

            var activeContext = new ActiveObjectContext(commonActiveContext, cancellationTokenSource.Token);

            var instance = new TstInstance(_logger, activeContext, threadPool);

            var methodResult = instance.Method2(11);

            var result = methodResult.Result;

            _logger.Info("A560B79C-93FD-42C3-9B75-1AFBE2679486", $"result = {result}");

            Thread.Sleep(10000);
        }

        private void Case9()
        {
            using var cancellationTokenSource = new CancellationTokenSource();

            using var threadPool = new CustomThreadPool(0, 20, cancellationTokenSource.Token);

            var commonActiveContext = new ActiveObjectCommonContext();

            var activeContext = new ActiveObjectContext(commonActiveContext, cancellationTokenSource.Token);

            var instance = new TstInstance(_logger, activeContext, threadPool);

            var result = instance.Method1();
            result.Wait();

            Thread.Sleep(10000);
        }

        private class GlobalContext
        {
            public string GlobalStr { get; set; }
        }

        private class LocalContext
        {
            public int Property1 { get; set; }
        }

        private void Case8()
        {
            using var cancellationTokenSource = new CancellationTokenSource();

            using var threadPool = new CustomThreadPool(0, 20, cancellationTokenSource.Token);

            var commonActiveContext = new ActiveObjectCommonContext();

            var activeContext = new ActiveObjectContext(commonActiveContext, cancellationTokenSource.Token);

            var globalContext = new GlobalContext();

            var functor = new LoggedCodeChunkFunctorWithoutResult<GlobalContext, LocalContext>(_logger, "131A6E93-FB84-4622-AB7F-94E32282A971", globalContext, (loggerValue, codeChunksContext, globalContextValue, localContextValue) => {
                codeChunksContext.CreateCodeChunk("BCD0FE20-836A-4BB3-B6ED-E99CCA7CD058", () =>
                {
                    loggerValue.Info("5E3BD1EA-C110-4B5D-A70E-607CB02700B3", "Chunk1");
                });

                codeChunksContext.CreateCodeChunk("22AEFD1C-5805-4713-B4AC-00C0CAA2A704", () =>
                {
                    loggerValue.Info("493E54F2-963D-4030-9018-783688F0F003", "Chunk2");
                });
            }, activeContext, threadPool);

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
            var codeChunksContext = new CodeChunksContext("6A148928-3945-42D9-8DB7-60531401BA37");

            codeChunksContext.CreateCodeChunk("611FD4E5-7BEB-400A-9F61-7107B1833CAA", (currentCodeChunk) =>
            {
                _logger.Info("3B65BA4D-4EAD-4929-98A0-9D17D8D58DE6", "Chunk1");

                foreach(var n in Enumerable.Range(1, 3))
                {
                    _logger.Info("B12A2B5A-961A-4D57-BEFD-0968DCA5CC59", $"Chunk1 n = {n}");

                    codeChunksContext.CreateCodeChunk("60A5B35A-A374-4DB6-B027-36D2787D17DF" + n.ToString(), currentCodeChunk, () => {
                        _logger.Info("86E4062C-1A28-42A8-A7DB-FFC552FAE01B", "Chunk1 " + n);
                    });
                }
            });

            codeChunksContext.CreateCodeChunk("14F48D04-4A50-465C-9B07-703BF5EC3FF0", () =>
            {
                _logger.Info("D0EEDE37-2168-4468-8CFC-DE79917B3539", "Chunk2");
            });

            codeChunksContext.Run();
        }

        private void Case6()
        {
            var codeChunksContext = new CodeChunksContextWithResult<int>("402F4094-6F7D-431F-8DFD-D7D67D303CC0");

            codeChunksContext.CreateCodeChunk("AA297A78-BEBE-4903-B464-FED2C2B23654", (currentCodeChunk) =>
            {
                _logger.Info("6CC69DCB-4354-49B7-82FC-4714CAC5231F", "Chunk1");
                codeChunksContext.Finish(16);
            });

            codeChunksContext.CreateCodeChunk("E66722E5-722E-4700-9546-227B1227D640", (currentCodeChunk) =>
            {
                _logger.Info("EE9CFE13-6380-4D25-B23B-F10A5311D3E8", "Chunk2");
            });

            codeChunksContext.Run();

            var result = codeChunksContext.Result;

            _logger.Info("EB65EFE2-5B6F-46A5-96EA-C018BDD1DA63", $"result = {result}");
        }

        private void Case5()
        {
            var codeChunksContext = new CodeChunksContext("E63D84C0-9771-44CC-B762-9E4DE6A299A8");

            codeChunksContext.CreateCodeChunk("80972024-5F7C-4BDA-841A-7D7B06EBCA1B", (currentCodeChunk) =>
            {
                _logger.Info("C9093211-E482-4261-BB94-60386BB20411", "Chunk1");
                codeChunksContext.Finish();
            });

            codeChunksContext.CreateCodeChunk("33EEDEDE-B350-4514-AC5E-EF0AB0C47D40", (currentCodeChunk) =>
            {
                _logger.Info("D332AE06-A16F-45D9-B56F-7C48FB18FA53", "Chunk2");
            });

            codeChunksContext.Run();
        }

        private void Case4()
        {
            using var cancellationTokenSource = new CancellationTokenSource();

            using var threadPool = new CustomThreadPool(0, 20, cancellationTokenSource.Token);

            var commonActiveContext = new ActiveObjectCommonContext();

            var activeContext = new ActiveObjectContext(commonActiveContext, cancellationTokenSource.Token);

            var localContext = new Case3LocalContext();

            var codeChunksContext = new CodeChunksContext("71841990-72E7-4E09-ADEB-F72AA9ED4D5E");

            LoggedFunctorWithoutResult<CodeChunksContext, Case3LocalContext>.Run(_logger, codeChunksContext, localContext, (loggerValue, codeChunksContextValue, localContextValue) => {
                codeChunksContextValue.CreateCodeChunk("93002742-6593-421A-93E3-1590B97760C3", (currentCodeChunk) =>
                {
                    loggerValue.Info("3ACE2DAA-4D07-4F5C-A0E9-F010E8DDF5BA", "Chunk1");

                    loggerValue.Info("F08358E1-B020-437C-92FE-20D11321A1F8", $"localContext.Property1 = {localContext.Property1}");

                    localContextValue.Property1 = 16;
                });

                codeChunksContext.CreateCodeChunk("63B032D6-BF39-42D9-B336-39DFD056A9FC", (currentCodeChunk) =>
                {
                    loggerValue.Info("CC61FCC7-F8F5-48CB-898F-3F4697E984E3", "Chunk2");

                    loggerValue.Info("307973EF-F423-4E3C-821B-27C7119FD04D", $"localContext.Property1 = {localContext.Property1}");
                });

                codeChunksContextValue.Run();
            }, activeContext, threadPool);

            Thread.Sleep(10000);
        }

        private class Case3LocalContext()
        {
            public int Property1 { get; set; }
        }

        private void Case3()
        {
            var localContext = new Case3LocalContext();

            var codeChunksContext = new CodeChunksContext("6B0EDECA-209F-4D67-BD49-3CCD479C0D4F");

            codeChunksContext.CreateCodeChunk("FEF1781D-D63A-4374-9977-F82B83EF90CC", (currentCodeChunk) =>
            {
                _logger.Info("3ACE2DAA-4D07-4F5C-A0E9-F010E8DDF5BA", "Chunk1");

                _logger.Info("F08358E1-B020-437C-92FE-20D11321A1F8", $"localContext.Property1 = {localContext.Property1}");

                localContext.Property1 = 16;
            });

            codeChunksContext.CreateCodeChunk("9233793C-5BEE-4B1F-A4BD-37A5FC077638", (currentCodeChunk) =>
            {
                _logger.Info("CC61FCC7-F8F5-48CB-898F-3F4697E984E3", "Chunk2");

                _logger.Info("307973EF-F423-4E3C-821B-27C7119FD04D", $"localContext.Property1 = {localContext.Property1}");
            });

            codeChunksContext.Run();
        }

        private void Case2()
        {
            var codeChunksContext = new CodeChunksContext("5DAA2D89-5C04-489B-92AF-7D2794117DEE");

            codeChunksContext.CreateCodeChunk("4A578C7D-3E87-4024-B58E-108E48889241", (currentCodeChunk) =>
            {
                _logger.Info("3ACE2DAA-4D07-4F5C-A0E9-F010E8DDF5BA", "Chunk1");
            });

            codeChunksContext.CreateCodeChunk("113A25E0-9217-4B67-A918-CC10200F2EDF", (currentCodeChunk) =>
            {
                _logger.Info("CC61FCC7-F8F5-48CB-898F-3F4697E984E3", "Chunk2");
            });

            codeChunksContext.Run();
        }

        private void Case1()
        {
            var codeChunksFactory = new CodeChunksContext("D84F51CD-B405-4F5E-90AE-55B8C08A8456");

            var codeChanksList = new List<CodeChunkWithSelfReference>()
            {
                new CodeChunkWithSelfReference(codeChunksFactory, "7D212BD2-40F4-4890-B990-BF917F17E91B", (currentCodeChunk) => {
                    _logger.Info("31A6BA75-4BE2-4DE9-82AB-C4446EAD9F69", "Chunk1");
                }),
                new CodeChunkWithSelfReference(codeChunksFactory, "843943DB-A201-45C4-AFD7-149A32975B26", (currentCodeChunk) => {
                    _logger.Info("28A8BD3A-C108-4AF3-81E9-2443A778F200", "Chunk2");
                })
            };

            foreach (var codeChunk in codeChanksList)
            {
                codeChunk.Run();
            }
        }
    }
}
