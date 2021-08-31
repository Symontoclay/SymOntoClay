using SymOntoClayProjectFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.CLI
{
    public class CLINewWorldspaceHandler
    {
        public void Run(CLICommand command)
        {
            var worldSpaceCreationSettings = new WorldSpaceCreationSettings() 
            { 
                ProjectName = command.ProjectName, 
                CreateOnlyWorldspace = true 
            };

            WorldSpaceCreator.CreateWithOutWSpaceFile(worldSpaceCreationSettings, Directory.GetCurrentDirectory()
                , errorMsg => ConsoleWrapper.WriteError(errorMsg)
            );
        }
    }
}
