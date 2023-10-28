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

namespace EyeTracker.UserControls
{
    public partial class TextInputField : UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;

        public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(TextInputField), new PropertyMetadata(string.Empty));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set 
            { 
                SetValue(TextProperty, value); 
                OnPropertyChanged();
            }
        }
        public static readonly DependencyProperty PlaceholderProperty =
        DependencyProperty.Register("Placeholder", typeof(string), typeof(TextInputField), new PropertyMetadata(string.Empty));

        public string Placeholder
        {
            get { return (string)GetValue(TextProperty); }
            set 
            {
                SetValue(PlaceholderProperty, value);
                OnPropertyChanged();
            }
        }

        public TextInputField()
        {
            InitializeComponent();
            updateVisibility();
        }


        private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateVisibility();
        }

        private void txtClear_Click(object sender, RoutedEventArgs e)
        {
            txtInput.Clear();
            txtInput.Focus();
        }

        private void updateVisibility()
        {
            if (string.IsNullOrEmpty(txtInput.Text))
            {
                txtPlaceholder.Visibility = Visibility.Visible;
                txtClear.Visibility = Visibility.Hidden;
            }
            else
            {
                txtPlaceholder.Visibility = Visibility.Hidden;
                txtClear.Visibility = Visibility.Visible;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            txtInput.Focus();
        }
    }
}
