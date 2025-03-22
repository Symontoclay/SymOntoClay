/*MIT License

Copyright (c) 2020 - 2024 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.Storage;
using SymOntoClay.Core.Internal.Htn;
using SymOntoClay.Threading;

namespace SymOntoClay.Core.Internal
{
    public interface IEngineContext: IMainStorageContext
    {   
        ICodeExecutorComponent CodeExecutor { get; }
        IHtnExecutorComponent HtnExecutor { get; }

        IHostSupport HostSupport { get; }
        IHostListener HostListener { get; }
        IConditionalEntityHostSupport ConditionalEntityHostSupport { get; }
        ISoundPublisherProvider SoundPublisherProvider { get; }
        INLPConverterFactory NLPConverterFactory { get; }
        IStorageFactories StorageFactories { get; }

        INLPConverterContext GetNLPConverterContext();
        INLPConverterContext GetNLPConverterContext(ILocalCodeExecutionContext localCodeExecutionContext);

        ICustomThreadPool CodeExecutionThreadPool { get; }
        ICustomThreadPool TriggersThreadPool { get; }
    }
}
