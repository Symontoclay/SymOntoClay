using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SymOntoClay.DefaultCLIEnvironment
{
    public static class DefaultPaths
    {
        public static string GetBuiltInStandardLibraryDir()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "StandardLibrary");
        }
    }
}
