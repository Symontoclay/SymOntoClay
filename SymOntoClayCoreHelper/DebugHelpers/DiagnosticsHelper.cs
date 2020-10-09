/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace SymOntoClay.CoreHelper.DebugHelpers
{
    public static class DiagnosticsHelper
    {
        [MethodForLoggingSupport]
        public static DiagnosticsCallInfo GetNotLoggingSupportCallInfo()
        {
            var className = string.Empty;
            var methodName = string.Empty;
            var framesToSkip = 0;

            while (true)
            {
                var frame = new StackFrame(framesToSkip, false);

                var method = frame.GetMethod();

                var attribute = method?.GetCustomAttribute<MethodForLoggingSupportAttribute>();

                var declaringType = method?.DeclaringType;

                if (declaringType == null)
                {
                    break;
                }

                if (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                framesToSkip++;
                className = declaringType.FullName;
                methodName = method.Name;

                if (attribute == null)
                {
                    break;
                }
            }

            var result = new DiagnosticsCallInfo();
            result.FullClassName = className;
            result.MethodName = methodName;
            return result;
        }
    }
}
