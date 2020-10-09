/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

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
            _dictionary = context.Dictionary;
        }

        protected readonly InternalParserContext _context;
        private IEntityLogger _logger;
        private IEntityDictionary _dictionary;

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

        protected ulong GetKey(string name)
        {
            return _dictionary.GetKey(name);
        }

        protected StrongIdentifierValue ParseName(string text)
        {
#if DEBUG
            //Log($"text = {text}");
#endif

            var name = NameHelper.CreateName(text, _context.Dictionary);
            return name;
        }

        protected void SetCurrentCodeEntity(CodeEntity codeEntity)
        {
            _context.SetCurrentCodeEntity(codeEntity);
        }

        protected void RemoveCurrentCodeEntity()
        {
            _context.RemoveCurrentCodeEntity();
        }

        protected CodeEntity CurrentCodeEntity => _context.CurrentCodeEntity;

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

        protected InlineTrigger CreateInlineTrigger()
        {
            var result = new InlineTrigger();
            DefaultSettingsOfCodeEntityHelper.SetUpInlineTrigger(result, CurrentDefaultSetings);
            return result;
        }

        protected InheritanceItem CreateInheritanceItem()
        {
            var result = new InheritanceItem();
            DefaultSettingsOfCodeEntityHelper.SetUpInheritanceItem(result, CurrentDefaultSetings);
            return result;
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
