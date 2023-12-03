using EyeTracker.Models;
using Microsoft.EntityFrameworkCore;
using System;
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

    public ROIConfig? Update(ROIConfig config)
    {
        var configForUpdate = _context.ROIConfigs
            .Include(x => x.ROIs)
                .ThenInclude(x => x.Points)
            .Where(x => x.Id == config.Id)
            .FirstOrDefault();
        if (configForUpdate != null)
        {
            _context.ROIPoints.RemoveRange(configForUpdate.ROIs.SelectMany(x => x.Points));
            _context.ROIs.RemoveRange(configForUpdate.ROIs);

            configForUpdate.Name = config.Name;
            configForUpdate.ROIs = config.ROIs;
            foreach (var roi in configForUpdate.ROIs)
            {
                var order = 1;
                foreach (var point in roi.Points)
                {
                    point.Order = order++;
                }
            }
            _context.ROIConfigs.Update(configForUpdate);
            _context.SaveChanges();
            _context.Entry(configForUpdate).State = EntityState.Detached;
            return config;
        }
        else
        {
            return null;
        }
    }
    public void Delete(Guid configId)
    {
        var configForUpdate = _context.ROIConfigs
            .Include(x => x.ROIs)
                .ThenInclude(x => x.Points)
            .Where(x => x.Id == configId)
            .FirstOrDefault();
        if (configForUpdate != null)
        {
            _context.ROIPoints.RemoveRange(configForUpdate.ROIs.SelectMany(x => x.Points));
            _context.ROIs.RemoveRange(configForUpdate.ROIs);
            _context.ROIConfigs.Remove(configForUpdate);
            _context.SaveChanges();
            _context.Entry(configForUpdate).State = EntityState.Detached;
        }
    }
}
