﻿using Contracts;
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
using YamlDotNet.Core;

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

        public ObservableCollection<Models.Recording> Recordings;
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
            RecordingsDataGrid.ItemsSource = new ObservableCollection<Models.Recording>(_recordingService.GetAllForConfig(SelectedConfig.Id));
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

        private void RecordingsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectionDisabled) return;
            if (RecordingsDataGrid.SelectedItem != null)
            {
                var recording = (Models.Recording)RecordingsDataGrid.SelectedItem;
                _recordingService.SelectedRecording = recording;
                RecordingsDataGrid.UnselectAll();
                var app = (App)Application.Current;

                var window = app.ServiceProvider.GetService<VideoPlayerWindow>();
                if (window == null) return;

                window.Owner = app.MainWindow;
                window.Owner.Opacity = 0.5;
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                window.ShowDialog();
                window.Owner.Opacity = 1;

                RefreshTable();
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

        private void Statistics_Button_Click(object sender, RoutedEventArgs e)
        {
            var recording = (Models.Recording)RecordingsDataGrid.SelectedItem;
            _recordingService.SelectedRecording = recording;
            RecordingsDataGrid.UnselectAll();
            var app = (App)Application.Current;
            NavigationService.Navigate(app.ServiceProvider.GetService<RecordingStatisticsPage>());
            RefreshTable();
        }
        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Models.Recording rec)
            {
                var result = MessageBox.Show("Confirm delete?", "Delete recording", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                if (result == MessageBoxResult.Yes)
                {
                    var recording = (Models.Recording)RecordingsDataGrid.SelectedItem;
                    if (recording == null) return;
                    _recordingService.DeleteRecordingById(recording.Id);
                    RecordingsDataGrid.ItemsSource = new ObservableCollection<Models.Recording>(_recordingService.GetAllForConfig(SelectedConfig.Id));
                }
            }
            RefreshTable();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // ovde pocinje recording
            _recordingService.StartRecordingScreen(SelectedProfile, SelectedConfig);
            MessageBox.Show("Recording...", SelectedConfig.Name);
            var recording = _recordingService.StopRecordingScreen();

            if (recording == null) return;
            _recordingService.DrawRecording(recording);

            RecordingsDataGrid.ItemsSource = new ObservableCollection<Models.Recording>(_recordingService.GetAllForConfig(SelectedConfig.Id));
            RefreshTable();
        }
    }
}
