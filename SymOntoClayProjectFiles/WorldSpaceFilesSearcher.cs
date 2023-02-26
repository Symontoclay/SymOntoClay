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

using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClay.ProjectFiles
{
    public static class WorldSpaceFilesSearcher
    {
#if DEBUG
        //private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public static WorldSpaceFiles Run(WorldSpaceFilesSearcherOptions options)
        {
            var appName = options.AppName;

            if(string.IsNullOrWhiteSpace(appName))
            {
                appName = @"SymOntoClay\UnknownApp";
            }

            if (string.IsNullOrWhiteSpace(options.InputFile))
            {
                if (string.IsNullOrWhiteSpace(options.InputDir))
                {
                    throw new NotImplementedException();
                }
                else
                {
                    var inputNpcFile = GetNpcFileNameInCurrentDir(options.InputDir);

#if DEBUG
                    //_logger.Info($"inputNpcFile = {inputNpcFile}");
#endif

                    if (string.IsNullOrWhiteSpace(inputNpcFile))
                    {
                        var wSpaceFile = WFilesSearcher.FindWSpaceFile(options.InputDir);

                        if (wSpaceFile == null)
                        {
                            throw new NotImplementedException();
                        }

                        return GetRunCommandFilesByWorldFile(wSpaceFile, appName, options.BaseTempDir, options.SearchMainNpcFile);
                    }
                    else
                    {
                        return GetWorldSpaceFilesByInputFile(inputNpcFile, appName, options.BaseTempDir);
                    }
                }
            }
            else
            {
                return GetWorldSpaceFilesByInputFile(options.InputFile, appName, options.BaseTempDir);
            }
        }

        private static WorldSpaceFiles GetRunCommandFilesByWorldFile(FileInfo wSpaceFile, string appName, string baseTempDir, bool searchMainNpcFile)
        {
#if DEBUG
            //_logger.Info($"wSpaceFile = {wSpaceFile}");
#endif

            var wSpaceJsonFile = WorldJsonFile.LoadFromFile(wSpaceFile.FullName);

#if DEBUG
            //_logger.Info($"wSpaceJsonFile = {wSpaceJsonFile}");
#endif

            var inputFile = string.Empty;

            if (searchMainNpcFile)
            {
                var mainNpc = wSpaceJsonFile.MainNpc;

#if DEBUG
                //_logger.Info($"mainNpc = {mainNpc}");
#endif

                if (string.IsNullOrWhiteSpace(mainNpc))
                {
                    throw new NotImplementedException();
                }

                var mainNPCFileName = $"{mainNpc}.sobj";

#if DEBUG
                //_logger.Info($"wSpaceFile.DirectoryName = {wSpaceFile.DirectoryName}");
#endif

                inputFile = DetectMainFileFllPathForMainNPCOfWorld(wSpaceFile.DirectoryName, mainNPCFileName, wSpaceJsonFile.Dirs);

#if DEBUG
                //_logger.Info($"inputFile = {inputFile}");
#endif

                if (!File.Exists(inputFile))
                {
                    throw new NotImplementedException();
                }
            }

            return NGetWorldSpaceFiles(inputFile, wSpaceFile, wSpaceJsonFile.Dirs, appName, baseTempDir);
        }

        private static string DetectMainFileFllPathForMainNPCOfWorld(string currDirName, string mainNPCFileName, WorldDirs worldDirs)
        {
#if DEBUG
            //_logger.Info($"currDirName = {currDirName}");
            //_logger.Info($"mainNPCFileName = {mainNPCFileName}");
#endif

            if (!string.IsNullOrWhiteSpace(worldDirs.World) && currDirName.EndsWith(worldDirs.World))
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(worldDirs.SharedLibs) && currDirName.EndsWith(worldDirs.SharedLibs))
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(worldDirs.Libs) && currDirName.EndsWith(worldDirs.Libs))
            {
                return string.Empty;
            }

            var targetFileName = Directory.EnumerateFiles(currDirName).SingleOrDefault(p => p.EndsWith(mainNPCFileName));

#if DEBUG
            //_logger.Info($"targetFileName = {targetFileName}");
#endif

            if (!string.IsNullOrWhiteSpace(targetFileName))
            {
                return targetFileName;
            }

            foreach (var subDir in Directory.EnumerateDirectories(currDirName))
            {
                targetFileName = DetectMainFileFllPathForMainNPCOfWorld(subDir, mainNPCFileName, worldDirs);

#if DEBUG
                //_logger.Info($"targetFileName (2) = {targetFileName}");
#endif

                if (!string.IsNullOrWhiteSpace(targetFileName))
                {
                    return targetFileName;
                }
            }

            return string.Empty;
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

            if (count == 1)
            {
                return filesList.Single();
            }

            throw new NotImplementedException();
        }

        private static WorldSpaceFiles GetWorldSpaceFilesByInputFile(string inputFile, string appName, string baseTempDir)
        {
#if DEBUG
            //_logger.Info($"inputFile = {inputFile}");
#endif

            var fileInfo = new FileInfo(inputFile);

            var wSpaceFile = WFilesSearcher.FindWSpaceFile(fileInfo.Directory);

#if DEBUG
            //_logger.Info($"wSpaceFile = {wSpaceFile}");
#endif

            var wSpaceJsonFile = WorldJsonFile.LoadFromFile(wSpaceFile.FullName);

#if DEBUG
            //_logger.Info($"wSpaceJsonFile = {wSpaceJsonFile}");
#endif

            if (wSpaceFile == null)
            {
                throw new NotImplementedException();
            }

            return NGetWorldSpaceFiles(inputFile, wSpaceFile, wSpaceJsonFile.Dirs, appName, baseTempDir);
        }

        private static WorldSpaceFiles NGetWorldSpaceFiles(string inputFile, FileInfo wSpaceFile, WorldDirs worldDirs, string appName, string baseTempDir)
        {
#if DEBUG
            //_logger.Info($"inputFile = {inputFile}");
            //_logger.Info($"wSpaceFile = {wSpaceFile}");
            //_logger.Info($"worldDirs = {worldDirs}");
            //_logger.Info($"appName = {appName}");
            //_logger.Info($"baseTempDir = {baseTempDir}");
#endif

            var result = new WorldSpaceFiles();

            result.LogicFile = inputFile;

            var baseDir = wSpaceFile.Directory.FullName;

#if DEBUG
            //_logger.Info($"baseDir = {baseDir}");
#endif

            var worldDir = Path.Combine(baseDir, worldDirs.World);

#if DEBUG
            //_logger.Info($"worldDir = {worldDir}");
#endif

            var worldDirInfo = new DirectoryInfo(worldDir);

            var worldFile = worldDirInfo.EnumerateFiles().Single(p => p.Name.EndsWith(".world"));

#if DEBUG
            //_logger.Info($"worldFile = {worldFile}");
#endif

            result.WorldFile = worldFile.FullName;

            result.SharedLibsDir = Path.Combine(baseDir, worldDirs.SharedLibs);

#if DEBUG
            //_logger.Info($"result.SharedLibsDir = {result.SharedLibsDir}");
#endif

            result.LibsDir = Path.Combine(baseDir, worldDirs.Libs);

            if(string.IsNullOrWhiteSpace(baseTempDir))
            {
                result.ImagesRootDir = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), appName, "Images");

#if DEBUG
                //_logger.Info($"result.ImagesRootDir = {result.ImagesRootDir}");
#endif

                result.TmpDir = Path.Combine(Environment.GetEnvironmentVariable("TMP"), appName);

#if DEBUG
                //_logger.Info($"result.TmpDir = {result.TmpDir}");
#endif
            }
            else
            {
                result.ImagesRootDir = Path.Combine(baseTempDir, appName, "Images");

#if DEBUG
                //_logger.Info($"result.ImagesRootDir = {result.ImagesRootDir}");
#endif

                result.TmpDir = Path.Combine(baseTempDir, appName);

#if DEBUG
                //_logger.Info($"result.TmpDir = {result.TmpDir}");
#endif
            }

            return result;
        }
    }
}
