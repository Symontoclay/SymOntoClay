/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using Newtonsoft.Json;
using NLog;
using SymOntoClay.Core.Internal.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Helpers
{
    public static class FileHelper
    {
        private static List<string> _rootExtesions = new List<string>() { ".sobj", ".world" };
        private static List<string> _sourceFileExtensions = new List<string>() { ".soc" };

        public static List<ParsedFileInfo> GetParsedFilesInfo(string appFileName, string id)
        {
            var fileInfo = new FileInfo(appFileName);

            if(!_rootExtesions.Contains(fileInfo.Extension))
            {
                throw new Exception($"Root file `{appFileName}` of entity `{id}` has invalid extension `{fileInfo.Extension}`.");
            }

            var existingFilesList = new List<string>();
            existingFilesList.Add(appFileName);

            var result = new List<ParsedFileInfo>();
            result.Add(new ParsedFileInfo()
            {
                FileName = appFileName,
                IsLocator = true
            });

            var directory = fileInfo.Directory;

            NGetParsedFilesInfoFromDirectory(directory, result, existingFilesList);

            return result;
        }

        private static void NGetParsedFilesInfoFromDirectory(DirectoryInfo directory, List<ParsedFileInfo> result, List<string> existingFilesList)
        {
            var filesList = directory.EnumerateFiles().Where(p => _sourceFileExtensions.Contains(p.Extension) && !existingFilesList.Contains(p.FullName)).Select(p => p.FullName);

            foreach(var fileName in filesList)
            {
                existingFilesList.Add(fileName);
                result.Add(new ParsedFileInfo()
                {
                    FileName = fileName,
                    IsLocator = false
                });
            }

            var directoriesList = directory.EnumerateDirectories();

            foreach(var dir in directoriesList)
            {
                NGetParsedFilesInfoFromDirectory(dir, result, existingFilesList);
            }
        }
    }
}
