using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazepointClient
{
    public class PointLabeler
    {
        public List<RegionOfInterest> RegionsOfInterest { get; set;} = new List<RegionOfInterest>();
        
        public PointLabeler(List<RegionOfInterest> regionOfInterests)
        {
            this.RegionsOfInterest = regionOfInterests;
        }

        private static int LinesIntersect(Line line1, Line line2)
        {
            if ((line1.k == line2.k) && (line1.n != line2.n)) return 1;  // regular intersect so count as 1
            
            return 0;  // no intersect or point on line (we won't count thsese) so count as 0
        }

        // count intersections by raycasting between an origin and the tested point
        // if even intersections then point is outside
        // if odd intersections then point is inside
        private static int CountIntersections(Point point, RegionOfInterest regionOfInterest)
        {
            int intersections = 0;

            Point rayOrigin = new Point(regionOfInterest.GetMinX() - 0.1 * point.y, point.y);
            Line ray = new Line(point, rayOrigin);

            // check only those sides behind which is the point that is tested
            List<Line> sidesToCheck = regionOfInterest.GetSidesAsLines().FindAll(line => line.k * point.x + line.n - point.y > 0);

            foreach(Line side in sidesToCheck)
            {
                intersections += LinesIntersect(side, ray);
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
