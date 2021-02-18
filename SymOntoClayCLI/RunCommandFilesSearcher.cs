/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

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

using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClay.CLI
{
    public static class RunCommandFilesSearcher
    {
#if DEBUG
        //private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public static RunCommandFiles Run(CLICommand command)
        {
            if(string.IsNullOrWhiteSpace(command.InputFile))
            {
                if(string.IsNullOrWhiteSpace(command.InputDir))
                {
                    throw new NotImplementedException();
                }
                else
                {
                    var inputNpcFile = GetNpcFileNameInCurrentDir(command.InputDir);

#if DEBUG
                    //_logger.Info($"inputNpcFile = {inputNpcFile}");
#endif

                    if(string.IsNullOrWhiteSpace(inputNpcFile))
                    {
                        var wSpaceFile = FindWSpaceFile(command.InputDir);

                        if(wSpaceFile == null)
                        {
                            throw new NotImplementedException();
                        }

                        return GetRunCommandFilesByWorldFile(wSpaceFile);
                    }
                    else
                    {
                        return GetRunCommandFilesByInputFile(inputNpcFile);
                    }                    
                }
            }
            else
            {
                return GetRunCommandFilesByInputFile(command.InputFile);
            }
        }

        private static RunCommandFiles GetRunCommandFilesByWorldFile(FileInfo wSpaceFile)
        {
#if DEBUG
            //_logger.Info($"wSpaceFile = {wSpaceFile}");
#endif

            var wSpaceJsonFile = WorldJsonFile.LoadFromFile(wSpaceFile.FullName);

#if DEBUG
            //_logger.Info($"wSpaceJsonFile = {wSpaceJsonFile}");
#endif

            var mainNpc = wSpaceJsonFile.MainNpc;

#if DEBUG
            //_logger.Info($"mainNpc = {mainNpc}");
#endif

            if (string.IsNullOrWhiteSpace(mainNpc))
            {
                throw new NotImplementedException();
            }

            var mainNPCFileName = $"{mainNpc}.sobj";

            var inputFile = DetectMainFileFllPathForMainNPCOfWorld(wSpaceFile.DirectoryName, mainNPCFileName);

#if DEBUG
            //_logger.Info($"inputFile = {inputFile}");
#endif

            if (!File.Exists(inputFile))
            {
                throw new NotImplementedException();
            }

            return NGetRunCommandFiles(inputFile, wSpaceFile);
        }

        private static string DetectMainFileFllPathForMainNPCOfWorld(string currDirName, string mainNPCFileName)
        {
#if DEBUG
            //_logger.Info($"currDirName = {currDirName}");
            //_logger.Info($"mainNPCFileName = {mainNPCFileName}");
#endif

            if(currDirName.EndsWith("World"))
            {
                return string.Empty;
            }

            if (currDirName.EndsWith("Modules"))
            {
                return string.Empty;
            }

            var targetFileName = Directory.EnumerateFiles(currDirName).SingleOrDefault(p => p.EndsWith(mainNPCFileName));

#if DEBUG
            //_logger.Info($"targetFileName = {targetFileName}");
#endif

            if(!string.IsNullOrWhiteSpace(targetFileName))
            {
                return targetFileName;
            }

            foreach(var subDir in Directory.EnumerateDirectories(currDirName))
            {
                targetFileName = DetectMainFileFllPathForMainNPCOfWorld(subDir, mainNPCFileName);

#if DEBUG
                //_logger.Info($"targetFileName (2) = {targetFileName}");
#endif

                if (!string.IsNullOrWhiteSpace(targetFileName))
                {
                    return targetFileName;
                }
            }

            throw new NotImplementedException();
        }

        private static string GetNpcFileNameInCurrentDir(string inputDir)
        {
#if DEBUG
            //_logger.Info($"inputDir = {inputDir}");
#endif

            var filesList = Directory.EnumerateFiles(inputDir).Where(p => p.EndsWith(".sobj"));

            var count = filesList.Count();

            if (count == 0)
            {
                return string.Empty;
            }

            if(count == 1)
            {
                return filesList.Single();
            }

            throw new NotImplementedException();
        }

        private static RunCommandFiles GetRunCommandFilesByInputFile(string inputFile)
        {
#if DEBUG
            //_logger.Info($"inputFile = {inputFile}");
#endif

            var fileInfo = new FileInfo(inputFile);

            var wSpaceFile = FindWSpaceFile(fileInfo.Directory);

#if DEBUG
            //_logger.Info($"wSpaceFile = {wSpaceFile}");
#endif

            if (wSpaceFile == null)
            {
                throw new NotImplementedException();
            }

            return NGetRunCommandFiles(inputFile, wSpaceFile);
        }

        private static RunCommandFiles NGetRunCommandFiles(string inputFile, FileInfo wSpaceFile)
        {
#if DEBUG
            //_logger.Info($"inputFile = {inputFile}");
            //_logger.Info($"wSpaceFile = {wSpaceFile}");
#endif

            var result = new RunCommandFiles();

            result.LogicFile = inputFile;

            var baseDir = wSpaceFile.Directory.FullName;

#if DEBUG
            //_logger.Info($"baseDir = {baseDir}");
#endif

            var worldDir = Path.Combine(baseDir, "World");

#if DEBUG
            //_logger.Info($"worldDir = {worldDir}");
#endif

            var worldDirInfo = new DirectoryInfo(worldDir);

            var worldFile = worldDirInfo.EnumerateFiles().Single(p => p.Name.EndsWith(".world"));

#if DEBUG
            //_logger.Info($"worldFile = {worldFile}");
#endif

            result.WorldFile = worldFile.FullName;

            result.SharedModulesDir = Path.Combine(baseDir, "Modules");

#if DEBUG
            //_logger.Info($"result.SharedModulesDir = {result.SharedModulesDir}");
#endif

            result.ImagesRootDir = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), "SymOntoClay", "CLI", "Images");

#if DEBUG
            //_logger.Info($"result.ImagesRootDir = {result.ImagesRootDir}");
#endif

            result.TmpDir = Path.Combine(Environment.GetEnvironmentVariable("TMP"), "SymOntoClay");

#if DEBUG
            //_logger.Info($"result.TmpDir = {result.TmpDir}");
#endif

            return result;
        }

        public static FileInfo FindWSpaceFile(string dir)
        {
            return FindWSpaceFile(new DirectoryInfo(dir));
        }

        private static FileInfo FindWSpaceFile(DirectoryInfo dir)
        {
            var wSpaceFilesList = dir.EnumerateFiles().Where(p => p.Name.EndsWith(".wspace"));

            var count = wSpaceFilesList.Count();

            switch (count)
            {
                case 0:
                    {
                        var parentDir = dir.Parent;

                        if(parentDir == null)
                        {
                            return null;
                        }

                        return FindWSpaceFile(parentDir);
                    }

                case 1:
                    return wSpaceFilesList.First();

                default:
                    throw new ArgumentOutOfRangeException(nameof(count), count, null);
            }
        }
    }
}
