using EyeTracker.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeTracker.Services;

public class ROIConfigService
{
    private readonly ApplicationDbContext _context;

    public ROIConfigService(ApplicationDbContext context)
    {
        _context = context;
    }
    public List<ROIConfig> GetROIConfigs()
    {
       return _context.ROIConfigs.AsNoTracking().ToList();
    }
    public ROIConfig Create(ROIConfig config)
    {
        _context.ROIConfigs.Add(config);
        _context.SaveChanges();
        _context.Entry(config).State = EntityState.Detached;
        return config;
    }
}
