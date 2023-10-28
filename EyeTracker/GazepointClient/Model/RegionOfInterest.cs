using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Contracts;

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

        public RegionOfInterest(ROI regionOfInterest)
        {
            this.points = new List<Point>();
            foreach(ROIPoint rOIPoint in regionOfInterest.Points)
            {
                this.points.Add(new Point(rOIPoint));
            }

            this.region = regionOfInterest.Id;
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
