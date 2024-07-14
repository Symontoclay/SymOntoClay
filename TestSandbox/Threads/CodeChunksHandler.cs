using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Monitor.NLog;
using SymOntoClay.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestSandbox.Threads
{
    public class CodeChunksHandler
    {
        private static readonly IMonitorLogger _logger = new MonitorLoggerNLogImpementation();

        public void Run()
        {
            _logger.Info("E7E3E621-38BD-4B54-B041-FD0C74FE5BBF", "Begin");

            Case4();
            //Case3();
            //Case2();
            //Case1();

            _logger.Info("B7E2F374-D3BC-48B5-B0F2-357590821EAE", "End");
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
