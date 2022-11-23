using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClayProjectFiles
{
    public static class WorldSpaceFilesSearcher
    {
#if DEBUG
        //private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public static WorldSpaceFiles Run(WorldSpaceFilesSearcherOptions options)
        {
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

                        return GetRunCommandFilesByWorldFile(wSpaceFile);
                    }
                    else
                    {
                        return GetWorldSpaceFilesByInputFile(inputNpcFile);
                    }
                }
            }
            else
            {
                return GetWorldSpaceFilesByInputFile(options.InputFile);
            }
        }

        private static WorldSpaceFiles GetRunCommandFilesByWorldFile(FileInfo wSpaceFile)
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

            var inputFile = DetectMainFileFllPathForMainNPCOfWorld(wSpaceFile.DirectoryName, mainNPCFileName, wSpaceJsonFile.Dirs);

#if DEBUG
            //_logger.Info($"inputFile = {inputFile}");
#endif

            if (!File.Exists(inputFile))
            {
                throw new NotImplementedException();
            }

            return NGetWorldSpaceFiles(inputFile, wSpaceFile, wSpaceJsonFile.Dirs);
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

            if (count == 1)
            {
                return filesList.Single();
            }

            throw new NotImplementedException();
        }

        private static WorldSpaceFiles GetWorldSpaceFilesByInputFile(string inputFile)
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

            return NGetWorldSpaceFiles(inputFile, wSpaceFile, wSpaceJsonFile.Dirs);
        }

        private static WorldSpaceFiles NGetWorldSpaceFiles(string inputFile, FileInfo wSpaceFile, WorldDirs worldDirs)
        {
#if DEBUG
            //_logger.Info($"inputFile = {inputFile}");
            //_logger.Info($"wSpaceFile = {wSpaceFile}");
            //_logger.Info($"worldDirs = {worldDirs}");
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

            result.SharedLibrariesDir = Path.Combine(baseDir, worldDirs.SharedLibs);

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
    }
}
