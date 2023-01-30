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

using System;
using System.Collections.Generic;
using System.Text;

namespace SymOntoClay.Core.Internal.Parsing.Internal
{
    public enum KeyWordTokenKind
    {
        /// <summary>
        /// Default value. Represents nothing.
        /// </summary>
        Unknown,
        Null,
        World,
        App,
        Lib,
        Class,
        Action,
        Actions,
        Fun,
        Constructor,
        Operator,
        Is,
        On,
        Init,
        Enter,
        Leave,
        Set,
        Not,
        And,
        Or,
        Select,
        Insert,
        LinguisticVariable,
        For,
        Range,
        Terms,
        Constraints,
        Relation,
        Inheritance,
        Error,
        Try,
        Catch,
        Else,
        Ensure,
        Where,
        Await,
        Wait,
        Complete,
        Break,
        Alias,
        Var,
        Public,
        Protected,
        Private,
        Return,
        If,
        Elif,
        While,
        Continue,
        Repeat,
        State,
        States,
        As,
        Default,
        Down,
        Duration,
        /// <summary>
        /// Repsesents symbol `_`.
        /// </summary>
        BlankIdentifier,
        Add,
        Fact,
        Reject,
        Exec,
        Synonym,
        Idle,
        With,
        Import,
        New,

        /// <summary>
        /// Special value for prediction.
        /// </summary>
        NamedParameter
    }
}
