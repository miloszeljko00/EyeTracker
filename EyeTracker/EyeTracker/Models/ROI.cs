using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EyeTracker.Models;

public class ROI
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ROIPoint> Points { get; set; } = new();
}
