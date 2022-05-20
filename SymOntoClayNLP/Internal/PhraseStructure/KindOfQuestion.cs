using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.NLP.Internal.PhraseStructure
{
    public enum KindOfQuestion
    {
        Undefined,
        None,
        /// <summary>
        /// Does a good student work smart at school?
        /// </summary>
        General,
        /// <summary>
        /// Who works smart at school and enjoy learning?
        /// </summary>
        Subject,
        /// <summary>
        /// What does it mean to work smart?
        /// </summary>
        Special,
        /// <summary>
        /// A good student works smart at school and enjoys learning, does not he?
        /// </summary>
        Tag
    }
}
