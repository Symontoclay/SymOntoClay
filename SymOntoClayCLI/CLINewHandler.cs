/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
            var appDir = wSpaceFile.Directory.GetDirectories().SingleOrDefault(p => p.Name == "Apps");

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

            var hostsDirName = Path.Combine(worldSpaceDirName, "Hosts");

            if (!Directory.Exists(hostsDirName))
            {
                Directory.CreateDirectory(hostsDirName);
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

            var npcFileName = Path.Combine(projectDirName, $"{projectName}.npc");

            File.WriteAllText(npcFileName, "{}");

            var appFileName = Path.Combine(projectDirName, $"{projectName}.soc");

            var sb = new StringBuilder();
            sb.AppendLine($"npc {projectName}");
            sb.AppendLine("{");
            sb.AppendLine("    on Init =>");
            sb.AppendLine("    {");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            sb.AppendLine();

            File.WriteAllText(appFileName, sb.ToString());
        }
    }
}
