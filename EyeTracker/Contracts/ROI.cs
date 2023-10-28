using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts;

public class ROI
{
    public string Id { get; set; } = string.Empty;
    public List<ROIPoint> Points { get; set; } = new();
}
