﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeTracker.Models;

public class ROIConfig
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ROI> ROIs { get; set; } = new();
    public List<Recording> Recordings { get; set; } = new();
}
