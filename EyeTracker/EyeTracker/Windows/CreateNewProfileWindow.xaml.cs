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

public partial class CreateNewProfileWindow : Window, INotifyPropertyChanged
{
    private readonly ProfileService _profileService;

    private Profile _profile;
    public Profile Profile
    {
        get { return _profile; }
        set 
        {
            _profile = value;
            OnPropertyChanged();
        }
    }
    public CreateNewProfileWindow(ProfileService profileService)
    {
        _profileService = profileService;
        _profile = new()
        {
            Id = Guid.NewGuid(),
            Name = "",
        };
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
    

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        if (Profile.Name == "")
        {
            MessageBox.Show("Name can't be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        
        _profileService.Create(Profile);
        Close();
    }
}
