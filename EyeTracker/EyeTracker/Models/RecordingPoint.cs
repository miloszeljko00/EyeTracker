using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeTracker.Models
{
    public class RecordingPoint
    {
        [Key]
        public Guid Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
