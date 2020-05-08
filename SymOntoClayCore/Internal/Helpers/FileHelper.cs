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
        private static List<string> Extesions = new List<string>() { ".txt" };

        public static List<ParsedFileInfo> GetParsedFilesInfo(string appFileName, string id)
        {
            var fileInfo = new FileInfo(appFileName);

            if(!Extesions.Contains(fileInfo.Extension))
            {
                throw new Exception($"Root file `{appFileName}` of entity `{id}` has invalid extension `{fileInfo.Extension}`.");
            }

            var existingFilesList = new List<string>();
            existingFilesList.Add(appFileName);

            var result = new List<ParsedFileInfo>();
            result.Add(new ParsedFileInfo() 
            {
                FileName = appFileName,
                IsMain = true
            });

            var directory = fileInfo.Directory;

            NGetParsedFilesInfoFromDirectory(directory, result, existingFilesList);

            return result;
        }

        private static void NGetParsedFilesInfoFromDirectory(DirectoryInfo directory, List<ParsedFileInfo> result, List<string> existingFilesList)
        {
            var filesList = directory.EnumerateFiles().Where(p => Extesions.Contains(p.Extension) && !existingFilesList.Contains(p.FullName)).Select(p => p.FullName);

            foreach(var fileName in filesList)
            {
                existingFilesList.Add(fileName);
                result.Add(new ParsedFileInfo()
                {
                    FileName = fileName,
                    IsMain = false
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
