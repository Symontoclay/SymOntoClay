/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using Newtonsoft.Json;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Helpers
{
    public static class ExpressionNodeHelper
    {
        public static bool Compare(BaseIndexedLogicalQueryNode expressionNode1, BaseIndexedLogicalQueryNode expressionNode2, List<ulong> additionalKeys, IEntityLogger logger)
        {
#if DEBUG
            //logger.Log($"(expressionNode1 == null) = {expressionNode1 == null} (expressionNode2 == null) = {expressionNode2 == null}");
            //logger.Log($"expressionNode1 = {expressionNode1}");
            //logger.Log($"expressionNode2 = {expressionNode2}");
            //logger.Log($"additionalKeys = {JsonConvert.SerializeObject(additionalKeys, Formatting.Indented)}");
#endif

            if (expressionNode1.IsKeyRef && expressionNode2.IsKeyRef)
            {
                var key = expressionNode2.AsKeyRef.Key;

                if (expressionNode1.AsKeyRef.Key == key)
                {
                    return true;
                }

                if(additionalKeys != null && additionalKeys.Any(p => p == key))
                {
                    return true;
                }

                return false;
            }

            throw new NotImplementedException();
        }
    }
}
