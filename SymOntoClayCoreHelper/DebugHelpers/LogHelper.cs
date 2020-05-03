﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.CoreHelper.DebugHelpers
{
    public static class LogHelper
    {
        public static string BuildLogString(DateTime dateTime, string levelName, string className, string methodName, string message)
        {
            return $"{dateTime}|{levelName}|{className}|{methodName}|{message}";
        }
    }
}
