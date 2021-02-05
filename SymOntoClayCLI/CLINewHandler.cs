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

using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClay.CLI
{
    public class CLINewHandler
    {
#if DEBUG
        //private readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

        public void Run(CLICommand command)
        {
            var wSpaceFile = RunCommandFilesSearcher.FindWSpaceFile(Directory.GetCurrentDirectory());

            if(wSpaceFile == null)
            {
                CreateWithOutWSpaceFile(command);
            }
            else
            {
                CreateWithWSpaceFile(command, wSpaceFile);
            }
        }

        private void CreateWithWSpaceFile(CLICommand command, FileInfo wSpaceFile)
        {
            var appDir = wSpaceFile.Directory.GetDirectories().SingleOrDefault(p => p.Name == "Npcs");

            if(appDir == null)
            {
                throw new NotImplementedException();
            }

            CreateNPC(command, appDir.FullName);
        }

        private void CreateWithOutWSpaceFile(CLICommand command)
        {
            var projectName = command.ProjectName;

            var worldSpaceDirName = Path.Combine(Directory.GetCurrentDirectory(), projectName);

            if(!Directory.Exists(worldSpaceDirName))
            {
                Directory.CreateDirectory(worldSpaceDirName);
            }

            var wSpaceFileName = Path.Combine(worldSpaceDirName, $"{projectName}.wspace");

            if(!File.Exists(wSpaceFileName))
            {
                var wSpaceJsonFile = new WorldJsonFile() { MainNpc = projectName };

                File.WriteAllText(wSpaceFileName, JsonConvert.SerializeObject(wSpaceJsonFile, Formatting.Indented));
            }            

            var worldDirName = Path.Combine(worldSpaceDirName, "World");

            if (!Directory.Exists(worldDirName))
            {
                Directory.CreateDirectory(worldDirName);
            }

            var worldFileName = Path.Combine(worldDirName, $"{projectName}.world");

            if (!File.Exists(worldFileName))
            {
                File.WriteAllText(worldFileName, "{}");
            }

            var hostsDirName = Path.Combine(worldSpaceDirName, "Things");

            if (!Directory.Exists(hostsDirName))
            {
                Directory.CreateDirectory(hostsDirName);
            }

            var playerDirName = Path.Combine(worldSpaceDirName, "Players");

            if(!Directory.Exists(playerDirName))
            {
                Directory.CreateDirectory(playerDirName);
            }

            var modulesDirName = Path.Combine(worldSpaceDirName, "Modules");

            if (!Directory.Exists(modulesDirName))
            {
                Directory.CreateDirectory(modulesDirName);
            }

            var npcsDirName = Path.Combine(worldSpaceDirName, "Npcs");

            if (!Directory.Exists(npcsDirName))
            {
                Directory.CreateDirectory(npcsDirName);
            }

            CreateNPC(command, npcsDirName);
        }

        private void CreateNPC(CLICommand command, string npcsDirName)
        {
            var projectName = command.ProjectName;

            var projectDirName = Path.Combine(npcsDirName, projectName);

            if(Directory.Exists(projectDirName))
            {
                ConsoleWrapper.WriteError($"The NPC '{projectName}' already exists!");
                return;
            }
            else
            {
                Directory.CreateDirectory(projectDirName);
            }

            var npcFileName = Path.Combine(projectDirName, $"{projectName}.sobj");

            File.WriteAllText(npcFileName, "{}");

            var appFileName = Path.Combine(projectDirName, $"{projectName}.soc");

            var sb = new StringBuilder();
            sb.AppendLine($"app {projectName}");
            sb.AppendLine("{");
            sb.AppendLine("    on Init =>");
            sb.AppendLine("    {");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(appFileName, sb.ToString());
        }
    }
}
