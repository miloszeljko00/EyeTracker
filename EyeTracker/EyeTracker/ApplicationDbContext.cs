using EyeTracker.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace EyeTracker;

public class ApplicationDbContext: DbContext
{
    #region Public properties
    public DbSet<EyeTrackerConfig> EyeTrackerConfigs { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<ROIConfig> ROIConfigs { get; set; }
    #endregion

    #region Contructor
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    #endregion
    #region Overridden method
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EyeTrackerConfig>().HasData(GetEyeTrackerConfigs());
        modelBuilder.Entity<Profile>().HasData(GetProfiles());
        modelBuilder.Entity<ROIConfig>().HasData(GetROIConfigs());
        base.OnModelCreating(modelBuilder);
    }
    #endregion
    #region Private method
    private List<EyeTrackerConfig> GetEyeTrackerConfigs()
    {
        return new List<EyeTrackerConfig>
        {
            new EyeTrackerConfig {
                Id = Guid.Parse("f3bb9e65-ddce-491b-b021-869a1c0660b3"),
                Address ="172.19.48.1",
                Port = "4242"
            },
        };
    }
    private List<Profile> GetProfiles()
    {
        return new List<Profile>
        {
            new Profile {
                Id = Guid.Parse("f3bb9e65-ddce-491b-b021-869a1c0660b4"),
                Name = "Miloš Zeljko"
            },
            new Profile {
                Id = Guid.Parse("f3bb9e65-ddce-491b-b021-869a1c0660b5"),
                Name = "Jovana Santovac"
            },
        };
    }
    private List<ROIConfig> GetROIConfigs()
    {
        return new List<ROIConfig>
        {
            new ROIConfig {
                Id = Guid.Parse("f3bb9e65-ddce-491b-b021-869a1c0660b7"),
                Name = "Config 1"
            },
            new ROIConfig {
                Id = Guid.Parse("f3bb9e65-ddce-491b-b021-869a1c0660b8"),
                Name = "Config 2"
            },
        };
    }
    #endregion
}
