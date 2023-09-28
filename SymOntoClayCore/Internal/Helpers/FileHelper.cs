/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using Newtonsoft.Json;
using NLog;
using SymOntoClay.Core.Internal.Parsing;
using SymOntoClay.Monitor.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Helpers
{
    public static class FileHelper
    {
        private static List<string> _rootExtesions = new List<string>() { ".sobj", ".world", ".slib" };
        private static List<string> _sourceFileExtensions = new List<string>() { ".soc" };

        public static List<ParsedFileInfo> GetParsedFilesInfo(IMonitorLogger logger, string projectFileName, string id)
        {
            var fileInfo = new FileInfo(projectFileName);

            if(!_rootExtesions.Contains(fileInfo.Extension))
            {
                if(string.IsNullOrWhiteSpace(id))
                {
                    throw new Exception($"Root file `{projectFileName}` has invalid extension `{fileInfo.Extension}`.");
                }
                else
                {
                    throw new Exception($"Root file `{projectFileName}` of entity `{id}` has invalid extension `{fileInfo.Extension}`.");
                }                
            }

            var existingFilesList = new List<string>();
            existingFilesList.Add(projectFileName);

            var result = new List<ParsedFileInfo>();
            result.Add(new ParsedFileInfo()
            {
                FileName = projectFileName,
                IsLocator = true
            });

            var directory = fileInfo.Directory;

            NGetParsedFilesInfoFromDirectory(logger, directory, result, existingFilesList);

            return result;
        }

        public static List<ParsedFileInfo> GetParsedFilesFromPaths(IMonitorLogger logger, IList<string> sourceCodePaths)
        {
            var result = new List<ParsedFileInfo>();

            foreach(var path in sourceCodePaths)
            {
                var itemsList = GetParsedFilesFromPath(logger, path);

                if(itemsList.Any())
                {
                    result.AddRange(itemsList);
                }
            }

            return result;
        }

        public static List<ParsedFileInfo> GetParsedFilesFromPath(IMonitorLogger logger, string sourceCodePath)
        {
            var directory = new DirectoryInfo(sourceCodePath);

            var result = new List<ParsedFileInfo>();
            var existingFilesList = new List<string>();

            NGetParsedFilesInfoFromDirectory(logger, directory, result, existingFilesList);

            return result;
        }

        private static void NGetParsedFilesInfoFromDirectory(IMonitorLogger logger, DirectoryInfo directory, List<ParsedFileInfo> result, List<string> existingFilesList)
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
                NGetParsedFilesInfoFromDirectory(logger, dir, result, existingFilesList);
            }
        }
    }
}
