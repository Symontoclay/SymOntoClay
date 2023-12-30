using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Monitor.Common
{
    public struct Changer
    {
        public Changer(KindOfChanger kindOfChanger, string id)
        {
            KindOfChanger = kindOfChanger;
            Id = id;
        }

        public KindOfChanger KindOfChanger { get; }
        public string Id { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[{KindOfChanger}: {Id}]";
        }
    }
}
