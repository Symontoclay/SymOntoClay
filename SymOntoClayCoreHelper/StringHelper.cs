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

namespace SymOntoClay.CoreHelper
{
    public static class StringHelper
    {
        public static string ToHtmlCode(string source)
        {
            var sb = new StringBuilder();

            foreach(var ch in source)
            {
                sb.Append(ToHtmlCode(ch));
            }

            return sb.ToString();
        }

        public static string ToHtmlCode(char ch)
        {
            switch(ch)
            {
                case '&':
                    return "&amp;";

                case '|':
                    return "&#124;";

                case '=':
                    return "&#61;";

                case '>':
                    return "&#62;";

                case '<':
                    return "&#60;";

                case '{':
                    return "&#123;";

                case '}':
                    return "&#125;";

                case '[':
                    return "&#91;";

                case ']':
                    return "&#93;";

                case '%':
                    return "&#37;";

                case '$':
                    return "&#36;";

                case '#':
                    return "&#35;";

                case '@':
                    return "&#64;";

                case '!':
                    return "&#33;";

                case '?':
                    return "&#63;";

                case '.':
                    return "&#46;";

                case ',':
                    return "&#44;";

                case '(':
                    return "&#40;";

                case ')':
                    return "&#41;";

                case '+':
                    return "&#43;";

                case '*':
                    return "&#42;";

                case '/':
                    return "&#47;";

                case '\\':
                    return "&#92;";

                case ';':
                    return "&#59;";

                case ':':
                    return "&#58;";

                case '"':
                    return "&#34;";

                case '\'':
                    return "&#39;";

                case '`':
                    return "&#96;";

                default:
                    return ch.ToString();
            }
        }
    }
}
