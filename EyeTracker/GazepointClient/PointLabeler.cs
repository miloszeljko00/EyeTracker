using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GazepointClient.Model;
using static System.Formats.Asn1.AsnWriter;

namespace GazepointClient
{
    public class PointLabeler
    {
        public List<RegionOfInterest> RegionsOfInterest { get; set;} = new List<RegionOfInterest>();
        
        public PointLabeler(List<RegionOfInterest> regionOfInterests)
        {
            this.RegionsOfInterest = regionOfInterests;
        }

        private static int LineSegmentsIntersect(LineSegment line1, LineSegment line2)
        {
            // two lines will intersect iff slopes are different
            if ((Math.Abs(line1.K - line2.K) > 0)) {
            
                // find intercept point to see if it's on the specific line segments that these two lines represent
                var interceptX = (line2.N - line1.N) / (line1.K - line2.K);
                var interceptY = line1.K * interceptX + line1.N;
                Point interceptPoint = new Point(interceptX, interceptY);

                // if the point is on both line segments then there is an intercept between two line segments
                if (line1.PointOnLineSegment(interceptPoint) && line2.PointOnLineSegment(interceptPoint)) return 1;

                return 0;
            }

            return 0;
        }

        // count intersections by raycasting between an origin and the tested point
        // if even intersections then point is outside
        // if odd intersections then point is inside
        private static int CountIntersections(Point point, RegionOfInterest regionOfInterest)
        {
            int intersections = 0;

            Point rayOrigin = new Point(regionOfInterest.GetMinX() - 0.1 * point.Y, point.Y);
            LineSegment ray = new LineSegment(point, rayOrigin);

            foreach(LineSegment side in regionOfInterest.GetSidesAsLines())
            {
                intersections += LineSegmentsIntersect(side, ray);
            }

            return intersections;
        }

        public string LabelSignalObjectsData(Point point, List<int> screenSize)
        {
            int intersections;

            foreach(RegionOfInterest regionOfInterest in RegionsOfInterest)
            {
                intersections = CountIntersections(point, regionOfInterest);

                if((intersections % 2) == 1) // odd so it's inside of the polygon
                {
                    return regionOfInterest.region;
                }
            }

            return "Not in any ROIs";
        }
    }
}
