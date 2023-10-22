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

        public void LabelSignalObjectsData(ref Dictionary<string, List<object>> signalObjectsData, Tuple<int> screenSize)
        {
            if (!signalObjectsData.ContainsKey("ENABLE_SEND_POG_BEST"))
            {
                throw new Exception("ENABLE_SEND_POG_BEST not in configuration. Can't label data without (x, y) coordinates");
            }


        }
    }
}
