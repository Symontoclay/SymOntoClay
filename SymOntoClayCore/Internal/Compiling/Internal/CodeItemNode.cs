﻿using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Compiling.Internal
{
    public class CodeItemNode : BaseNode
    {
        public CodeItemNode(IMainStorageContext context)
            : base(context)
        {
        }

        public void Run(CodeItemAstExpression expression)
        {
#if DEBUG
            //Log($"expression = {expression}");
#endif

            CompilePushVal(expression.CodeItemValue);

            CompilePushAnnotation(expression);

            var command = new IntermediateScriptCommand();
            command.OperationCode = OperationCode.Instantiate;

            AddCommand(command);

#if DEBUG
            //DbgPrintCommands();
#endif
        }
    }
}