using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSandbox.Navigations
{
    public class WayPoint
    {
        public string Name { get; set; }
        public List<Link> Links { get; set; } = new List<Link>();

        public override string ToString()
        {
            return $"[{Name} - {Links?.Count} links]";
        }
    }
}
