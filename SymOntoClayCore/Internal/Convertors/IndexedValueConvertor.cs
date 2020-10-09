/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.IndexedData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Convertors
{
    public static class IndexedValueConvertor
    {
        public static IndexedLogicalValue ConvertNullValueToLogicalValue(IndexedNullValue source, IMainStorageContext mainStorageContext)
        {
            return ValueConvertor.ConvertNullValueToLogicalValue(source.OriginalNullValue, mainStorageContext).GetIndexed(mainStorageContext);
        }

        public static IndexedLogicalValue ConvertNumberValueToLogicalValue(IndexedNumberValue source, IMainStorageContext mainStorageContext)
        {
            return ValueConvertor.ConvertNumberValueToLogicalValue(source.OriginalNumberValue, mainStorageContext).GetIndexed(mainStorageContext);
        }

        public static IndexedNumberValue ConvertNullValueToNumberValue(IndexedNullValue source, IMainStorageContext mainStorageContext)
        {
            return ValueConvertor.ConvertNullValueToNumberValue(source.OriginalNullValue, mainStorageContext).GetIndexed(mainStorageContext);
        }

        public static IndexedNumberValue ConvertLogicalValueToNumberValue(IndexedLogicalValue source, IMainStorageContext mainStorageContext)
        {
            return ValueConvertor.ConvertLogicalValueToNumberValue(source.OriginalLogicalValue, mainStorageContext).GetIndexed(mainStorageContext);
        }
    }
}
