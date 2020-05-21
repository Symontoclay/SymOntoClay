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
            var applicationName = new Name();
            applicationName.OriginalNameValue = StandardNamesConstants.ApplicationTypeName;
            applicationName.OriginalNameKey = _context.Dictionary.GetKey(applicationName.OriginalNameValue);
            applicationName.NameKey = applicationName.OriginalNameKey;

            ApplicationName = applicationName;

#if IMAGINE_WORKING
            Log("Do");
#else
                throw new NotImplementedException();
#endif
        }
    }
}
