using EyeTracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace EyeTracker.Services;

public class ROIConfigService
{
    private readonly ApplicationDbContext _context;

    public ROIConfig? SelectedConfig { get; set; }

    public ROIConfigService(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<ROIConfig> GetROIConfigs()
    {
       var roiConfigs = _context.ROIConfigs
            .Include(x=>x.ROIs)
                .ThenInclude(x=>x.Points)
            .AsNoTracking()
        .ToList();

        roiConfigs
            .ForEach(config => config.ROIs
                .ForEach(roi => roi.Points = roi.Points.OrderBy(point => point.Order)
            .ToList()));

        return roiConfigs;        
    }
    public ROIConfig Create(ROIConfig config)
    {
        foreach (var roi in config.ROIs)
        {
            var order = 1;
            foreach (var point in roi.Points)
            {
                point.Order = order++;
            }
        }
        _context.ROIConfigs.Add(config);
        _context.SaveChanges();
        _context.Entry(config).State = EntityState.Detached;
        return config;
    }
}
