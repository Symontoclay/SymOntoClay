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

namespace SymOntoClay.BaseTestLib
{
    public static class BehaviorTestEngineRunner
    {
        public static bool RunMinimalInstance(string fileContent, Func<int, string, bool> logHandler)
        {
            var builder = new BehaviorTestEngineInstanceBuilder();
            var testInstance = builder.CreateMinimalInstance(fileContent, logHandler);
            return testInstance.Run();
        }

        public static bool RunMinimalInstanceTimeoutBased(string fileContent, Action<int, string> logHandler)
        {
            var builder = new BehaviorTestEngineInstanceBuilder();
            var testInstance = builder.CreateMinimalInstance(fileContent, logHandler);
            return testInstance.Run();
        }
        
        public static bool RunMinimalInstanceTimeoutBased(string fileContent, Action<string> logHandler)
        {
            var builder = new BehaviorTestEngineInstanceBuilder();
            var testInstance = builder.CreateMinimalInstance(fileContent, logHandler);
            return testInstance.Run();
        }

        public static bool RunMinimalInstanceWithCategories(string fileContent, List<string> categories, Func<int, string, bool> logHandler)
        {
            var builder = new BehaviorTestEngineInstanceBuilder();
            var testInstance = builder.CreateMinimalInstanceWithCategories(fileContent, categories, logHandler);
            return testInstance.Run();
        }

        public static bool RunMinimalInstanceWithImportStandardLibrary(string fileContent, Func<int, string, bool> logHandler)
        {
            var builder = new BehaviorTestEngineInstanceBuilder();
            var testInstance = builder.CreateMinimalInstanceWithImportStandardLibrary(fileContent, logHandler);
            return testInstance.Run();
        }

        public static bool RunMinimalInstanceTimeoutBasedWithImportStandardLibrary(string fileContent, Action<int, string> logHandler)
        {
            var builder = new BehaviorTestEngineInstanceBuilder();
            var testInstance = builder.CreateMinimalInstanceWithImportStandardLibrary(fileContent, logHandler);
            return testInstance.Run();
        }

        public static bool RunMinimalInstanceWithPlatformListener(string fileContent, Func<int, string, bool> logHandler, object platformListener)
        {
            var builder = new BehaviorTestEngineInstanceBuilder();
            var testInstance = builder.CreateMinimalInstanceWithPlatformListener(fileContent, logHandler, platformListener);
            return testInstance.Run();
        }

        public static bool RunMinimalInstanceTimeoutBasedWithPlatformListener(string fileContent, Action<int, string> logHandler, object platformListener)
        {
            var builder = new BehaviorTestEngineInstanceBuilder();
            var testInstance = builder.CreateMinimalInstanceWithPlatformListener(fileContent, logHandler, platformListener);
            return testInstance.Run();
        }

        public static bool RunMinimalInstanceWithPlatformListenerAndImportStandardLibrary(string fileContent, Func<int, string, bool> logHandler, object platformListener)
        {
            var builder = new BehaviorTestEngineInstanceBuilder();
            var testInstance = builder.CreateMinimalInstanceWithPlatformListenerAndImportStandardLibrary(fileContent, logHandler, platformListener);
            return testInstance.Run();
        }

        public static bool RunMinimalInstanceWithOneHtnIteration(string fileContent, Func<int, string, bool> logHandler)
        {
            var builder = new BehaviorTestEngineInstanceBuilder();
            var testInstance = builder.CreateMinimalInstanceWithOneHtnIteration(fileContent, logHandler);
            return testInstance.Run();
        }
    }
}
