using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SymOntoClay.CoreHelper.DebugHelpers
{
    public static class QuickLogger
    {
        private static string _fileName;
        private readonly static object _lockObj = new object();

        static QuickLogger()
        {
            _fileName = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), $"SymOntoClay_{DateTime.Now:dd_MM_yyyy_HH_mm_ss_ffff}.log");
        }

        public static void Log(string txt)
        {
            lock (_lockObj)
            {
                File.AppendAllText(_fileName, txt);
            }            
        }
    }
}
