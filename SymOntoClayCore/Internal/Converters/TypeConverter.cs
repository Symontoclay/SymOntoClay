using SymOntoClay.Common.CollectionsHelpers;
using SymOntoClay.Common.DebugHelpers;
using SymOntoClay.Core.DebugHelpers;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.DataResolvers;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        private readonly ResolverOptions _defaultOptions = ResolverOptions.GetDefaultOptions();

        /// <inheritdoc/>
        protected override void LinkWithOtherBaseContextComponents()
        {
            base.LinkWithOtherBaseContextComponents();

            _inheritanceResolver = _context.DataResolversFactory.GetInheritanceResolver();

            _anyTypeName = _context.CommonNamesStorage.AnyTypeName;
        }

        /// <inheritdoc/>
        public ResolverOptions DefaultOptions => _defaultOptions;

        /// <inheritdoc/>
        public int GetCapacity(IMonitorLogger logger, IList<StrongIdentifierValue> typesList)
        {
            if(typesList.IsNullOrEmpty())
            {
                return 1;
            }

            return typesList.Min(p => p.Capacity ?? 1);
        }

        /// <inheritdoc/>
        public CallResult CheckAndTryConvert(IMonitorLogger logger, Value value, IList<StrongIdentifierValue> typesList, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return CheckAndTryConvert(logger, value, typesList, localCodeExecutionContext, DefaultOptions);
        }

        /// <inheritdoc/>
        public CallResult CheckAndTryConvert(IMonitorLogger logger, Value value, IList<StrongIdentifierValue> typesList, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            Info("50526CB3-643C-4602-8B17-58CB31D522CA", $"value = {value?.ToHumanizedString()}");
            Info("95E681A8-1612-4252-B477-F70B51097823", $"typesList = {typesList.WriteListToString()}");
#endif

            var checkResult = CheckFitValue(logger, value, typesList, localCodeExecutionContext, options);

#if DEBUG
            Info("AB5669E1-4B92-45A8-A562-5162241198CD", $"checkResult = {checkResult}");
#endif

            var kindOfResult = checkResult.KindOfResult;

            switch(kindOfResult)
            {
                case KindOfTypeFitCheckingResult.IsFit:
                    return new CallResult(value);

                case KindOfTypeFitCheckingResult.NeedConvesion:
                    return new CallResult(Convert(logger, value, checkResult.SuggestedType, localCodeExecutionContext, options));

                case KindOfTypeFitCheckingResult.IsNotFit:
                    throw new Exception($"435AEB0F-4EB7-4219-89D3-D63871296755: The value '{value.ToHumanizedString()}' does not fit to type `{typesList.TypesListToHumanizedString()}`");

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfResult), kindOfResult, null);
            }
        }

        /// <inheritdoc/>
        public TypeFitCheckingResult CheckFitValue(IMonitorLogger logger, Value value, IList<StrongIdentifierValue> typesList, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return CheckFitValue(logger, value, typesList, localCodeExecutionContext, DefaultOptions);
        }

        /// <inheritdoc/>
        public TypeFitCheckingResult CheckFitValue(IMonitorLogger logger, Value value, IList<StrongIdentifierValue> typesList, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            Info("F003F8DC-8153-42A6-86D5-9F544A627690", $"value = {value.ToHumanizedString()}");
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

            //if(value.IsRuleInstance && value.AsRuleInstance.KindOfRuleInstance == KindOfRuleInstance.Fact)
            //{
            //    е
            //}

            var isFit = _inheritanceResolver.IsFit(logger, typesList, value, localCodeExecutionContext, options);

            if (isFit)
            {
                return TypeFitCheckingResult.Fit;
            }

            //throw new Exception($"The value '{value.ToHumanizedString()}' does not fit to variable {CodeItem.ToHumanizedString()}");

            //throw new NotImplementedException("11924B32-F3D5-4CC4-93FA-D14C239F27C5");

            return TypeFitCheckingResult.IsNotFit;
        }

        /// <inheritdoc/>
        public Value Convert(IMonitorLogger logger, Value value, StrongIdentifierValue targetType, ILocalCodeExecutionContext localCodeExecutionContext)
        {
            return Convert(logger, value, targetType, localCodeExecutionContext, DefaultOptions);
        }

        /// <inheritdoc/>
        public Value Convert(IMonitorLogger logger, Value value, StrongIdentifierValue targetType, ILocalCodeExecutionContext localCodeExecutionContext, ResolverOptions options)
        {
#if DEBUG
            //Info("772AFCFF-0704-4AC2-A3C4-8BEF21681070", $"value = {value}");
            //Info("FB99DB07-9E1E-4904-8DAB-0E419C84C716", $"targetType = {targetType}");
#endif

            throw new NotImplementedException("30CA5B09-9171-4873-8893-DB3746C05036");
        }
    }
}
