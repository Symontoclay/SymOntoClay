/*MIT License

Copyright (c) 2020 - 2023 Sergiy Tolkachov

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

using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.Parsing.Internal;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing
{
    public class Parser : BaseComponent, IParser
    {
        public Parser(IBaseCoreContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IBaseCoreContext _context;

        /// <inheritdoc/>
        public List<CodeItem> Parse(string text)
        {
            return Parse(text, true);
        }

        /// <inheritdoc/>
        public List<CodeItem> Parse(string text, bool needCheckDirty)
        {
#if DEBUG
            //Log($"text = {text}");
#endif

            var codeFile = new CodeFile();

            var internalParserContext = new InternalParserContext(text, codeFile, _context);
            internalParserContext.NeedCheckDirty = needCheckDirty;

            var parser = new SourceCodeParser(internalParserContext);
            parser.Run();

            return parser.Result;
        }

        /// <inheritdoc/>
        public CodeFile Parse(ParsedFileInfo parsedFileInfo, DefaultSettingsOfCodeEntity defaultSettings)
        {
#if DEBUG
            //Log($"parsedFileInfo = {parsedFileInfo}");
#endif

            var text = File.ReadAllText(parsedFileInfo.FileName);

#if DEBUG
            //Log($"text = {text}");
#endif

            var result = new CodeFile();
            result.IsLocator = parsedFileInfo.IsLocator;
            result.FileName = parsedFileInfo.FileName;
            
            var internalParserContext = new InternalParserContext(text, result, _context);
            
            internalParserContext.SetCurrentDefaultSetings(defaultSettings);

            var parser = new SourceCodeParser(internalParserContext);
            parser.Run();

            var codeEntitiesList = parser.Result;

#if DEBUG
            //Log($"codeEntitiesList = {codeEntitiesList.WriteListToString()}");
#endif

            result.CodeEntities = codeEntitiesList;

            return result;
        }

        /// <inheritdoc/>
        public List<CodeFile> Parse(List<ParsedFileInfo> parsedFileInfoList, DefaultSettingsOfCodeEntity defaultSettings)
        {
#if DEBUG
            //Log($"parsedFileInfoList = {parsedFileInfoList.WriteListToString()}");
#endif

            var result = new List<CodeFile>();

            foreach(var parsedFileInfo in parsedFileInfoList.Where(p => !p.IsLocator))
            {
                result.Add(Parse(parsedFileInfo, defaultSettings));
            }

            return result;
        }

        /// <inheritdoc/>
        public RuleInstance ParseRuleInstance(string text)
        {
            return ParseRuleInstance(text, true);
        }

        /// <inheritdoc/>
        public RuleInstance ParseRuleInstance(string text, bool needCheckDirty)
        {
            var codeEntity = Parse(text, needCheckDirty).First();

            if (codeEntity.Kind == KindOfCodeEntity.RuleOrFact)
            {
                return codeEntity.AsRuleInstance;
            }

            throw new NotSupportedException($"There can only be rule or fact here!");
        }
    }
}
