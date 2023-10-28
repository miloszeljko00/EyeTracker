﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeTracker.Models
{
    public class Recording
    {
        [Key]
        public Guid Id { get; set; }
        public Profile Profile { get; set; } = new();
        public ROIConfig ROIConfig { get; set; } = new();
        public string VideoUrl = string.Empty;
        public List<RecordingPoint> Points = new();
        public DateTime RecordingStart { get; set; }
        public DateTime RecordingEnd { get; set;}
    }
}
