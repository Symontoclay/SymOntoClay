/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Core;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters.Helpers;

namespace SymOntoClay.UnityAsset.Core.Internal.TypesConverters.DefaultConverters
{
    [PlatformTypesConverter]
    public class Vector3AndStrongIdentifierValueConverter: BasePlatformTypesConverter
    {        
        /// <inheritdoc/>
        public override Type PlatformType => typeof(Vector3);

        /// <inheritdoc/>
        public override Type CoreType => typeof(StrongIdentifierValue);

        /// <inheritdoc/>
        public override bool CanConvertToPlatformType => true;

        /// <inheritdoc/>
        public override bool CanConvertToCoreType => false;

        /// <inheritdoc/>
        public override object ConvertToCoreType(object platformObject, IEngineContext context, LocalCodeExecutionContext localContext)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override object ConvertToPlatformType(object coreObject, IEngineContext context, LocalCodeExecutionContext localContext)
        {
            var identifier = (StrongIdentifierValue)coreObject;

#if DEBUG
            //var logger = context.Logger;
            //logger.Log($"identifier = {identifier}");
#endif

            var kindOfName = identifier.KindOfName;

            switch(kindOfName)
            {
                case KindOfName.Entity:
                    return ConvertEntityToPlatformType(identifier, context, localContext);

                case KindOfName.Concept:
                    return ConvertConceptToPlatformType(identifier, context, localContext);

                default:
                    throw new ArgumentOutOfRangeException(nameof(kindOfName), kindOfName, null);
            }
        }

        private object ConvertEntityToPlatformType(StrongIdentifierValue identifier, IEngineContext context, LocalCodeExecutionContext localContext)
        {
            var entityValue = PlatformTypesConverterHelper.GetResolvedEntityValue(identifier, context, localContext);

            return entityValue.Position;
        }

        private object ConvertConceptToPlatformType(StrongIdentifierValue concept, IEngineContext context, LocalCodeExecutionContext localContext)
        {
#if DEBUG
            //var logger = context.Logger;
            //logger.Log($"concept = {concept}");
#endif

            var conditionalEntityValue = PlatformTypesConverterHelper.GetResolvedConditionalEntityValue(concept, context, localContext);

            return conditionalEntityValue.Position;
        }
    }
}
