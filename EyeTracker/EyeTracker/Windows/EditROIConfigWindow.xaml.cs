using EyeTracker.Models;
using EyeTracker.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EyeTracker.Windows;

/// <summary>
/// Interaction logic for CreateROIConfigWindow.xaml
/// </summary>
public partial class EditROIConfigWindow : Window, INotifyPropertyChanged
{
    private readonly ROIConfigService _roiConfigService;
    private int _numberOfROIs;

    public int NumberOfROIs
    {
        get { return _numberOfROIs; }
        set 
        { 
            _numberOfROIs = value;
            OnPropertyChanged();
        }
    }

    private ROIConfig _roiConfig;
    public ROIConfig ROIConfig
    {
        get { return _roiConfig; }
        set 
        { 
            _roiConfig = value;
            OnPropertyChanged();
        }
    }
    public EditROIConfigWindow(ROIConfigService roiConfigService)
    {
        _roiConfigService = roiConfigService;
        var config = _roiConfigService.SelectedConfig;
        if (config == null)
        {
            this.Close();
            return;
        }
        _roiConfig = config;
        _numberOfROIs = _roiConfig.ROIs.Count;
        DataContext = this;
        InitializeComponent();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }
    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        try
        {
            DragMove();
        }
        catch { }
    }

    private void btnMinimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void btnRestore_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Normal;
        btnMaximize.Visibility = Visibility.Visible;
        btnRestore.Visibility = Visibility.Collapsed;
    }

    private void btnMaximize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Maximized;
        btnMaximize.Visibility = Visibility.Collapsed;
        btnRestore.Visibility = Visibility.Visible;
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var app = (App)Application.Current;
        var window = app.ServiceProvider.GetService<TransparentOverlayWindow>();
        if (window == null) return;
        window.Owner = this;
        window.Owner.Opacity = 0.5;
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

        window.ROIs = new(ROIConfig.ROIs);

        window.Show();

        window.Owner.Opacity = 1;
        Focus();
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        if (ROIConfig.Name == "")
        {
            MessageBox.Show("Name can't be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (ROIConfig.ROIs.Count == 0)
        {
            MessageBox.Show("Atleast 1 ROI must be marked.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        _roiConfigService.Update(ROIConfig);
        Close();
    }
}
