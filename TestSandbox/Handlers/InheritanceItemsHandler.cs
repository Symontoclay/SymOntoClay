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

using NLog;
using SymOntoClay.Core.Internal.CodeModel;
using SymOntoClay.Core.Internal.CodeModel.Helpers;
using System;
using System.Collections.Generic;
using SymOntoClay.CoreHelper.DebugHelpers;
using TestSandbox.Helpers;
using TestSandbox.PlatformImplementations;

namespace TestSandbox.Handlers
{
    public class InheritanceItemsHandler
    {
        private static readonly IEntityLogger _logger = new LoggerImpementation();

        public void Run()
        {
            _logger.Log("Begin");

            var context = TstEngineContextHelper.CreateAndInitContext().EngineContext;

            //var dictionary = context.Dictionary;

            var inheritanceStorage = context.Storage.GlobalStorage.InheritanceStorage;

            var subName = NameHelper.CreateName("SubClass");
            var superName = NameHelper.CreateName("SuperClass");

            var inheritanceItem = new InheritanceItem();
            inheritanceItem.SubName = subName;
            inheritanceItem.SuperName = superName;
            inheritanceItem.Rank = new LogicalValue(1);

            _logger.Log($"inheritanceItem = {inheritanceItem}");

            inheritanceStorage.SetInheritance(inheritanceItem);

            inheritanceItem = new InheritanceItem();
            inheritanceItem.SubName = subName;
            inheritanceItem.SuperName = superName;
            inheritanceItem.Rank = new LogicalValue(0.5F);

            _logger.Log($"inheritanceItem = {inheritanceItem}");

            inheritanceStorage.SetInheritance(inheritanceItem);

            inheritanceItem = new InheritanceItem();
            inheritanceItem.SubName = subName;
            inheritanceItem.SuperName = superName;
            inheritanceItem.Rank = new LogicalValue(0);

            _logger.Log($"inheritanceItem = {inheritanceItem}");

            inheritanceStorage.SetInheritance(inheritanceItem);

            var list = inheritanceStorage.GetItemsDirectly(subName);

            _logger.Log($"list.Count = {list.Count}");
            _logger.Log($"inheritanceItem = {list.WriteListToString()}");

            _logger.Log("End");
        }
    }
}
