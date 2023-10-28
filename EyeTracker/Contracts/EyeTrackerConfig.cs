using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts;

public class EyeTrackerConfig
{
    public string Address { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public int ScreenWidth { get; set; }
    public int ScreenHeight { get; set; }
}
