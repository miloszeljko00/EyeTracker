using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazepointClient
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

    public class Line
    {
        public double k { get; set; }
        public double n { get; set; }

        public Line(Point p1, Point p2)
        {
            this.k = (p2.y - p1.y) / (p2.x - p1.x);
            this.n = p2.y - p2.x * k;
        }
    }

    public class RegionOfInterest
    {
        public List<Point> points { get; set; }
        public string region { get; set; }

        public RegionOfInterest(List<Point> points, string region)
        {
            this.points = points;
            this.region = region;
        }

        public int GetNumberOfSides()
        {
            return points.Count;
        }

        public List<Line> GetSidesAsLines()
        {
            var lines = new List<Line>();

            for(int i = 0; i < points.Count; i++)
            {
                lines.Add(new Line(points[i], points[(i+1) % points.Count]));
            }

            return lines;
        }

        public List<Tuple<Point, Point>> GetSidesAsPointPairs()
        {
            var sides = new List<Tuple<Point, Point>>();

            for(int i = 0; i < points.Count; i++)
            {
                sides.Add(new Tuple<Point, Point>(points[i], points[(i+1) % points.Count]));
            }

            return sides;
        }

        public double GetMinX()
        {
            return points.Min(p => p.x);
        }
    }
}
