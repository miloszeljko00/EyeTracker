using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeTracker.Models;

[Serializable]
public class EyeTrackerConfig
{
    [Key]
    public Guid Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
}
