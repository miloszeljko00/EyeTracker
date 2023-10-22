using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeTracker.Models;

public class Profile
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
