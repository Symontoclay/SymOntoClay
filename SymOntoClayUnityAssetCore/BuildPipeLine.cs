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
        public static void CopyFiles(string sourceDir, string outputExeFile)
        {
            var fileInfo = new FileInfo(outputExeFile);

            var baseDirectory = fileInfo.DirectoryName;

            var fileName = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);

            var targetDir = Path.Combine(baseDirectory, $"{fileName}_Data");

            CopySymOntoClaySourceFiles(sourceDir, targetDir);

            CopyNLPDicts(sourceDir, targetDir);
        }

        private static void CopyNLPDicts(string sourceDir, string targetDir)
        {
            var sourceDictsDir = Path.Combine(sourceDir, "SymOntoClay", "Dicts");
            var targetDictsDir = Path.Combine(targetDir, "SymOntoClay", "Dicts");

            CopyDirectory(sourceDictsDir, sourceDictsDir, targetDictsDir);
        }

        private static void CopySymOntoClaySourceFiles(string sourceDir, string targetDir)
        {
            var sourceDirectoriesList = DetectDirectoriesWithSymOntoClaySourceFiles(sourceDir);

            if(!sourceDirectoriesList.Any())
            {
                return;
            }

            foreach (var dir in sourceDirectoriesList)
            {
                CopyDirectory(dir, sourceDir, targetDir);
            }
        }

        private static void CopyDirectory(string sourceDir, string baseSourceDir, string baseTargetDir)
        {
            var newDir = sourceDir.Replace(baseSourceDir, baseTargetDir);

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

                var newFileName = fileName.Replace(baseSourceDir, baseTargetDir);

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
