using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazepointClient.Model
{
    public class LineSegment
    {
        public Point p1 { get; set; }
        public Point p2 { get; set; }
        public double k { get; set; }
        public double n { get; set; }

        public LineSegment(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;

            this.k = (p2.y - p1.y) / (p2.x - p1.x);
            this.n = p2.y - p2.x * k;
        }

        public bool PointOnLineSegment(Point p)
        {
            return p.x <= Math.Max(p1.x, p2.x) && p.x >= Math.Min(p1.x, p2.x) &&
               p.y <= Math.Max(p1.y, p2.y) && p.y >= Math.Min(p1.y, p2.y) &&
               Math.Abs((p2.y - p1.y) * (p.x - p1.x) - (p.y - p1.y) * (p2.x - p1.x)) < double.Epsilon;
        }
    }
}
