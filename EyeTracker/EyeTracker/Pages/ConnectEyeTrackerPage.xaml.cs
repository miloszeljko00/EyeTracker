using EyeTracker.Models;
using EyeTracker.Services;
using Microsoft.EntityFrameworkCore;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EyeTracker.Pages
{
    /// <summary>
    /// Interaction logic for ConnectEyeTrackerPage.xaml
    /// </summary>
    public partial class ConnectEyeTrackerPage : Page, INotifyPropertyChanged
    {
        private readonly EyeTrackerConfigService _eyeTrackerConfigService;
        public ConnectEyeTrackerPage(EyeTrackerConfigService eyeTrackerConfigService)
        {
            _eyeTrackerConfigService = eyeTrackerConfigService;
            _eyeTrackerConfig = _eyeTrackerConfigService.EyeTrackerConfig;
            DataContext = this;
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private EyeTrackerConfig _eyeTrackerConfig;

        public EyeTrackerConfig EyeTrackerConfig
        {
            get { return _eyeTrackerConfig; }
            set
            {
                _eyeTrackerConfig = value;
                OnPropertyChanged();
            }
        }
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var result = _eyeTrackerConfigService.Connect(EyeTrackerConfig);
            if (!result)
            {
                MessageBox.Show(
                    "Can't connect to EyeTracker!",
                    "Connection Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK
                );
                return;
            }
            var app = (App)App.Current;
            NavigationService.Navigate(app.ServiceProvider.GetService<ProfilesPage>());
        }
    }
}
