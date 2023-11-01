using Contracts;
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

namespace EyeTracker.Pages
{
    public partial class RecordingsPage : Page
    {
        private readonly ROIConfigService _roiConfigService;
        private readonly RecordingService _recordingService;

        private bool _selectionDisabled = false;

        private Models.ROIConfig _selectedConfig;
        public Models.ROIConfig SelectedConfig
        {
            get { return _selectedConfig; }
            set
            {
                _selectedConfig = value;
                OnPropertyChanged();
            }
        }
        private Profile _selectedProfile;
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
        public RecordingsPage(ProfileService profileService, ROIConfigService roiConfigService, RecordingService recordingService)
        {
            _roiConfigService = roiConfigService;
            _profileService = profileService;
            _recordingService = recordingService;
            _selectedConfig = LoadSelectedConfig();
            _selectedProfile = LoadSelectedProfile();
            DataContext = this;
            InitializeComponent();
            RecordingsDataGrid.ItemsSource = SelectedConfig.Recordings;
        }

        private Models.ROIConfig LoadSelectedConfig()
        {
            if (_roiConfigService.SelectedConfig == null)
            {
                NavigationService.GoBack();
            }
            return _roiConfigService.SelectedConfig ?? new Models.ROIConfig();
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
            if (RecordingsDataGrid.SelectedItem != null)
            {
                var recording = (Recording)RecordingsDataGrid.SelectedItem;
                MessageBox.Show(recording.Id + " playing recorded video...");

                RecordingsDataGrid.SelectedItem = null;
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
            RecordingsDataGrid.UnselectAll();
            RecordingsDataGrid.Items.Refresh();
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
            // ovde pocinje recording
            _recordingService.StartRecordingScreen();
            MessageBox.Show("Recording...", SelectedConfig.Name);
            _recordingService.StopRecordingScreen();

            _recordingService.DrawROI(SelectedConfig.ROIs);

            RefreshTable();
            // ovde se zavrsava
            // TODO: Save recording
        }
    }
}
