/*MIT License

Copyright (c) 2020 - 2022 Sergiy Tolkachov

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

using DictionaryGenerator;
using NLog;
using SymOntoClay.NLP.CommonDict;
using SymOntoClay.NLP.CommonDict.Implementations;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestSandbox.Handlers
{
    public class GenerateDllDictHandler
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Run()//SymOntoClayNLPBinaryDict
        {
            _logger.Info("Begin");

            var outputFileName = Path.Combine(Directory.GetCurrentDirectory(), "main_dict.cs");

            _logger.Info($"outputFileName = {outputFileName}");

            var dict = GetJsonDict();
            //var fullDict = GetJsonDict();

            //var toFrame = fullDict["to"];

            //var dict = fullDict.Take(1000).ToDictionary(p => p.Key, p => p.Value);

            //dict["to"] = toFrame;

            //dict["i"].Single().ConditionalLogicalMeaning = toFrame.Single().ConditionalLogicalMeaning;

            _logger.Info($"dict.Count = {dict.Count}");

            var generator = new CSharpDictionaryGenerator(outputFileName, "MyTmpDict", dict);
            generator.Run();

            //var targetUnit = new CodeCompileUnit();

            //var targetNamespace = new CodeNamespace("CodeDOMSample");
            //targetNamespace.Imports.Add(new CodeNamespaceImport("System"));
            //var targetClass = new CodeTypeDeclaration("CodeDOMCreatedClass");
            //targetClass.IsClass = true;
            //targetClass.TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed;
            //targetNamespace.Types.Add(targetClass);
            //targetUnit.Namespaces.Add(targetNamespace);

            //targetClass.BaseTypes.Add("IWordsDict");

            //var provider = CodeDomProvider.CreateProvider("CSharp");
            //var options = new CodeGeneratorOptions();
            //options.BracingStyle = "C";
            //using (StreamWriter sourceWriter = new StreamWriter(outputFileName))
            //{
            //    provider.GenerateCodeFromCompileUnit(
            //        targetUnit, sourceWriter, options);
            //}



            _logger.Info("End");
        }

        private Dictionary<string, List<BaseGrammaticalWordFrame>> GetJsonDict()
        {
            var dictPath = Path.Combine(Directory.GetCurrentDirectory(), "main.dict");

            _logger.Info($"dictPath = {dictPath}");

            var serializer = new WordsDictJSONSerializationEngine();

            var dict = serializer.LoadFromFile(dictPath);

            return dict;
        }
    }
}
