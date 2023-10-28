using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazepointClient.Model
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public Point(ROIPoint rOIPoint)
        {
            this.X = rOIPoint.X;
            this.Y = rOIPoint.Y;
        }

        public static Point FractionCoordinatesToAbsoluteCoordinates(Point point, List<int> screenSize)
        {
            return new Point(screenSize[0] * point.X, screenSize[1] * point.Y);
        }
    }
}
