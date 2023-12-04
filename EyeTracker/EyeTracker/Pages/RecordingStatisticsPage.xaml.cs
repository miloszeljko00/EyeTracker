using EyeTracker.Models;
using EyeTracker.Services;
using Postprocessing.Interfaces;
using Postprocessing.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
    /// Interaction logic for RecordingStatistics.xaml
    /// </summary>
    public partial class RecordingStatisticsPage : Page, INotifyPropertyChanged
    {
        private readonly IPythonScript _pythonScript;
        private readonly RecordingService _recordingService;
        private readonly string _recordingDataPath;
        private BitmapImage? _imageSource;

        public BitmapImage? ImageSource
        {
            get { return _imageSource; }
            set
            {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }
        public Recording Recording;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public RecordingStatisticsPage(IPythonScript pythonScript, RecordingService recordingService)
        {
            _recordingService= recordingService;
            _pythonScript = pythonScript;
            if (recordingService.SelectedRecording == null) {
                NavigationService.GoBack();
                Recording = new Recording();
            }
            else
            {
                Recording = recordingService.SelectedRecording;
            }
            DataContext = this;
            InitializeComponent();
            string workingDirectory = Directory.GetCurrentDirectory();
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

            _recordingDataPath = System.IO.Path.Join(projectDirectory, "..", "GazepointClient", "recording_data", Recording.Id.ToString() + ".csv");

            
            
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private async void RegionsPiechart_Click(object sender, RoutedEventArgs e)
        {
            // Show spinner
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                // Get the result path after the script execution
                var resultPath = _pythonScript.GetResultsPath(Recording.Id.ToString() + "region_piechart");

                // Update the UI with the result image
                if (resultPath != null)
                {
                    ImageSource = new BitmapImage(new Uri(resultPath));
                }

                // Hide the spinner
                Mouse.OverrideCursor = null;

            }
            catch (Exception ex)
            {
                // Execute the script asynchronously
                await Task.Run(() => _pythonScript.CallScript("region_piechart.py", _recordingDataPath, Recording.Id.ToString() + "region_piechart"));


                // Update UI on the UI thread
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    // Hide the spinner
                    Mouse.OverrideCursor = null;

                    // Get the result path after the script execution
                    var resultPath = _pythonScript.GetResultsPath(Recording.Id.ToString() + "region_piechart");

                    // Update the UI with the result image
                    if (resultPath != null)
                    {
                        ImageSource = new BitmapImage(new Uri(resultPath));
                    }
                }));
            }
        }

        private async void TimePerRegion_Click(object sender, RoutedEventArgs e)
        {
            // Show spinner
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                var resultPath = _pythonScript.GetResultsPath(Recording.Id.ToString() + "time_per_region");

                // Update the UI with the result image
                if (resultPath != null)
                {
                    ImageSource = new BitmapImage(new Uri(resultPath));
                }

                // Hide the spinner
                Mouse.OverrideCursor = null;
            }
            catch(Exception ex)
            {
                // Execute the script asynchronously
                await Task.Run(() => _pythonScript.CallScript("time_per_region.py", _recordingDataPath, Recording.Id.ToString() + "time_per_region"));


                // Update UI on the UI thread
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    // Hide the spinner
                    Mouse.OverrideCursor = null;

                    // Get the result path after the script execution
                    var resultPath = _pythonScript.GetResultsPath(Recording.Id.ToString() + "time_per_region");

                    // Update the UI with the result image
                    if (resultPath != null)
                    {
                        ImageSource = new BitmapImage(new Uri(resultPath));
                    }
                }));
            }
        }

        private async void CoordinateClustering_Click(object sender, RoutedEventArgs e)
        {
            // Show spinner
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                var resultPath = _pythonScript.GetResultsPath(Recording.Id.ToString() + "coordinate_clustering");
                
                if (resultPath != null)
                {
                    ImageSource = new BitmapImage(new Uri(resultPath));
                }

                // Hide the spinner
                Mouse.OverrideCursor = null;
            }
            catch(Exception ex)
            {
                // Execute the script asynchronously
                await Task.Run(() => _pythonScript.CallScript("coordinate_clustering.py", _recordingDataPath, Recording.Id.ToString() + "coordinate_clustering"));

                // Update UI on the UI thread
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    // Hide the spinner
                    Mouse.OverrideCursor = null;

                    // Get the result path after the script execution
                    var resultPath = _pythonScript.GetResultsPath(Recording.Id.ToString() + "coordinate_clustering");

                    // Update the UI with the result image
                    if (resultPath != null)
                    {
                        ImageSource = new BitmapImage(new Uri(resultPath));
                    }
                }));
            }
        }
    }
}
