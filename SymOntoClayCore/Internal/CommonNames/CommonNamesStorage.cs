using SymOntoClay.Core.Internal.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.CommonNames
{
    public class CommonNamesStorage : BaseComponent, ICommonNamesStorage
    {
        public CommonNamesStorage(IEngineContext context)
            : base(context.Logger)
        {
            _context = context;
        }

        private readonly IEngineContext _context;

        public Name ApplicationName { get; private set; }

        public void LoadFromSourceCode()
        {
            throw new NotImplementedException();

            //TODO: Resolve this after refactoring.
            //var applicationName = new Name(_context.CodeModelContext);
            //applicationName.OriginalNameValue = StandardNamesConstants.ApplicationTypeName;

            throw new NotImplementedException();

            //TODO: Resolve this after refactoring.
            //applicationName.OriginalNameKey = _context.Dictionary.GetKey(applicationName.OriginalNameValue);
            //applicationName.NameKey = applicationName.OriginalNameKey;

            //ApplicationName = applicationName;

#if IMAGINE_WORKING
            Log("Do");
#else
                throw new NotImplementedException();
#endif
        }
    }
}
