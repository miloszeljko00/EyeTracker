using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class Recording
    {
        public ROIConfig ROIConfig { get; set; } = new();
        public List<RecordingPoint> Points { get; set; } = new();
        public DateTime RecordingStart { get; set; }
        public DateTime RecordingEnd { get; set; }
    }
}
