/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Convertors
{
    public static class ValueConvertor
    {
        public static LogicalValue ConvertNullValueToLogicalValue(NullValue source, IMainStorageContext mainStorageContext)
        {
            var result = new LogicalValue(null);

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext);

            return result;
        }

        public static LogicalValue ConvertNumberValueToLogicalValue(NumberValue source, IMainStorageContext mainStorageContext)
        {
            var result = new LogicalValue((float?)source.AsNumberValue.SystemValue);

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext);

            return result;
        }

        public static NumberValue ConvertNullValueToNumberValue(NullValue source, IMainStorageContext mainStorageContext)
        {
            var result = new NumberValue(null);

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext);

            return result;
        }

        public static NumberValue ConvertLogicalValueToNumberValue(LogicalValue source, IMainStorageContext mainStorageContext)
        {
            var result = new NumberValue(source.AsLogicalValue.SystemValue);

            FillAnnotationsModalitiesAndSections(source, result, mainStorageContext);

            return result;
        }

        private static void FillAnnotationsModalitiesAndSections(AnnotatedItem source, AnnotatedItem dest, IMainStorageContext mainStorageContext)
        {
            throw new NotImplementedException();
        }
    }
}
