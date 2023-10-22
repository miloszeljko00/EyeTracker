using EyeTracker.Models;
using EyeTracker.Services;
using EyeTracker.Windows;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EyeTracker.Pages;


public partial class ROIConfigsPage : Page, INotifyPropertyChanged
{
    private readonly ROIConfigService _roiConfigService;
    public ObservableCollection<ROIConfig> ROIConfigs {  get; set; }

    private Profile _selectedProfile;
    private bool _selectionDisabled = false;
    public Profile SelectedProfile
    {
        get { return _selectedProfile; }
        set
        {
            _selectedProfile = value;
            OnPropertyChanged();
        }
    }
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private ProfileService _profileService;
    public ROIConfigsPage(ProfileService profileService, ROIConfigService roiConfigService)
    {
        _roiConfigService = roiConfigService;
        _profileService = profileService;
        _selectedProfile = LoadSelectedProfile();
        ROIConfigs = new(_roiConfigService.GetROIConfigs());
        DataContext = this;
        InitializeComponent();
        ROIConfigsDataGrid.ItemsSource = ROIConfigs;
    }

    private Profile LoadSelectedProfile()
    {
        if (_profileService.SelectedProfile == null)
        {
            NavigationService.GoBack();
        }
        return _profileService.SelectedProfile ?? new Profile();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void ROIConfigsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_selectionDisabled) return;
        if (ROIConfigsDataGrid.SelectedItem != null)
        {
            var roiConfig = (ROIConfig)ROIConfigsDataGrid.SelectedItem;
            MessageBox.Show(roiConfig.Name);

            ROIConfigsDataGrid.SelectedItem = null;
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.GoBack();
    }

    private void NameColumn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _selectionDisabled = false;
    }
    private void EditColumn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _selectionDisabled = true;
    }
    private void DeleteColumn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _selectionDisabled = true;
    }
    private void RefreshTable()
    {
        ROIConfigsDataGrid.UnselectAll();
        ROIConfigsDataGrid.Items.Refresh();
    }

    private void Edit_Button_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("EDIT");
        RefreshTable();
    }
    private void Delete_Button_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("DELETE");
        RefreshTable();
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        var app = (App)Application.Current;
        var window = app.ServiceProvider.GetService<CreateROIConfigWindow>();
        if (window == null) return;
        window.Owner = app.MainWindow;
        window.Owner.Opacity = 0.5;
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.ShowDialog();
        window.Owner.Opacity = 1;
        if (window.ROIConfig.Name == "") return;
        if (window.ROIConfig.ROIs.Count == 0) return;
        ROIConfigs.Add(window.ROIConfig);
    }
}
