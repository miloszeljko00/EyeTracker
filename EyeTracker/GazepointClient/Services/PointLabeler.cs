using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using GazepointClient.Model;
using static System.Formats.Asn1.AsnWriter;

namespace GazepointClient.Services
{
    public class PointLabeler
    {
        public List<RegionOfInterest> RegionsOfInterest { get; set; } = new List<RegionOfInterest>();

        public PointLabeler(List<RegionOfInterest> regionOfInterests)
        {
            RegionsOfInterest = regionOfInterests;
        }

        public PointLabeler(ROIConfig roiConfig)
        {
            foreach (ROI roi in roiConfig.ROIs)
            {
                RegionsOfInterest.Add(new RegionOfInterest(roi));
            }
        }

        private static int LineSegmentsIntersect(LineSegment line1, LineSegment line2)
        {
            // just cast negative inf to positive inf if needed
            line1.K = line1.K == double.NegativeInfinity ? double.PositiveInfinity : line1.K;
            line2.K = line2.K == double.NegativeInfinity ? double.PositiveInfinity : line2.K;

            // if either slope is infinite (but not at the same time) then they surely intercept
            if ((line1.K == double.PositiveInfinity) ^ (line2.K == double.PositiveInfinity))
            {
                // if one is vertical then they must intercept on the vertical ones' x coordinate
                var interceptX = line1.K == double.PositiveInfinity ? line1.P1.X : line2.P1.X;
                var interceptY = line1.K == double.PositiveInfinity ? line2.K * interceptX + line2.N : line1.K * interceptX - line1.N;
                Point interceptPoint = new Point(interceptX, interceptY);

                // if the point is on both line segments then there is an intercept between two line segments
                if (line1.PointOnLineSegment(interceptPoint) && line2.PointOnLineSegment(interceptPoint)) return 1;

                return 0;
            }

            // two lines will intersect iff slopes are different
            if ((Math.Abs(line1.K - line2.K) > 0))
            {

                // find intercept point to see if it's on the specific line segments that these two lines represent
                var interceptX = (line2.N - line1.N) / (line1.K - line2.K);
                var interceptY = line1.K * interceptX + line1.N;
                Point interceptPoint = new Point(interceptX, interceptY);

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

            foreach (LineSegment side in regionOfInterest.GetSidesAsLines())
            {
                intersections += LineSegmentsIntersect(side, ray);
            }

            return intersections;
        }

        public string LabelSignalObjectsData(Point point, int screenHeight, int screenWidth)
        {
            if ((point.X > screenWidth) || (point.X < 0) || (point.Y > screenHeight) || (point.Y < 0)) return "Outside of screen";

            int intersections;

            foreach (RegionOfInterest regionOfInterest in RegionsOfInterest)
            {
                intersections = CountIntersections(point, regionOfInterest);

                if (intersections % 2 == 1) // odd so it's inside of the polygon
                {
                    return regionOfInterest.region;
                }
            }

            return "Not in any ROIs";
        }
    }
}
