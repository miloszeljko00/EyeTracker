using EyeTracker.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace EyeTracker.Services;

public class EyeTrackerConfigService
{
    public EyeTrackerConfig EyeTrackerConfig { get; set; }

    private readonly ApplicationDbContext _context;

    public EyeTrackerConfigService(ApplicationDbContext context)
    {
        _context = context;
        EyeTrackerConfig = context.EyeTrackerConfigs.AsNoTracking().First();
    }

    public bool Connect(EyeTrackerConfig eyeTrackerConfig)
    {
        // Validate connection
        eyeTrackerConfig = _context.EyeTrackerConfigs.Where(x => x.Id == eyeTrackerConfig.Id).First();
        if (eyeTrackerConfig == null) return false;

        eyeTrackerConfig.Address = EyeTrackerConfig.Address;
        eyeTrackerConfig.Port = EyeTrackerConfig.Port;
        _context.SaveChanges();
        _context.Entry(eyeTrackerConfig).State = EntityState.Detached;
        return true;
    }
}
