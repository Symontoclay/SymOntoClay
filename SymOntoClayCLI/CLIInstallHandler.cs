/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

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
using SymOntoClay.CLI.Helpers;
using SymOntoClay.ProjectFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.CLI
{
    public class CLIInstallHandler
    {
        public void Run(CLICommand command)
        {
            var libName = command.ProjectName;

            if (!command.NoLogo)
            {
                ConsoleWrapper.WriteOutput("Only installation from a local source is supported!");
                ConsoleWrapper.WriteText($"Installing `{libName}`...");
            }

            var worldSpaceFilesSearcherOptions = new WorldSpaceFilesSearcherOptions()
            {
                InputDir = command.InputDir
            };

            var targetFiles = WorldSpaceFilesSearcher.Run(worldSpaceFilesSearcherOptions);

            var socAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(p => p.GetName().Name == "soc").Location;

            var assemblyPath = (new FileInfo(socAssembly)).DirectoryName;

            var libraryBaseSource = Path.Combine(assemblyPath, "LibsForInstall");

            var librarySource = Path.Combine(libraryBaseSource, libName);

            if(!Directory.Exists(librarySource))
            {
                if (!command.NoLogo)
                {
                    ConsoleWrapper.WriteError($"Unknown library `{libName}`!");
                }
                    
                return;
            }

            var libraryDest = Path.Combine(targetFiles.SharedLibsDir, libName);

            if(Directory.Exists(libraryDest))
            {
                if (!command.NoLogo)
                {
                    ConsoleWrapper.WriteOutput($"Library `{libName}` already exists. It will be updated.");
                }

                Directory.Delete(libraryDest, true);
            }

            Directory.CreateDirectory(libraryDest);

            CopyDirectory(librarySource, librarySource, libraryDest);

            if (!command.NoLogo)
            {
                ConsoleWrapper.WriteText($"Library `{libName}` has been installed!");
            }
        }

        private void CopyDirectory(string dir, string baseSourceDir, string baseDestDir)
        {
            var relativeDirName = dir.Replace(baseSourceDir, string.Empty);

            var destDirName = Path.Combine(baseDestDir, relativeDirName);

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            var subDirs = Directory.GetDirectories(dir);

            foreach(var subDir in subDirs)
            {
                CopyDirectory(subDir, baseSourceDir, baseDestDir);
            }

            var files = Directory.GetFiles(dir);

            foreach(var file in files)
            {
                var relativeFileName = file.Replace(baseSourceDir, string.Empty).Substring(1);

                var newFileName = Path.Combine(baseDestDir, relativeFileName);

                File.Copy(file, newFileName, true);
            }
        }
    }
}
