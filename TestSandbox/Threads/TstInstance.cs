﻿using SymOntoClay.ActiveObject.Functors;
using SymOntoClay.ActiveObject.MethodResponses;
using SymOntoClay.ActiveObject.Threads;
using SymOntoClay.Common;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using SymOntoClay.Threading;
using System;
using System.Text;

namespace TestSandbox.Threads
{
    public class TstInstance
    {
        private class InternalData: IObjectToString
        {
            public int Property1 { get; set; }

            /// <inheritdoc/>
            public override string ToString()
            {
                return ToString(0u);
            }

            /// <inheritdoc/>
            public string ToString(uint n)
            {
                return this.GetDefaultToStringInformation(n);
            }

            /// <inheritdoc/>
            string IObjectToString.PropertiesToString(uint n)
            {
                var spaces = DisplayHelper.Spaces(n);
                var sb = new StringBuilder();
                sb.AppendLine($"{spaces}{nameof(Property1)} = {Property1}");
                return sb.ToString();
            }
        }

        public TstInstance(IMonitorLogger logger, IActiveObjectContext activeObjectContext, ICustomThreadPool threadPool)
        {
            _logger = logger;
            _activeObjectContext = activeObjectContext;
            _threadPool = threadPool;
            _data = new InternalData();
        }

        private readonly IMonitorLogger _logger;
        private readonly IActiveObjectContext _activeObjectContext;
        private readonly ICustomThreadPool _threadPool;
        private readonly InternalData _data;

        private class Method1LocalContext: IObjectToString
        {
            public int Field1;

            /// <inheritdoc/>
            public override string ToString()
            {
                return ToString(0u);
            }

            /// <inheritdoc/>
            public string ToString(uint n)
            {
                return this.GetDefaultToStringInformation(n);
            }

            /// <inheritdoc/>
            string IObjectToString.PropertiesToString(uint n)
            {
                var spaces = DisplayHelper.Spaces(n);
                var sb = new StringBuilder();
                sb.AppendLine($"{spaces}{nameof(Field1)} = {Field1}");
                return sb.ToString();
            }
        }

        public IMethodResponse Method1()
        {
            return LoggedCodeChunkFunctorWithoutResult<InternalData, Method1LocalContext>.Run(_logger, "9090DD1A-81C0-4789-889A-9D7A24D8DDD0", _data,
                (loggerValue, codeChunksContext, globalContextValue, localContextValue) => {
                    codeChunksContext.CreateCodeChunk("240D8A34-918B-4949-AFAF-7A72F5F8AF93", () =>
                    {
                        loggerValue.Info("4CFFAD56-D6DE-4913-BF5E-D65E4D56131E", $"globalContextValue = {globalContextValue}");
                        loggerValue.Info("1E9F8BE0-A13A-4F9B-91E6-BC481F761C0A", $"localContextValue = {localContextValue}");

                        loggerValue.Info("33BED06D-A69C-4945-85F7-0BB17FAB8EC4", "Chunk1");

                        globalContextValue.Property1 = 16;
                        localContextValue.Field1 = 42;
                    });

                    codeChunksContext.CreateCodeChunk("F1B20977-05AB-400A-94FD-CFBE5B84B5DF", (currentCodeChunk) =>
                    {
                        _logger.Info("819B488D-8752-4337-A4F1-621D8471EF6B", "Chunk2");

                        loggerValue.Info("862A1027-5671-432D-9EDA-72AB46D623C1", $"globalContextValue = {globalContextValue}");
                        loggerValue.Info("41604C21-35A1-4FD2-9DE2-C85774D2A3D8", $"localContextValue = {localContextValue}");

                        SyncMethodReponseResolver.Run(loggerValue, "413EB4F7-56FD-44C2-813B-8A35F1C65DCA", currentCodeChunk, () => {
                            return PrivateMethod1(globalContextValue, loggerValue, currentCodeChunk);
                        });
                    });
                }, _activeObjectContext, _threadPool).ToMethodResponse();
        }

        private class PrivateMethod1LocalContext: IObjectToString
        {
            public int Filed2;

            /// <inheritdoc/>
            public override string ToString()
            {
                return ToString(0u);
            }

            /// <inheritdoc/>
            public string ToString(uint n)
            {
                return this.GetDefaultToStringInformation(n);
            }

            /// <inheritdoc/>
            string IObjectToString.PropertiesToString(uint n)
            {
                var spaces = DisplayHelper.Spaces(n);
                var sb = new StringBuilder();
                sb.AppendLine($"{spaces}{nameof(Filed2)} = {Filed2}");
                return sb.ToString();
            }
        }

        private static IMethodResponse PrivateMethod1(InternalData data, IMonitorLogger logger, ICodeChunk currentCodeChunk)
        {
            logger.Info("9E315FF4-69F0-463B-8C4D-B14A80F23E29", "Begin");

            return LoggedSyncCodeChunkFunctorWithoutResult<PrivateMethod1LocalContext>.Run(logger, "03AE2121-F27A-4E8C-B10A-9B6A62E676CE", currentCodeChunk, (codeChunksContext, localContextValue) => {
                codeChunksContext.CreateCodeChunk("98547968-0A9C-432F-AD3E-297915746616", () =>
                {
                    logger.Info("47890337-ABE5-4DC4-9079-0C8EF31C0926", $"data = {data}");
                    logger.Info("6F528CFA-1596-4DBA-ADC7-25BBC00C0A06", $"localContextValue = {localContextValue}");

                    logger.Info("512957CB-8402-47DC-81B4-76DDAF523292", "private Chunk1");

                    data.Property1 = 88;
                    localContextValue.Filed2 = 77;
                });

                codeChunksContext.CreateCodeChunk("2BB05A1E-6216-4446-BBF4-F1A7A3AA1985", () =>
                {
                    logger.Info("EAA9CC46-9E23-4A28-9B55-1267ED195023", $"data = {data}");
                    logger.Info("E2E61680-516E-4C22-807F-A9B77A260E91", $"localContextValue = {localContextValue}");

                    logger.Info("96BBE8F3-BE06-466B-9D92-9DB721F064DA", "private Chunk2");
                });
            }).ToMethodResponse();
        }
    }
}
