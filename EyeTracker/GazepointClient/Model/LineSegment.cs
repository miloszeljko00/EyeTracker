using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazepointClient.Model
{
    public class LineSegment
    {
        public Point P1 { get; set; }
        public Point P2 { get; set; }
        public double K { get; set; }
        public double N { get; set; }

        public LineSegment(Point p1, Point p2)
        {
            this.P1 = p1;
            this.P2 = p2;

            this.K = (p2.Y - p1.Y) / (p2.X - p1.X);
            this.N = p2.Y - p2.X * K;
        }

        public bool PointOnLineSegment(Point p)
        {
            return p.X <= Math.Max(P1.X, P2.X) && p.X >= Math.Min(P1.X, P2.X) &&  // between min and max x
               p.Y <= Math.Max(P1.Y, P2.Y) && p.Y >= Math.Min(P1.Y, P2.Y) &&  // between min and max y
               Math.Abs((P2.Y - P1.Y) * (p.X - P1.X) - (p.Y - P1.Y) * (P2.X - P1.X)) < double.Epsilon;  // the 3 points are collienar
        }
    }
}
