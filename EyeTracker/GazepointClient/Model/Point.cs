using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazepointClient.Model
{
    public class Point
    {
        public double x { get; set; }
        public double y { get; set; }

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point FractionCoordinatesToAbsoluteCoordinates(Point point, List<int> screenSize)
        {
            return new Point(screenSize[0] * point.x, screenSize[1] * point.y);
        }
    }
}
