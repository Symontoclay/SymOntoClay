using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;

namespace SymOntoClay.Core.Internal.Converters
{
    public class TypeConverter: BaseContextComponent, ITypeConverter
    {
        public TypeConverter(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IMainStorageContext _context;

        /// <inheritdoc/>
        public CallResult CheckAndTryConvert(IMonitorLogger logger, Value value, IReadOnlyCollection<StrongIdentifierValue> typesList, ILocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            Info("50526CB3-643C-4602-8B17-58CB31D522CA", $"value = {value}");
            Info("95E681A8-1612-4252-B477-F70B51097823", $"typesList = {typesList.WriteListToString()}");
#endif

            throw new NotImplementedException("435AEB0F-4EB7-4219-89D3-D63871296755");
        }

        public CallResult 
    }
}
