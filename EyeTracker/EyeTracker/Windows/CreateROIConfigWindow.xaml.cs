using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace EyeTracker.Windows
{
    /// <summary>
    /// Interaction logic for CreateROIConfigWindow.xaml
    /// </summary>
    public partial class CreateROIConfigWindow : Window
    {
        public CreateROIConfigWindow()
        {
            InitializeComponent();
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
            DragMove();
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
            window.Owner = app.MainWindow;
            window.Owner.Opacity = 0.5;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
            window.Owner.Opacity = 1;
        }
    }
}
