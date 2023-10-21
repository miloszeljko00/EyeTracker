using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace EyeTracker.Windows;


public partial class TransparentOverlayWindow : Window
{
    public TransparentOverlayWindow()
    {
        InitializeComponent();
    }
    private Point? lastMousePosition = null; // To track the last mouse position
    private Point? firstMousePosition = null; // To track the position of the first dot

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            Point mousePosition = e.GetPosition(canvas);

            // Create a new Ellipse (dot) and set its properties.
            Ellipse newDot = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Red
            };

            // Set the dot's position to the mouse click and add it to the canvas.
            Canvas.SetLeft(newDot, mousePosition.X - (newDot.Width / 2));
            Canvas.SetTop(newDot, mousePosition.Y - (newDot.Height / 2));

            canvas.Children.Add(newDot);

            if (lastMousePosition.HasValue)
            {
                // Create a Line element to connect the last two dots
                Line line = new Line
                {
                    X1 = lastMousePosition.Value.X,
                    Y1 = lastMousePosition.Value.Y,
                    X2 = mousePosition.X,
                    Y2 = mousePosition.Y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                canvas.Children.Add(line);
            }

            if (!firstMousePosition.HasValue)
            {
                // Store the position of the first dot
                firstMousePosition = mousePosition;
            }

            lastMousePosition = mousePosition;
        }
        else if (e.ChangedButton == MouseButton.Right)
        {
            // Handle right-click to remove the last dot and line
            if (canvas.Children.Count >= 3)
            {
                // Update lastMousePosition to the position of the last dot before removal
                var index = canvas.Children.Count - 4;
                if (index < 0) index = 0;
                lastMousePosition = new Point(Canvas.GetLeft(canvas.Children[index]) + 5, Canvas.GetTop(canvas.Children[index]) + 5);

                // Remove the last drawn line
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
                // Remove the last drawn dot
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
            }
            else
            {
                // If there's only one dot, remove it and reset both positions
                canvas.Children.Clear();
                firstMousePosition = null;
                lastMousePosition = null;
            }
        }
    }


    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (firstMousePosition.HasValue && lastMousePosition.HasValue)
            {
                // Create a Line element to connect the first and last dots
                Line line = new Line
                {
                    X1 = firstMousePosition.Value.X,
                    Y1 = firstMousePosition.Value.Y,
                    X2 = lastMousePosition.Value.X,
                    Y2 = lastMousePosition.Value.Y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                canvas.Children.Add(line);
            }
        }
    }

}

