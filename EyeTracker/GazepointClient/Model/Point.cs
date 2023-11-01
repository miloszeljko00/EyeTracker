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

        public static Point FractionCoordinatesToAbsoluteCoordinates(Point point, int screenWidth, int screenHeight)
        {
            return new Point(screenWidth * point.X, screenHeight * point.Y);
        }
    }
}
