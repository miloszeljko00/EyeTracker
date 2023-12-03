using EyeTracker.Models;
using EyeTracker.Services;
using EyeTracker.Windows;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for UsersPage.xaml
    /// </summary>
    public partial class ProfilesPage : Page
    {
        private readonly ProfileService _profileService;
        private readonly EyeTrackerConfigService _eyeTrackerConfigService;
        private bool _selectionDisabled = false;
        public EyeTrackerConfig EyeTrackerConfig { get; set; }
        public ProfilesPage(ProfileService profileService, EyeTrackerConfigService eyeTrackerConfigService)
        {
            _profileService = profileService;
            _eyeTrackerConfigService = eyeTrackerConfigService;
            EyeTrackerConfig = _eyeTrackerConfigService.EyeTrackerConfig;
            DataContext = this;
            InitializeComponent();
            profileDataGrid.ItemsSource = _profileService.GetProfiles();
        }


        private void profileDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectionDisabled) return;
            if (profileDataGrid.SelectedItem == null) return;

            var profile = (Profile)profileDataGrid.SelectedItem;
            _profileService.SelectedProfile = profile;
            profileDataGrid.SelectedItem = null;

            var app = (App)Application.Current;
            NavigationService.Navigate(app.ServiceProvider.GetService<ROIConfigsPage>());
            RefreshTable();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (profileDataGrid.SelectedItem == null) return;

            var profile = (Profile)profileDataGrid.SelectedItem;
            _profileService.SelectedProfile = profile;
            profileDataGrid.SelectedItem = null;

            var app = (App)Application.Current;
            var window = app.ServiceProvider.GetService<EditProfileWindow>();
            if (window == null) return;
            window.Owner = app.MainWindow;
            window.Owner.Opacity = 0.5;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
            window.Owner.Opacity = 1;
            if (window.Profile.Name == "") return;
            profileDataGrid.ItemsSource = _profileService.GetProfiles();
        }
        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Profile p)
            {
                var result = MessageBox.Show("Confirm delete?", "Delete profile", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                if (result == MessageBoxResult.Yes)
                {
                    var profile = (Models.Profile)profileDataGrid.SelectedItem;
                    if (profile == null) return;
                    _profileService.Delete(profile.Id);
                    profileDataGrid.ItemsSource = _profileService.GetProfiles();
                }
            }
            RefreshTable();
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
            profileDataGrid.UnselectAll();
            profileDataGrid.Items.Refresh();
        }

        private void New_Profile_Button_Click(object sender, RoutedEventArgs e)
        {
            var app = (App)Application.Current;
            var window = app.ServiceProvider.GetService<CreateNewProfileWindow>();
            if (window == null) return;
            window.Owner = app.MainWindow;
            window.Owner.Opacity = 0.5;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
            window.Owner.Opacity = 1;
            if (window.Profile.Name == "") return;
            profileDataGrid.ItemsSource = _profileService.GetProfiles();
        }
    }
}
