using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class RecordingPoint
    {
        public int Order { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public long Timestamp { get; set; }
        public string Label { get; set; } = string.Empty;
    }
}
