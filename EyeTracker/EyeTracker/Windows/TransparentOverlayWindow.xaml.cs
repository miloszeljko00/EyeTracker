using EyeTracker.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace EyeTracker.Windows;


public partial class TransparentOverlayWindow : Window
{

    private List<WindowState> _windowStates = new List<WindowState>();
    private List<Window> _minimizedWindows = new List<Window>();

    private List<ROI> _rois;
    public List<ROI> ROIs
    {
        get { return _rois; } 
        set
        {
            _rois = value;
            _rois.ForEach(roi =>
            {
                DrawROI(roi);
            });
        }
    }
    private List<ROIPoint> _roiPoints = new();
    private ROIPoint? _lastROIPoint = null;
    private ROIPoint? _firstROIPoint = null;
    private bool isDragging = false;
    private DateTime mouseDown;
    readonly TimeSpan interval = TimeSpan.FromMilliseconds(150);
    private Point dragStartPoint;
    private Rectangle? selectionRectangle;
    private Ellipse[]? cornerDots;
    private Line? selectionLine;
    public TransparentOverlayWindow()
    {
        _rois = new();
        InitializeComponent();
        MinimizeOtherWindows();
    }
    private void DrawROI(ROI roi)
    {
        for (int i = 0; i < roi.Points.Count; i ++)
        {
            DrawDot(roi.Points[i].X, roi.Points[i].Y);
            if (i + 1 >= roi.Points.Count)
            {
                DrawLine(roi.Points[i].X, roi.Points[i].Y, roi.Points[0].X, roi.Points[0].Y);
            }
            else
            {
                DrawLine(roi.Points[i].X, roi.Points[i].Y, roi.Points[i+1].X, roi.Points[i+1].Y);
            }
        }
        DrawROIName(roi);
    }
    private void DrawROIName(ROI roi)
    {
        TextBlock textBlock = new TextBlock
        {
            Text = roi.Name,
            FontSize = 18,
            Foreground = Brushes.White,
            Background = Brushes.Black,
            Padding = new Thickness(5),
        };
        var middlePoint = FindMiddlePoint(roi.Points);
        Canvas.SetLeft(textBlock, middlePoint.X - textBlock.ActualWidth / 2);
        Canvas.SetTop(textBlock, middlePoint.Y - textBlock.ActualHeight / 2);
        textBlock.SizeChanged += (s, e) =>
        {
            Canvas.SetLeft(textBlock, middlePoint.X - textBlock.ActualWidth / 2);
            Canvas.SetTop(textBlock, middlePoint.Y - textBlock.ActualHeight / 2);
        };
        canvas.Children.Add(textBlock);
    }
    private void DrawLine(double x1, double y1, double x2, double y2)
    {
        Line line = new Line
        {
            X1 = x1,
            Y1 = y1,
            X2 = x2,
            Y2 = y2,
            Stroke = Brushes.Red,
            StrokeThickness = 1
        };

        canvas.Children.Add(line);
    }
    private void DrawDot(double x, double y)
    {
        var newDot = new Ellipse
        {
            Width = 10,
            Height = 10,
            Fill = Brushes.Red
        };
        Canvas.SetLeft(newDot, x - (newDot.Width / 2));
        Canvas.SetTop(newDot, y - (newDot.Height / 2));
        canvas.Children.Add(newDot);
    }
    private void MinimizeOtherWindows()
    {
        // Minimize all other windows
        foreach (Window window in Application.Current.Windows)
        {
            if (window != this && window.WindowState != WindowState.Minimized)
            {
                _windowStates.Add(window.WindowState);
                _minimizedWindows.Add(window);
                window.WindowState = WindowState.Minimized;
            }
        }
    }
    private void CloseWindow()
    {
        // Restore minimized windows
        for (int i = 0; i < _minimizedWindows.Count; i++)
        {
            _minimizedWindows[i].WindowState = _windowStates[i];
        }
        Close();
    }
    private void RemoveLastROIPoint()
    {
        _lastROIPoint = _roiPoints.LastOrDefault();

        if (_lastROIPoint == null)
        {
            // nema tacaka za brisanje
            _firstROIPoint = null;
            _lastROIPoint = null;
        }
        else
        {
            if (selectionLine != null)
            {
                if (canvas.Children.Contains(selectionLine)) canvas.Children.Remove(selectionLine);
            }
            _roiPoints.Remove(_lastROIPoint);
            canvas.Children.RemoveAt(canvas.Children.Count - 1);

            // Remove line
            if (_roiPoints.Count > 0)
            {
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
                _lastROIPoint = _roiPoints.Last();

                selectionLine = new Line
                {
                    X1 = _lastROIPoint.X,
                    Y1 = _lastROIPoint.Y,
                    X2 = _lastROIPoint.X,
                    Y2 = _lastROIPoint.Y,
                    Stroke = Brushes.Red,
                    StrokeDashArray = new DoubleCollection { 2 },
                    StrokeThickness = 1
                };
                canvas.Children.Add(selectionLine);
            }
            else
            {
                _firstROIPoint = null;
                _lastROIPoint = null;
            }
        }
    }
    private void PlaceROIPoint(MouseButtonEventArgs e)
    {
        var newROIPoint = new ROIPoint()
        {
            Id = Guid.NewGuid(),
            X = e.GetPosition(canvas).X,
            Y = e.GetPosition(canvas).Y,
        };
        _roiPoints.Add(newROIPoint);
        // Create a new Ellipse (dot) and set its properties.
        Ellipse newDot = new Ellipse
        {
            Width = 10,
            Height = 10,
            Fill = Brushes.Red
        };

        // Set the dot's position to the mouse click and add it to the canvas.
        Canvas.SetLeft(newDot, newROIPoint.X - (newDot.Width / 2));
        Canvas.SetTop(newDot, newROIPoint.Y - (newDot.Height / 2));

        canvas.Children.Add(newDot);

        if (_lastROIPoint != null)
        {
            // Create a Line element to connect the last two dots
            Line line = new Line
            {
                X1 = _lastROIPoint.X,
                Y1 = _lastROIPoint.Y,
                X2 = newROIPoint.X,
                Y2 = newROIPoint.Y,
                Stroke = Brushes.Red,
                StrokeThickness = 1
            };

            canvas.Children.Add(line);
        }

        if (_firstROIPoint == null)
        {
            // Store the position of the first dot
            _firstROIPoint = newROIPoint;
        }
        if(selectionLine != null)
        {
            if(canvas.Children.Contains(selectionLine)) canvas.Children.Remove(selectionLine);
        }
        
        selectionLine = new Line
        {
            X1 = newROIPoint.X,
            Y1 = newROIPoint.Y,
            X2 = newROIPoint.X,
            Y2 = newROIPoint.Y,
            Stroke = Brushes.Red,
            StrokeDashArray = new DoubleCollection { 2 },
            StrokeThickness = 1
        };
        canvas.Children.Add(selectionLine);
        _lastROIPoint = newROIPoint;
    }
    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape) CloseWindow();
        else if (e.Key == Key.Space) FinishROI();
        else if (e.Key == Key.Back) RemoveLastROI();
        else if (e.Key == Key.Enter) FinishEnteringROIs();
    }
    private void FinishEnteringROIs()
    {
        if (_firstROIPoint != null) return;
        if (_lastROIPoint != null) return;
        if (_roiPoints.Count > 0) return;
        if (ROIs.Count == 0) return;

        var parent = (CreateROIConfigWindow)this.Owner;
        parent.ROIConfig.ROIs = ROIs;
        parent.NumberOfROIs = parent.ROIConfig.ROIs.Count;
        CloseWindow();
    }
    private void RemoveLastROI()
    {
        if (_roiPoints.Count > 0)
        {
            if (selectionLine != null)
            {
                if (canvas.Children.Contains(selectionLine)) canvas.Children.Remove(selectionLine);
            }
            while (_roiPoints.Count > 0)
            {
                _roiPoints.Remove(_roiPoints.Last());
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
                if (_roiPoints.Count > 0)
                {
                    canvas.Children.RemoveAt(canvas.Children.Count - 1);
                }
            }
        }
        else
        {
            if (ROIs.Count > 0)
            {
                var roi = ROIs.Last();
                ROIs.Remove(roi);
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
                var numberOfPoints = roi.Points.Count;
                for (int i = 0; i < numberOfPoints; i++)
                {
                    canvas.Children.RemoveAt(canvas.Children.Count - 1);
                    canvas.Children.RemoveAt(canvas.Children.Count - 1);
                }
            }
        }
        _firstROIPoint = null;
        _lastROIPoint = null;
    }
    private ROIPoint FindMiddlePoint(List<ROIPoint> roiPoints)
    {
        if (roiPoints.Count == 0)
        {
            throw new ArgumentException("List of dots is empty.");
        }

        double totalX = 0;
        double totalY = 0;

        foreach (var dot in roiPoints)
        {
            totalX += dot.X;
            totalY += dot.Y;
        }

        double centerX = totalX / roiPoints.Count;
        double centerY = totalY / roiPoints.Count;

        return new ROIPoint()
        {
            Id = Guid.NewGuid(),
            X = centerX,
            Y = centerY,
        };
    }
    private void FinishROI()
    {
        if (_firstROIPoint != null && _lastROIPoint != null && _roiPoints.Count > 2)
        {
            if (selectionLine != null)
            {
                if (canvas.Children.Contains(selectionLine)) canvas.Children.Remove(selectionLine);
            }
            // Create a Line element to connect the first and last dots
            Line line = new Line
            {
                X1 = _firstROIPoint.X,
                Y1 = _firstROIPoint.Y,
                X2 = _lastROIPoint.X,
                Y2 = _lastROIPoint.Y,
                Stroke = Brushes.Red,
                StrokeThickness = 1
            };

            canvas.Children.Add(line);
            var roiName = "";
            // prozor za unos naziva
            var window = new ROINameEntryWindow();

            window.Owner = this;
            window.Owner.Opacity = 0.5;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (window.ShowDialog() == true)
            {
                roiName = window.EnteredData;
                var roi = new ROI()
                {
                    Id = Guid.NewGuid(),
                    Name = roiName,
                    Points = _roiPoints,
                };

                DrawROIName(roi);

                ROIs.Add(roi);

                _roiPoints = new();
                _firstROIPoint = null;
                _lastROIPoint = null;
            }
            else
            {
                var roi = new ROI()
                {
                    Id = Guid.NewGuid(),
                    Name = roiName,
                    Points = _roiPoints,
                };

                ROIs.Add(roi);
                _roiPoints = new();
                _firstROIPoint = null;
                _lastROIPoint = null;
                RemoveLastROI();
            }

            window.Owner.Opacity = 1;
            Focus();
        }
    }
    private List<ROIPoint> ConvertToPoints(Ellipse[] ellipses)
    {
        var points = new List<ROIPoint>();
        foreach (Ellipse ellipse in ellipses)
        {
            points.Add(new ROIPoint() 
            { 
                Id = Guid.NewGuid(),
                X = Canvas.GetLeft(ellipse) + ellipse.Width / 2, 
                Y = Canvas.GetTop(ellipse) + ellipse.Height / 2,
            });
        }
        return points;
    }
    private void FinishROIByDrag()
    {
        if (cornerDots == null) return;
        if (selectionRectangle == null) return;


        var roiName = "";
        // prozor za unos naziva
        var window = new ROINameEntryWindow();

        window.Owner = this;
        window.Owner.Opacity = 0.5;
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        if (window.ShowDialog() == true)
        {
            roiName = window.EnteredData;
            var roi = new ROI()
            {
                Id = Guid.NewGuid(),
                Name = roiName,
                Points = ConvertToPoints(cornerDots),
            };
            ROIs.Add(roi);

            DrawROI(roi);
        }
        ClearDrag();
        window.Owner.Opacity = 1;
        Focus();
    }
    private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (isDragging) return;
        RemoveLastROIPoint();
    }  
    private void ClearDrag()
    {
        if (selectionRectangle != null) canvas.Children.Remove(selectionRectangle);
        if (cornerDots != null)
        {
            if(canvas.Children.Contains(cornerDots[0])) canvas.Children.Remove(cornerDots[0]);
            if (canvas.Children.Contains(cornerDots[1])) canvas.Children.Remove(cornerDots[1]);
            if (canvas.Children.Contains(cornerDots[2])) canvas.Children.Remove(cornerDots[2]);
            if (canvas.Children.Contains(cornerDots[3])) canvas.Children.Remove(cornerDots[3]);
        }
        
        selectionRectangle = null;
        cornerDots = null;
        isDragging = false;
    }
    private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        mouseDown = DateTime.Now;
        if (isDragging) return;
        if (_roiPoints.Count > 0) return;

        dragStartPoint = e.GetPosition(canvas);
        selectionRectangle = new Rectangle
        {
            Stroke = Brushes.Red,
            StrokeDashArray = new DoubleCollection { 2 },
            StrokeThickness = 1
        };
        canvas.Children.Add(selectionRectangle);

        cornerDots = new Ellipse[4];
        for (int i = 0; i < 4; i++)
        {
            cornerDots[i] = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Red
            };
            canvas.Children.Add(cornerDots[i]);
        }
        isDragging = true;
    }
    private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (DateTime.Now.Subtract(mouseDown) > interval)
        {
            // Mouse was dragged
            if (selectionRectangle == null) return;
            selectionRectangle.StrokeDashArray = new DoubleCollection { 1 };
            FinishROIByDrag();
        }
        else
        {
            ClearDrag();
            PlaceROIPoint(e);
        }
        isDragging = false;
    }
    private void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (isDragging)
        {
            if (_roiPoints.Count > 0) return;
            if (selectionRectangle == null) return;
            if (cornerDots == null) return;

            Point currentPoint = e.GetPosition(canvas);
            double width = Math.Abs(currentPoint.X - dragStartPoint.X);
            double height = Math.Abs(currentPoint.Y - dragStartPoint.Y);
            Canvas.SetLeft(selectionRectangle, Math.Min(dragStartPoint.X, currentPoint.X));
            Canvas.SetTop(selectionRectangle, Math.Min(dragStartPoint.Y, currentPoint.Y));
            selectionRectangle.Width = width;
            selectionRectangle.Height = height;

            double dotSize = 10;
            Canvas.SetLeft(cornerDots[0], dragStartPoint.X - dotSize / 2);
            Canvas.SetTop(cornerDots[0], dragStartPoint.Y - dotSize / 2);
            Canvas.SetLeft(cornerDots[1], dragStartPoint.X - dotSize / 2);
            Canvas.SetTop(cornerDots[1], currentPoint.Y - dotSize / 2);
            Canvas.SetLeft(cornerDots[2], currentPoint.X - dotSize / 2);
            Canvas.SetTop(cornerDots[2], currentPoint.Y - dotSize / 2);
            Canvas.SetLeft(cornerDots[3], currentPoint.X - dotSize / 2);
            Canvas.SetTop(cornerDots[3], dragStartPoint.Y - dotSize / 2);
        }
        
        if (_roiPoints.Count > 0 && selectionLine != null)
        {
            Point currentPoint = e.GetPosition(canvas);
            selectionLine.X2 = currentPoint.X;
            selectionLine.Y2 = currentPoint.Y;
        }
        
    }
}

