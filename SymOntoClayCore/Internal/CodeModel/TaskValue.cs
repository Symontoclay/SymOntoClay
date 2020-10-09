/*Copyright (C) 2020 Sergiy Tolkachov aka metatypeman

This file is part of SymOntoClay.

SymOntoClay is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; version 2.1.

SymOntoClay is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; if not, see <https://www.gnu.org/licenses/>*/

using SymOntoClay.Core.Internal.Convertors;
using SymOntoClay.Core.Internal.IndexedData;
using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class TaskValue : Value
    {
        public TaskValue(Task systemTask)
        {
            SystemTask = systemTask;
            TaskId = Guid.NewGuid().ToString("D");
        }

        /// <inheritdoc/>
        public override KindOfValue KindOfValue => KindOfValue.TaskValue;

        public string TaskId { get; set; }
        public Task SystemTask { get; set; }

        public IndexedTaskValue Indexed { get; set; }

        public IndexedTaskValue GetIndexed(IMainStorageContext mainStorageContext)
        {
            if (Indexed == null)
            {
                return ConvertorToIndexed.ConvertTaskValue(this, mainStorageContext);
            }

            return Indexed;
        }

        /// <inheritdoc/>
        public override IndexedValue GetIndexedValue(IMainStorageContext mainStorageContext)
        {
            return GetIndexed(mainStorageContext);
        }

        /// <inheritdoc/>
        public override IndexedAnnotatedItem IndexedAnnotatedItem => Indexed;

        /// <inheritdoc/>
        public override IndexedAnnotatedItem GetIndexedAnnotatedItem(IMainStorageContext mainStorageContext)
        {
            return GetIndexed(mainStorageContext);
        }

        /// <inheritdoc/>
        public override IndexedAnnotatedItem GetIndexedAnnotatedItem(IMainStorageContext mainStorageContext, Dictionary<object, object> convertingContext)
        {
            if (Indexed == null)
            {
                return ConvertorToIndexed.ConvertTaskValue(this, mainStorageContext, convertingContext);
            }

            return Indexed;
        }

        /// <inheritdoc/>
        public override bool IsTaskValue => true;

        /// <inheritdoc/>
        public override TaskValue AsTaskValue => this;

        /// <inheritdoc/>
        public override object GetSystemValue()
        {
            return SystemTask;
        }

        /// <inheritdoc/>
        public override AnnotatedItem CloneAnnotatedItem(Dictionary<object, object> context)
        {
            return CloneValue(context);
        }

        /// <inheritdoc/>
        public override Value CloneValue(Dictionary<object, object> cloneContext)
        {
            if (cloneContext.ContainsKey(this))
            {
                return (Value)cloneContext[this];
            }

            var result = new TaskValue(SystemTask);
            cloneContext[this] = result;

            result.TaskId = TaskId;
            result.AppendAnnotations(this, cloneContext);

            return result;
        }

        /// <inheritdoc/>
        protected override string PropertiesToString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(TaskId)} = {TaskId}");
            sb.AppendLine($"{spaces}SystemTask?.Status = {SystemTask?.Status}");

            sb.PrintExisting(n, nameof(Indexed), Indexed);

            sb.Append(base.PropertiesToString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToShortString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(TaskId)} = {TaskId}");
            sb.AppendLine($"{spaces}SystemTask?.Status = {SystemTask?.Status}");

            sb.PrintExisting(n, nameof(Indexed), Indexed);

            sb.Append(base.PropertiesToShortString(n));
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override string PropertiesToBriefString(uint n)
        {
            var spaces = DisplayHelper.Spaces(n);
            var sb = new StringBuilder();
            sb.AppendLine($"{spaces}{nameof(TaskId)} = {TaskId}");
            sb.AppendLine($"{spaces}SystemTask?.Status = {SystemTask?.Status}");

            sb.PrintExisting(n, nameof(Indexed), Indexed);

            sb.Append(base.PropertiesToBriefString(n));
            return sb.ToString();
        }
    }
}
