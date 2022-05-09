using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.Dot
{
    public abstract class BaseLeaf
    {
        protected BaseLeaf(DotContext context)
        {
            Context = context;
        }

        protected DotContext Context = null;

        public virtual bool IsContainer
        {
            get
            {
                return false;
            }
        }

        public virtual BaseLeaf SomeChildLeaf
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected StringBuilder Sb { get; set; }

        public string Text
        {
            get
            {
                if (Sb == null)
                {
                    return string.Empty;
                }

                return Sb.ToString();
            }
        }

        public string Name { get; protected set; } = string.Empty;

        public void Run()
        {
            Sb = new StringBuilder();

            OnRun();
        }

        protected virtual void OnRun()
        {
        }
    }
}
