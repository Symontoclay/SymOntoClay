using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSandbox.Navigations
{
    public class Link
    {
        public WayPoint A { get; set; }
        public WayPoint B { get; set; }

        public override string ToString()
        {
            return $"[{A?.Name} <-> {B?.Name}]";
        }
    }
}
