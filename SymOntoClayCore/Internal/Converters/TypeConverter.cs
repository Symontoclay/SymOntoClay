using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

        private InheritanceResolver _inheritanceResolver;
        private StrongIdentifierValue _anyTypeName;

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            _inheritanceResolver = _context.DataResolversFactory.GetInheritanceResolver();

            _anyTypeName = _context.CommonNamesStorage.AnyTypeName;
        }

        /// <inheritdoc/>
        public CallResult CheckAndTryConvert(IMonitorLogger logger, Value value, IList<StrongIdentifierValue> typesList, ILocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            Info("50526CB3-643C-4602-8B17-58CB31D522CA", $"value = {value}");
            Info("95E681A8-1612-4252-B477-F70B51097823", $"typesList = {typesList.WriteListToString()}");
#endif

            var checkResult = CheckFitValue(logger, value, typesList, localCodeExecutionContext);

#if DEBUG
            Info("AB5669E1-4B92-45A8-A562-5162241198CD", $"checkResult = {checkResult}");
#endif

            throw new NotImplementedException("435AEB0F-4EB7-4219-89D3-D63871296755");
        }

        /// <inheritdoc/>
        public TypeFitCheckingResult CheckFitValue(IMonitorLogger logger, Value value, IList<StrongIdentifierValue> typesList, ILocalCodeExecutionContext localCodeExecutionContext)
        {
#if DEBUG
            Info("F003F8DC-8153-42A6-86D5-9F544A627690", $"value = {value}");
            Info("569F4890-E159-4A87-BDCE-03F5D9F039FB", $"typesList = {typesList.WriteListToString()}");
#endif

            if (value.IsNullValue)
            {
                return TypeFitCheckingResult.Fit;
            }

            if (typesList.IsNullOrEmpty())
            {
                return TypeFitCheckingResult.Fit;
            }

            if (typesList.Any(p => p == _anyTypeName))
            {
                return TypeFitCheckingResult.Fit;
            }

            var isFit = _inheritanceResolver.IsFit(logger, typesList, value, localCodeExecutionContext);

            if (isFit)
            {
                return TypeFitCheckingResult.Fit;
            }

            //throw new Exception($"The value '{value.ToHumanizedString()}' does not fit to variable {CodeItem.ToHumanizedString()}");

            throw new NotImplementedException("11924B32-F3D5-4CC4-93FA-D14C239F27C5");
        }
    }
}
