using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSandbox.Navigations
{
    public class NavPath
    {
        public WayPoint Start { get; set; }
        public WayPoint End { get; set; }
        public List<WayPoint> Corners { get; set; } = new List<WayPoint>();

        public override string ToString()
        {
            return $"[{Start?.Name};{End?.Name} : {string.Join("<->", Corners.Select(p => p.Name))}]";
        }
    }
}
