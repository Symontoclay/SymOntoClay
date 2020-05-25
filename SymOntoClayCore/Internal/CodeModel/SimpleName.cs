using SymOntoClay.CoreHelper.DebugHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CodeModel
{
    public class SimpleName : BaseLoggedComponent, IObjectToString, IObjectToShortString, IObjectToBriefString
    {
        public SimpleName(string text, string nspace, ICodeModelContext context)
            : base(context.Logger)
        {
            throw new NotSupportedException("Namespaces are not supported yet!");

            _context = context;
        }

        public SimpleName(string text, ICodeModelContext context)
            : base(context.Logger)
        {
            _context = context;

            NameValue = text;
            FullNameValue = text;

            CalculateIndex();
        }

        private readonly ICodeModelContext _context;

        public string NameValue { get; private set; }
        public string FullNameValue { get; private set; }

        public ulong FullNameKey { get; private set; }

        public void CalculateIndex()
        {
            var dictionary = _context.Dictionary;

            FullNameKey = dictionary.GetKey(FullNameValue);
        }
    }
}
