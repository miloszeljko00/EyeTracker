﻿using System;
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
        public int Order { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Timestamp { get; set; }
        public string Label { get; set; } = string.Empty;
    }
}
