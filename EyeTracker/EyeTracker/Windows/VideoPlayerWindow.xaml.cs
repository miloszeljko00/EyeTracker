using EyeTracker.Models;
using EyeTracker.Services;
using LibVLCSharp.Shared;
using LibVLCSharp.WPF;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace EyeTracker.Windows;

public partial class VideoPlayerWindow : Window
{
    private readonly RecordingService _recordingService;
    public Recording? SelectedRecording;
    LibVLC _libVLC;
    MediaPlayer _mediaPlayer;

    public VideoPlayerWindow(RecordingService recordingService)
    {
        _recordingService = recordingService;
        SelectedRecording = _recordingService.SelectedRecording;
        if(SelectedRecording == null)
        {
            Close();
            return;
        }
        DataContext = this;
        InitializeComponent();
        // aviMediaElement.Source = new Uri(SelectedRecording.VideoUrl, UriKind.RelativeOrAbsolute);

        videoPlayer.Loaded += VideoPlayer_Loaded;

    }
    void VideoPlayer_Loaded(object sender, RoutedEventArgs e)
    {
        Core.Initialize();

        _libVLC = new LibVLC();
        _mediaPlayer = new MediaPlayer(_libVLC);

        videoPlayer.MediaPlayer = _mediaPlayer;
        string currentDirectory = Directory.GetCurrentDirectory();
        _mediaPlayer.Play(new Media(_libVLC, new Uri(currentDirectory + Path.DirectorySeparatorChar + SelectedRecording.VideoUrl)));
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
}
