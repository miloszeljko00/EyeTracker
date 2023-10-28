using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazepointClient.Model
{
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

        public List<LineSegment> GetSidesAsLines()
        {
            var lines = new List<LineSegment>();

            for (int i = 0; i < points.Count; i++)
            {
                lines.Add(new LineSegment(points[i], points[(i + 1) % points.Count]));
            }

            return lines;
        }

        public double GetMinX()
        {
            return points.Min(p => p.X);
        }
    }
}
