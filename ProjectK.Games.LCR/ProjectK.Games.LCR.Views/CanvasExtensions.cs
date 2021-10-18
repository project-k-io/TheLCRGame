using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ProjectK.Games.LCR.Models;

namespace ProjectK.Games.LCR.Views
{
    public static class CanvasExtensions
    {
        public static void DrawLine(this Canvas canvas, List<Point> points, Color color)
        {
            canvas.DrawLine(points.ToArray(), color);
        }

        public static void DrawLine(this Canvas canvas, Point[] points, Color color)
        {
            var line = new Polyline();
            var collection = new PointCollection();
            foreach (var p in points)
            {
                collection.Add(p);
            }
            line.Points = collection;
            line.Stroke = new SolidColorBrush(color);
            line.StrokeThickness = 2;
            canvas.Children.Add(line);
        }

        public static void DrawText(this Canvas canvas, Point point, (int x, int y) offset, SolidColorBrush brush, double fontSize, string text, double angle = 0)
        {
            canvas.DrawText((point.X, point.Y), offset, brush, fontSize, text, angle);
        }

        public static void DrawText(this Canvas canvas, (double x, double y) point, (int x, int y) offset, SolidColorBrush brush, double fontSize, string text, double angle = 0)
        {
            var textBlock = new TextBlock
            {
                Foreground = brush,
                Text = text,
                FontSize = fontSize,
                LayoutTransform = new RotateTransform(angle)
            };
            Canvas.SetLeft(textBlock, point.x + offset.x);
            Canvas.SetTop(textBlock, point.y + offset.y);
            canvas.Children.Add(textBlock);
        }


        public static (List<Point> points, List<(Point point, int index)> labels) GetAxisY(double y1, double y2, double x, int n, int width)
        {
            int delta = n > 10 ? n / 10 : 1;
            var centers = GenericExtensions.GetAxisCenters(y1, y2, n, delta);
            var axis = new List<Point>();
            var labels = new List<(Point point, int index)>();
            for (var i = 0; i < centers.Count; i++)
            {
                var y = centers[i];
                var point = new Point(x, y);
                var (point1, point2) = point.GetYPointLine(width);
                axis.AddRange(new[] { point, point1, point, point2, point });
                var index = i * delta;
                labels.Add((point1, index));
            }
            return (axis, labels);
        }

        public static (List<Point> points, List<(Point point, int index)> labels)  GetAxisX(double x1, double x2, double y, int n, int height)
        {
            int delta = n > 10 ? n / 10 : 1;
            var centers = GenericExtensions.GetAxisCenters(x1, x2, n, delta);
            var axis = new List<Point>();
            var labels = new List<(Point point, int index)>();
            for (var i = 0; i < centers.Count; i++)
            {
                var x = centers[i];
                var point = new Point(x, y);
                var (point1, point2) = point.GetXPointLine(height);
                axis.AddRange(new[] { point, point1, point, point2, point });
                var index = i * delta;
                labels.Add((point2, index));
            }
            return (axis, labels);
        }

        public static void DrawAxisLabels(this Canvas canvas, List<(Point point, int index)> labels, (int x, int y) offset)
        {
            foreach (var (point, index) in labels)
            {
                canvas.DrawText(point, offset, Brushes.Black, 14, index.ToString());
            }
        }

        public static (Point left, Point right) GetYPointLine(this Point center, int width)
        {
            var left = new Point(center.X - width, center.Y);
            var right = new Point(center.X + width, center.Y);
            return (left, right);
        }

        public static (Point top, Point bottom) GetXPointLine(this Point center, int height)
        {
            var top = new Point(center.X, center.Y - height);
            var bottom = new Point(center.X, center.Y + height);
            return (top, bottom);
        }
        public static void DrawPointAndText(this Canvas canvas, 
            (double x1, double y1, double x2, double y2) rect, 
            (int x, int y) count, 
            (int x, int y) index,
            (int x, int y) pointOffset,
            (int x, int y) textOffset,
            string text,
            SolidColorBrush brush
            )
        {
            var xCenters = GenericExtensions.GetAxisCenters(rect.x1, rect.x2,  count.x, 1);
            var yCenters = GenericExtensions.GetAxisCenters(rect.y1, rect.y2, count.y, 1);
            var (x, y) = (xCenters[index.x], yCenters[index.y]);
            var width = 10;
            var ellipse = new Ellipse
            {
                Stroke = brush,
                Fill = brush,
                Width = width * 2,
                Height = width * 2
            };
            Canvas.SetLeft(ellipse, x + pointOffset.x);
            Canvas.SetTop(ellipse, y + pointOffset.y);
            // Text
            var textBlock = new TextBlock
            {
                Text = text,
                FontStyle = FontStyles.Italic,
                FontWeight = FontWeights.DemiBold,
                Foreground = brush,
                FontSize = 22
            };
            Canvas.SetLeft(textBlock, x + textOffset.x);
            Canvas.SetTop(textBlock, y + textOffset.y);
            canvas.Children.Add(ellipse);
            canvas.Children.Add(textBlock);
        }

        public static List<Point> ToPoints(this List<(double x, double y)> drawPoints)
        {
            var points = drawPoints.Select(p => new Point(p.x, p.y)).ToList();
            return points;
        }

    }
}
