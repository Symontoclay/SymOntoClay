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
