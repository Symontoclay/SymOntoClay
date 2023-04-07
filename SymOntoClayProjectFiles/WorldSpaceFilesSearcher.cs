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
            var wSpaceJsonFile = WorldJsonFile.LoadFromFile(wSpaceFile.FullName);

            var inputFile = string.Empty;

            if (searchMainNpcFile)
            {
                var mainNpc = wSpaceJsonFile.MainNpc;

                if (string.IsNullOrWhiteSpace(mainNpc))
                {
                    throw new NotImplementedException();
                }

                var mainNPCFileName = $"{mainNpc}.sobj";

                inputFile = DetectMainFileFllPathForMainNPCOfWorld(wSpaceFile.DirectoryName, mainNPCFileName, wSpaceJsonFile.Dirs);

                if (!File.Exists(inputFile))
                {
                    throw new NotImplementedException();
                }
            }

            return NGetWorldSpaceFiles(inputFile, wSpaceFile, wSpaceJsonFile.Dirs, appName, baseTempDir);
        }

        private static string DetectMainFileFllPathForMainNPCOfWorld(string currDirName, string mainNPCFileName, WorldDirs worldDirs)
        {
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

            if (!string.IsNullOrWhiteSpace(targetFileName))
            {
                return targetFileName;
            }

            foreach (var subDir in Directory.EnumerateDirectories(currDirName))
            {
                targetFileName = DetectMainFileFllPathForMainNPCOfWorld(subDir, mainNPCFileName, worldDirs);

                if (!string.IsNullOrWhiteSpace(targetFileName))
                {
                    return targetFileName;
                }
            }

            return string.Empty;
        }

        private static string GetNpcFileNameInCurrentDir(string inputDir)
        {
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
            var fileInfo = new FileInfo(inputFile);

            var wSpaceFile = WFilesSearcher.FindWSpaceFile(fileInfo.Directory);

            var wSpaceJsonFile = WorldJsonFile.LoadFromFile(wSpaceFile.FullName);

            if (wSpaceFile == null)
            {
                throw new NotImplementedException();
            }

            return NGetWorldSpaceFiles(inputFile, wSpaceFile, wSpaceJsonFile.Dirs, appName, baseTempDir);
        }

        private static WorldSpaceFiles NGetWorldSpaceFiles(string inputFile, FileInfo wSpaceFile, WorldDirs worldDirs, string appName, string baseTempDir)
        {
            var result = new WorldSpaceFiles();

            result.LogicFile = inputFile;

            var baseDir = wSpaceFile.Directory.FullName;

            var worldDir = Path.Combine(baseDir, worldDirs.World);

            var worldDirInfo = new DirectoryInfo(worldDir);

            var worldFile = worldDirInfo.EnumerateFiles().Single(p => p.Name.EndsWith(".world"));

            result.WorldFile = worldFile.FullName;

            result.SharedLibsDir = Path.Combine(baseDir, worldDirs.SharedLibs);

            result.LibsDir = Path.Combine(baseDir, worldDirs.Libs);

            if(string.IsNullOrWhiteSpace(baseTempDir))
            {
                result.ImagesRootDir = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), appName, "Images");

                result.TmpDir = Path.Combine(Environment.GetEnvironmentVariable("TMP"), appName);

            }
            else
            {
                result.ImagesRootDir = Path.Combine(baseTempDir, appName, "Images");

                result.TmpDir = Path.Combine(baseTempDir, appName);

            }

            return result;
        }
    }
}
