//using SymOntoClay.ActiveObject.Functors;
//using SymOntoClay.ActiveObject.MethodResponses;
//using SymOntoClay.ActiveObject.Threads;
//using SymOntoClay.Common;
//using SymOntoClay.Common.DebugHelpers;
//using SymOntoClay.Core.Internal.CodeModel;
//using SymOntoClay.Monitor.Common;
//using SymOntoClay.Threading;
//using System;
//using System.Text;

//namespace TestSandbox.Threads
//{
//    public class TstInstance
//    {
//        private class InternalData: IObjectToString
//        {
//            public int Property1 { get; set; }

//            /// <inheritdoc/>
//            public override string ToString()
//            {
//                return ToString(0u);
//            }

//            /// <inheritdoc/>
//            public string ToString(uint n)
//            {
//                return this.GetDefaultToStringInformation(n);
//            }

//            /// <inheritdoc/>
//            string IObjectToString.PropertiesToString(uint n)
//            {
//                var spaces = DisplayHelper.Spaces(n);
//                var sb = new StringBuilder();
//                sb.AppendLine($"{spaces}{nameof(Property1)} = {Property1}");
//                return sb.ToString();
//            }
//        }

//        public TstInstance(IMonitorLogger logger, IActiveObjectContext activeObjectContext, ICustomThreadPool threadPool)
//        {
//            _logger = logger;
//            _activeObjectContext = activeObjectContext;
//            _threadPool = threadPool;
//            _data = new InternalData();
//        }

//        private readonly IMonitorLogger _logger;
//        private readonly IActiveObjectContext _activeObjectContext;
//        private readonly ICustomThreadPool _threadPool;
//        private readonly InternalData _data;

//        private class Method1LocalContext: IObjectToString
//        {
//            public int Field1;

//            /// <inheritdoc/>
//            public override string ToString()
//            {
//                return ToString(0u);
//            }

//            /// <inheritdoc/>
//            public string ToString(uint n)
//            {
//                return this.GetDefaultToStringInformation(n);
//            }

//            /// <inheritdoc/>
//            string IObjectToString.PropertiesToString(uint n)
//            {
//                var spaces = DisplayHelper.Spaces(n);
//                var sb = new StringBuilder();
//                sb.AppendLine($"{spaces}{nameof(Field1)} = {Field1}");
//                return sb.ToString();
//            }
//        }

//        public IMethodResponse Method1()
//        {
//            return LoggedCodeChunkFunctorWithoutResult<InternalData, Method1LocalContext>.Run(_logger, "9090DD1A-81C0-4789-889A-9D7A24D8DDD0", _data,
//                (codeChunksContext) => {
//                    codeChunksContext.CreateCodeChunk("240D8A34-918B-4949-AFAF-7A72F5F8AF93", (IMonitorLogger loggerValue, InternalData globalContextValue, Method1LocalContext localContextValue) =>
//                    {
//                        loggerValue.Info("4CFFAD56-D6DE-4913-BF5E-D65E4D56131E", $"globalContextValue = {globalContextValue}");
//                        loggerValue.Info("1E9F8BE0-A13A-4F9B-91E6-BC481F761C0A", $"localContextValue = {localContextValue}");

//                        loggerValue.Info("33BED06D-A69C-4945-85F7-0BB17FAB8EC4", "Chunk1");

//                        globalContextValue.Property1 = 16;
//                        localContextValue.Field1 = 42;
//                    });

//                    codeChunksContext.CreateCodeChunk("F1B20977-05AB-400A-94FD-CFBE5B84B5DF", (ICodeChunk currentCodeChunk, IMonitorLogger loggerValue, InternalData globalContextValue, Method1LocalContext localContextValue) =>
//                    {
//                        _logger.Info("819B488D-8752-4337-A4F1-621D8471EF6B", "Chunk2");

//                        loggerValue.Info("862A1027-5671-432D-9EDA-72AB46D623C1", $"globalContextValue = {globalContextValue}");
//                        loggerValue.Info("41604C21-35A1-4FD2-9DE2-C85774D2A3D8", $"localContextValue = {localContextValue}");

//                        SyncMethodReponseResolver.Run(loggerValue, "413EB4F7-56FD-44C2-813B-8A35F1C65DCA", currentCodeChunk, () => {
//                            return PrivateMethod1(globalContextValue, loggerValue, currentCodeChunk);
//                        });
//                    });
//                }, _activeObjectContext, _threadPool).ToMethodResponse();
//        }

//        private class PrivateMethod1LocalContext: IObjectToString
//        {
//            public int Filed2;

//            /// <inheritdoc/>
//            public override string ToString()
//            {
//                return ToString(0u);
//            }

//            /// <inheritdoc/>
//            public string ToString(uint n)
//            {
//                return this.GetDefaultToStringInformation(n);
//            }

//            /// <inheritdoc/>
//            string IObjectToString.PropertiesToString(uint n)
//            {
//                var spaces = DisplayHelper.Spaces(n);
//                var sb = new StringBuilder();
//                sb.AppendLine($"{spaces}{nameof(Filed2)} = {Filed2}");
//                return sb.ToString();
//            }
//        }

//        private static IMethodResponse PrivateMethod1(InternalData data, IMonitorLogger logger, ICodeChunk currentCodeChunk)
//        {
//            logger.Info("9E315FF4-69F0-463B-8C4D-B14A80F23E29", "Begin");

//            return LoggedSyncCodeChunkFunctorWithoutResult<PrivateMethod1LocalContext>.Run(logger, "03AE2121-F27A-4E8C-B10A-9B6A62E676CE", currentCodeChunk, (codeChunksContext, localContextValue) => {
//                codeChunksContext.CreateCodeChunk("98547968-0A9C-432F-AD3E-297915746616", () =>
//                {
//                    logger.Info("47890337-ABE5-4DC4-9079-0C8EF31C0926", $"data = {data}");
//                    logger.Info("6F528CFA-1596-4DBA-ADC7-25BBC00C0A06", $"localContextValue = {localContextValue}");

//                    logger.Info("512957CB-8402-47DC-81B4-76DDAF523292", "private Chunk1");

//                    data.Property1 = 88;
//                    localContextValue.Filed2 = 77;
//                });

//                codeChunksContext.CreateCodeChunk("2BB05A1E-6216-4446-BBF4-F1A7A3AA1985", () =>
//                {
//                    logger.Info("EAA9CC46-9E23-4A28-9B55-1267ED195023", $"data = {data}");
//                    logger.Info("E2E61680-516E-4C22-807F-A9B77A260E91", $"localContextValue = {localContextValue}");

//                    logger.Info("96BBE8F3-BE06-466B-9D92-9DB721F064DA", "private Chunk2");
//                });
//            }).ToMethodResponse();
//        }

//        private class Method2LocalContext : IObjectToString
//        {
//            public float Field1;

//            /// <inheritdoc/>
//            public override string ToString()
//            {
//                return ToString(0u);
//            }

//            /// <inheritdoc/>
//            public string ToString(uint n)
//            {
//                return this.GetDefaultToStringInformation(n);
//            }

//            /// <inheritdoc/>
//            string IObjectToString.PropertiesToString(uint n)
//            {
//                var spaces = DisplayHelper.Spaces(n);
//                var sb = new StringBuilder();
//                sb.AppendLine($"{spaces}{nameof(Field1)} = {Field1}");
//                return sb.ToString();
//            }
//        }

//        public IMethodResponse<string> Method2(int someParam)
//        {
//            _logger.Info("85784889-695B-420B-8B25-AD7ACCADFD10", $"someParam = {someParam}");

//            return LoggedCodeChunkFunctorWithResult<InternalData, Method2LocalContext, int, string>.Run(_logger, "0C61B33E-E20A-44EE-8F71-F4D2C8508BCB", _data, someParam,
//                (loggerValue, codeChunksContext, globalContextValue, localContextValue, someParamValue) => {
//                    codeChunksContext.CreateCodeChunk("7869834A-F3F3-4A59-9BA5-BA3832451018", () =>
//                    {
//                        loggerValue.Info("B3256A59-5CA2-463F-B153-9B19F9E67869", $"globalContextValue = {globalContextValue}");
//                        loggerValue.Info("ED753CEC-4E19-40BD-88C0-B0AC81EC19A6", $"localContextValue = {localContextValue}");

//                        loggerValue.Info("37F3A8F8-234F-4873-AA70-A764ADB01462", "Chunk1");

//                        globalContextValue.Property1 = 16;
//                        //localContextValue.Field1 = 42;
//                    });
//                    codeChunksContext.CreateCodeChunk("B4747F71-6675-469E-ADF4-4A4DF38B1F31", (currentCodeChunk) =>
//                    {
//                        _logger.Info("8BA94558-A3CD-4929-82D7-D245EBD75F7F", "Chunk2");

//                        loggerValue.Info("0A4A03A9-28E5-4031-95D8-CA601D9788C7", $"globalContextValue = {globalContextValue}");
//                        loggerValue.Info("027374B6-5109-4EC6-A7F4-327757144FEA", $"localContextValue = {localContextValue}");

//                        localContextValue.Field1 = SyncMethodReponseResolver<float>.Run(loggerValue, "E6ED7085-BAD7-4815-9297-C151291F08D2", currentCodeChunk, () =>
//                        {
//                            return PrivateMethod2(globalContextValue, loggerValue, currentCodeChunk, someParamValue);
//                        });

//                        loggerValue.Info("3CC1D3AD-9C02-4385-B7C8-C23FE5E2537A", $"localContextValue = {localContextValue}");

//                        codeChunksContext.Finish("Hi!");
//                    });
//                }, _activeObjectContext, _threadPool).ToMethodResponse();
//        }

//        private class PrivateMethod2LocalContext : IObjectToString
//        {
//            /// <inheritdoc/>
//            public override string ToString()
//            {
//                return ToString(0u);
//            }

//            /// <inheritdoc/>
//            public string ToString(uint n)
//            {
//                return this.GetDefaultToStringInformation(n);
//            }

//            /// <inheritdoc/>
//            string IObjectToString.PropertiesToString(uint n)
//            {
//                var spaces = DisplayHelper.Spaces(n);
//                var sb = new StringBuilder();
//                //sb.AppendLine($"{spaces}{nameof()} = {}");
//                return sb.ToString();
//            }
//        }

//        private static IMethodResponse<float> PrivateMethod2(InternalData data, IMonitorLogger logger, ICodeChunk currentCodeChunk, int otherParam)
//        {
//            logger.Info("1FF1EB5B-D495-424E-8A6A-AE723D260695", $"otherParam = {otherParam}");

//            return LoggedSyncCodeChunkFunctorWithResult<PrivateMethod2LocalContext, int, float>.Run(logger, "03AE2121-F27A-4E8C-B10A-9B6A62E676CE", currentCodeChunk, otherParam,
//            (codeChunksContext, localContextValue, otherParamValue) => {
//                codeChunksContext.CreateCodeChunk("DE6B1FB4-0588-4C8B-9728-AB188D545A4F", () =>
//                {
//                    logger.Info("A6FEBED7-A316-4F7D-957B-E272327500FF", $"data = {data}");
//                    logger.Info("921F5B0F-8FC0-4087-A358-1598C19F5F8C", $"localContextValue = {localContextValue}");

//                    logger.Info("7BB8C204-38CD-4F2D-A939-455DC03CFFB4", "private Chunk1");

//                    data.Property1 = 88;
//                    //localContextValue.Filed2 = 77;
//                });

//                codeChunksContext.CreateCodeChunk("FCAB8D7E-6BD5-419D-ABF1-1E6DD6F48490", () =>
//                {
//                    logger.Info("6070466B-ABC5-4338-AC6C-8064A3118EF8", $"data = {data}");
//                    logger.Info("33AA15E4-10E1-4DDA-8EE5-93CE3016ABCE", $"localContextValue = {localContextValue}");

//                    logger.Info("2B86D839-E7B9-46F3-9BDC-BE1432E1770E", "private Chunk2");

//                    codeChunksContext.Finish(3.18f);
//                });
//            }).ToMethodResponse();
//        }
//    }
//}
