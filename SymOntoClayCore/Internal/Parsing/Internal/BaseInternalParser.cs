/*MIT License

Copyright (c) 2020 - <curr_year/> Sergiy Tolkachov

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
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.Helpers;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public abstract class BaseInternalParser
    {
        protected BaseInternalParser(InternalParserContext context)
        {
            _context = context;
            _logger = context.Logger;
        }

        protected readonly InternalParserContext _context;
        private IEntityLogger _logger;

        public void Run()
        {
            OnEnter();

            while ((_currToken = _context.GetToken()) != null)
            {
                OnRun();

                if (_isExited)
                {
                    OnFinish();
                    return;
                }
            }

            OnFinish();
            OnFinishByEmpty();
        }

        protected Token _currToken = null;
        private bool _isExited = false;

        protected virtual void OnEnter()
        {
        }

        protected virtual void OnRun()
        {
        }

        protected virtual void OnFinish()
        {
        }

        protected virtual void OnFinishByEmpty()
        {
        }

        protected void Exit()
        {
            _isExited = true;
        }

        protected StrongIdentifierValue ParseName(string text)
        {
#if DEBUG
            //Log($"text = {text}");
#endif

            var name = NameHelper.CreateName(text);
            return name;
        }

        protected void SetCurrentCodeItem(CodeItem codeEntity)
        {
            _context.SetCurrentCodeItem(codeEntity);
        }

        protected void RemoveCurrentCodeEntity()
        {
            _context.RemoveCurrentCodeItem();
        }

        protected CodeItem CurrentCodeItem => _context.CurrentCodeItem;

        protected DefaultSettingsOfCodeEntity CurrentDefaultSetings => _context.CurrentDefaultSetings;
        protected void SetCurrentDefaultSetings(DefaultSettingsOfCodeEntity defaultSettings)
        {
            _context.SetCurrentDefaultSetings(defaultSettings);
        }

        protected void RemoveCurrentDefaultSetings()
        {
            _context.RemoveCurrentDefaultSetings();
        }

        protected CodeEntity CreateCodeEntity()
        {
            var result = new CodeEntity();
            DefaultSettingsOfCodeEntityHelper.SetUpCodeEntity(result, CurrentDefaultSetings);
            return result;
        }

        protected CodeEntity CreateCodeEntity(KindOfCodeEntity kind)
        {
            var result = CreateCodeEntity();
            result.Kind = kind;

            return result;
        }

        protected CodeEntity CreateCodeEntityAndSetAsCurrent(KindOfCodeEntity kind)
        {
            var result = CreateCodeEntity(kind);
            
            return result;
        }

        protected InlineTrigger CreateInlineTrigger()
        {
            var result = new InlineTrigger();
            DefaultSettingsOfCodeEntityHelper.SetUpInlineTrigger(result, CurrentDefaultSetings);

            FillUpCodeItem(result);

            return result;
        }

        protected LinguisticVariable CreateLinguisticVariableAndSetAsCurrentCodeItem()
        {
            var result = CreateLinguisticVariable();

            SetCurrentCodeItem(result);

            return result;
        }

        protected LinguisticVariable CreateLinguisticVariable()
        {
            var result = new LinguisticVariable();
            DefaultSettingsOfCodeEntityHelper.SetUpLinguisticVariable(result, CurrentDefaultSetings);

            FillUpCodeItem(result);            

            return result;
        }

        protected ActionDef CreateAction()
        {
            var result = new ActionDef();
            DefaultSettingsOfCodeEntityHelper.SetUpAction(result, CurrentDefaultSetings);

            FillUpCodeItem(result);

            return result;
        }

        protected NamedFunction CreateNamedFunctionAndSetAsCurrentCodeItem()
        {
            var result = CreateNamedFunction();

            SetCurrentCodeItem(result);

            return result;
        }

        protected NamedFunction CreateNamedFunction()
        {
            var result = new NamedFunction();
            DefaultSettingsOfCodeEntityHelper.SetUpNamedFunction(result, CurrentDefaultSetings);

            FillUpCodeItem(result);

            return result;
        }

        protected Operator CreateOperatorAndSetAsCurrentCodeItem()
        {
            var result = CreateOperator();

            SetCurrentCodeItem(result);

            return result;
        }

        protected Operator CreateOperator()
        {
            var result = new Operator();

            DefaultSettingsOfCodeEntityHelper.SetUpOperator(result, CurrentDefaultSetings);

            FillUpCodeItem(result);

            return result;
        }

        protected InheritanceItem CreateInheritanceItem()
        {
            var result = new InheritanceItem();
            DefaultSettingsOfCodeEntityHelper.SetUpInheritanceItem(result, CurrentDefaultSetings);

            return result;
        }

        private void FillUpCodeItem(CodeItem codeItem)
        {
            codeItem.CodeFile = _context.CodeFile;

            codeItem.ParentCodeEntity = CurrentCodeItem;
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        protected void Log(string message)
        {
            _logger.Log(message);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        protected void Warning(string message)
        {
            _logger.Warning(message);
        }

        /// <inheritdoc/>
        [MethodForLoggingSupport]
        protected void Error(string message)
        {
            _logger.Error(message);
        }
    }
}
