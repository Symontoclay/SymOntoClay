using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.DataResolvers
{
    public enum ReplacingNotResultsStrategy
    {
        /// <summary>
        /// Elements of the result will be replaced by other elements that do not belong to set of the replaced elements.
        /// The replacing elements can be of all kinds.
        /// </summary>
        AllKindOfItems,

        /// <summary>
        /// Elements of the result will be replaced by other elements that do not belong to set of the replaced elements.
        /// The replacing elements can have only kinds of replaced elements.
        /// </summary>
        PresentKindOfItems,

        /// <summary>
        /// Elements of the result will be replaced by other elements that do not belong to set of the replaced elements.
        /// Firstly replacing elements of kinds of replaced elements are placed.
        /// Next replacing elements of other kinds  are placed.
        /// </summary>
        FirstPresentNextOtherKindOfItems,

        /// <summary>
        /// Elements of the result will be replaced by other elements that do not belong to set of the replaced elements.
        /// The replacing elements can have only dominant kind of replaced elements.
        /// </summary>
        DominantKindOfItems
    }
}
