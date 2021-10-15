using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ProjectK.Games.LCR.Views
{
    public static class CanvasExtensions
    {
        public static void DrawLine(this Canvas canvas, Point[] points)
        {
            var line = new Polyline();
            var collection = new PointCollection();
            foreach (var p in points)
            {
                collection.Add(p);
            }
            line.Points = collection;
            line.Stroke = new SolidColorBrush(Colors.Black);
            line.StrokeThickness = 1;
            canvas.Children.Add(line);
        }

        public static void DrawText(this Canvas canvas, double x, double y, string text)
        {
            var textBlock = new TextBlock{ Text = text };
            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);
            canvas.Children.Add(textBlock);
        }
        public static List<Point> GetAxisYCenters(double y1, double y2, double x, int n)
        {
            int delta = n > 10 ? n / 10 : 1;
            var step = (y2 - y1) / n;
            var points = new List<Point>();
            for (var i = 0; i <= n; i += delta)
            {
                var center = new Point(x, y2 - step * i);
                points.Add( center);
            }
            return points;
        }

        public static List<Point> GetAxisY(this Canvas canvas, double y1, double y2, double x, int n, int width)
        {
            var centers = GetAxisYCenters(y1, y2, x, n);
            var points = new List<Point>();
            for (var i = 0; i < centers.Count; i ++)
            {
                var center = centers[i];
                var (left, right) = center.GetYPointLine(width);
                points.AddRange(new[] { center, left, center, right, center });
                canvas.DrawText(left.X - 20, left.Y - 10, i.ToString());
            }
            return points;
        }

        public static List<Point> GetAxisX(this Canvas canvas, double x1, double x2, double y, int n, int height)
        {
            int delta = n > 10 ? n / 10 : 1;
            var step = (x2 - x1) / n;
            var points = new List<Point>();
            for (var i = 0; i <= n; i += delta)
            {
                var center = new Point(x1 + step * i, y);
                var (top, bottom) = center.GetXPointLine(height);
                points.AddRange(new[] { center, top, center, bottom, center });
                canvas.DrawText(bottom.X - 5, bottom.Y + 10, i.ToString());
            }
            return points;
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

    }
}
