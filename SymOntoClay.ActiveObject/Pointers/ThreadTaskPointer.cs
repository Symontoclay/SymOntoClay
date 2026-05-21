using SymOntoClay.Common.SerializationToImage.Attributes;
using SymOntoClay.Threading;
using System;

namespace SymOntoClay.ActiveObject.Pointers
{
    public class ThreadTaskPointer: IThreadTaskPointer
    {
        public ThreadTaskPointer()
        {
        }

        public ThreadTaskPointer(IThreadTask task)
        {
            Task = task;
        }

        /// <inheritdoc/>
        [SystemNoSerializedMember]
        public IThreadTask Task { get; set; } = null;
    }

    public class ThreadTaskPointer<TResult>: IThreadTaskPointer<TResult>
    {
        public ThreadTaskPointer()
        {
        }

        public ThreadTaskPointer(IThreadTask<TResult> task)
        {
            TaskWithResult = task;
        }

        /// <inheritdoc/>
        public IThreadTask Task { get => TaskWithResult; set => throw new NotImplementedException("6A3E0F80-0F59-4654-9191-DB4BD098BEF9"); }

        /// <inheritdoc/>
        [SystemNoSerializedMember]
        public IThreadTask<TResult> TaskWithResult { get; set; } = null;
    }
}
