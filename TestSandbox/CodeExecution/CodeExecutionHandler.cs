/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using NLog;
using SymOntoClay.Core.Internal.CodeExecution;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Ast.Expressions;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using SymOntoClay.Core.Internal.IndexedData.ScriptingData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using TestSandbox.Helpers;
using TestSandbox.Parsing;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.CodeExecution
{
    public class CodeExecutionHandler
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            var context = TstEngineContextHelper.CreateAndInitContext().EngineContext;

            var applicationInheritanceItem = new InheritanceItem()
            {
                IsSystemDefined = true
            };

            applicationInheritanceItem.SubName = NameHelper.CreateName("PeaseKeeper", context.Dictionary);
            applicationInheritanceItem.SuperName = context.CommonNamesStorage.NpcName;
            applicationInheritanceItem.Rank = new LogicalValue(1.0F);

            context.Storage.GlobalStorage.InheritanceStorage.SetInheritance(applicationInheritanceItem);

            var compiledFunctionBody = new CompiledFunctionBody();
            
            var strVal = new StringValue("The beatles!");

            var command = new ScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = strVal.GetIndexed(context);

            _logger.Log($"command = {command}");

            compiledFunctionBody.Commands[0] = command;

            var identifier = NameHelper.CreateName("@>log", context.Dictionary);

            command = new ScriptCommand();
            command.OperationCode = OperationCode.PushVal;
            command.Value = identifier.GetIndexed(context);
            command.Position = 1;

            _logger.Log($"command = {command}");

            compiledFunctionBody.Commands[1] = command;


            command = new ScriptCommand();
            command.OperationCode = OperationCode.CallBinOp;
            command.KindOfOperator = KindOfOperator.LeftRightStream;
            command.Position = 2;

            _logger.Log($"command = {command}");

            compiledFunctionBody.Commands[2] = command;

            command = new ScriptCommand();
            command.OperationCode = OperationCode.Return;
            command.Position = 3;

            _logger.Log($"command = {command}");

            compiledFunctionBody.Commands[3] = command;

            _logger.Log($"compiledFunctionBody = {compiledFunctionBody}");
            _logger.Log($"compiledFunctionBody = {compiledFunctionBody.ToDbgString()}");

            var codeFrame = new CodeFrame();
            codeFrame.CompiledFunctionBody = compiledFunctionBody;
            codeFrame.LocalContext = new LocalCodeExecutionContext();
            codeFrame.LocalContext.Storage = context.Storage.GlobalStorage;
            codeFrame.LocalContext.Holder = NameHelper.CreateName("PixKeeper", context.Dictionary).GetIndexed(context);
            //codeFrame.LocalContext.Holder = new Name();

            _logger.Log($"codeFrame = {codeFrame}");
            _logger.Log($"codeFrame = {codeFrame.ToDbgString()}");

            var threadExecutor = new SyncThreadExecutor(context);

            threadExecutor.SetCodeFrame(codeFrame);

            threadExecutor.Start();

            _logger.Log("End");
        }
    }
}
