/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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

using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClay.UnityAsset.Core
{
    public static class BuildPipeLine
    {
#if DEBUG
        //private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public static void CopyFiles(string sourceDir, string outputExeFile)
        {
#if DEBUG
            //_logger.Info($"sourceDir = {sourceDir}");
            //_logger.Info($"outputExeFile = {outputExeFile}");
#endif

            var fileInfo = new FileInfo(outputExeFile);

            var baseDirectory = fileInfo.DirectoryName;

            var fileName = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);

#if DEBUG
            //_logger.Info($"baseDirectory = {baseDirectory}");
            //_logger.Info($"fileName = {fileName}");
#endif

            var targetDir = Path.Combine(baseDirectory, $"{fileName}_Data");

#if DEBUG
            //_logger.Info($"targetDir = {targetDir}");
#endif

            CopySymOntoClaySourceFiles(sourceDir, targetDir);
        }

        private static void CopySymOntoClaySourceFiles(string sourceDir, string targetDir)
        {
#if DEBUG
            //_logger.Info($"sourceDir = {sourceDir}");
            //_logger.Info($"targetDir = {targetDir}");
#endif

            var sourceDirectoriesList = DetectDirectoriesWithSymOntoClaySourceFiles(sourceDir);

            if(!sourceDirectoriesList.Any())
            {
                return;
            }

#if DEBUG
            //_logger.Info($"sourceDirectoriesList.Count = {sourceDirectoriesList.Count}");
#endif
            foreach (var dir in sourceDirectoriesList)
            {
#if DEBUG
                //_logger.Info($"dir = {dir}");
#endif

                CopyDirectory(dir, sourceDir, targetDir);
            }
        }

        private static void CopyDirectory(string sourceDir, string baseSourceDir, string baseTargetDir)
        {
#if DEBUG
            //_logger.Info($"sourceDir = {sourceDir}");
            //_logger.Info($"baseSourceDir = {baseSourceDir}");
            //_logger.Info($"baseTargetDir = {baseTargetDir}");
#endif

            var newDir = sourceDir.Replace(baseSourceDir, baseTargetDir);

#if DEBUG
            //_logger.Info($"newDir = {newDir}");
#endif

            if(Directory.Exists(newDir))
            {
                Directory.Delete(newDir, true);
            }

            Directory.CreateDirectory(newDir);

            var filesNamesList = Directory.GetFiles(sourceDir);

            foreach(var fileName in filesNamesList)
            {
                if(fileName.EndsWith(".meta"))
                {
                    continue;
                }

#if DEBUG
                //_logger.Info($"fileName = {fileName}");
#endif

                var newFileName = fileName.Replace(baseSourceDir, baseTargetDir);

#if DEBUG
                //_logger.Info($"newFileName = {newFileName}");
#endif

                File.Copy(fileName, newFileName);
            }

            foreach (var subDir in Directory.GetDirectories(sourceDir))
            {
                CopyDirectory(subDir, baseSourceDir, baseTargetDir);
            }
        }

        private static List<string> DetectDirectoriesWithSymOntoClaySourceFiles(string sourceDir)
        {
            var result = new List<string>();

            EnumerateDirectoriesWithSymOntoClaySourceFiles(sourceDir, result);

            return result;
        }

        private static void EnumerateDirectoriesWithSymOntoClaySourceFiles(string sourceDir, List<string> result)
        {
            if(Directory.GetFiles(sourceDir).Any(p => p.EndsWith(".wspace")))
            {
                result.Add(sourceDir);
                return;
            }

            foreach(var subDir in Directory.GetDirectories(sourceDir))
            {
                EnumerateDirectoriesWithSymOntoClaySourceFiles(subDir, result);
            }
        }
    }
}
