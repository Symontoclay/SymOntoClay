/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Parsing.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing
{
    public class Parser : BaseComponent, IParser
    {
        public Parser(IMainStorageContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IMainStorageContext _context;

        public CodeFile Parse(ParsedFileInfo parsedFileInfo)
        {
#if DEBUG
            //Log($"parsedFileInfo = {parsedFileInfo}");
#endif

            var text = File.ReadAllText(parsedFileInfo.FileName);

#if DEBUG
            //Log($"text = {text}");
#endif

            var result = new CodeFile();
            result.IsLocator = parsedFileInfo.IsLocator;
            result.FileName = parsedFileInfo.FileName;
            
            var internalParserContext = new InternalParserContext(text, result, _context);

            var globalStorage = _context.Storage.GlobalStorage;
            internalParserContext.SetCurrentDefaultSetings(globalStorage.DefaultSettingsOfCodeEntity);

            var parser = new SourceCodeParser(internalParserContext);
            parser.Run();

            var codeEntitiesList = parser.Result;

#if DEBUG
            //Log($"codeEntitiesList = {codeEntitiesList.WriteListToString()}");
#endif

            result.CodeEntities = codeEntitiesList;

            return result;
        }

        public List<CodeFile> Parse(List<ParsedFileInfo> parsedFileInfoList)
        {
#if DEBUG
            //Log($"parsedFileInfoList = {parsedFileInfoList.WriteListToString()}");
#endif

            var result = new List<CodeFile>();

            foreach(var parsedFileInfo in parsedFileInfoList.Where(p => !p.IsLocator))
            {
                result.Add(Parse(parsedFileInfo));
            }

            return result;
        }
    }
}
