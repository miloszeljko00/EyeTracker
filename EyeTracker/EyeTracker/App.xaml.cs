using EyeTracker.Pages;
using EyeTracker.Services;
using EyeTracker.Windows;
using GazepointClient.Interfaces;
using GazepointClient.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using ScreenRecorder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EyeTracker;

public partial class App : Application
{
    public readonly ServiceProvider ServiceProvider;

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();
    }

    private static void ConfigureServices(ServiceCollection services)
    {
        string databaseFilePath = "EyeTracker.db";

        // Delete the existing database file if it exists
        //if (File.Exists(databaseFilePath))
        //{
        //    File.Delete(databaseFilePath);
        //}

        //Contexts
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite($"Data Source={databaseFilePath}");
        });

        // Windows
        services.AddSingleton<MainWindow>();
        services.AddTransient<CreateROIConfigWindow>();
        services.AddTransient<TransparentOverlayWindow>();
        services.AddTransient<VideoPlayerWindow>();

        //Pages
        services.AddTransient<ConnectEyeTrackerPage>();
        services.AddTransient<ProfilesPage>();
        services.AddTransient<ROIConfigsPage>();
        services.AddTransient<RecordingsPage>();

        //Services
        services.AddSingleton<EyeTrackerConfigService>();
        services.AddSingleton<ProfileService>();
        services.AddSingleton<ROIConfigService>();
        services.AddSingleton<GPClient>();
        services.AddSingleton<RecordingService>();
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        var mainWindow = ServiceProvider.GetService<MainWindow>();
        if (mainWindow == null) return;
        mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        mainWindow?.Show();
    }
}