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

        public static void DrawText(this Canvas canvas, double x, double y, string text)
        {
            var textBlock = new TextBlock { Text = text };
            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);
            canvas.Children.Add(textBlock);
        }

        public static List<double> GetAxisCenters(double n1, double n2, int n, int delta)
        {
            var step = (n2 - n1) / n;
            var points = new List<double>();
            for (var i = 0; i <= n; i += delta)
            {
                var center = n1 + step * i;
                points.Add(center);
            }
            return points;
        }

        public static List<Point> GetAxisY(this Canvas canvas, double y1, double y2, double x, int n, int width)
        {
            int delta = n > 10 ? n / 10 : 1;
            var centers = GetAxisCenters(y1, y2, n, delta);
            var axis = new List<Point>();
            for (var i = 0; i < centers.Count; i++)
            {
                var y = centers[i];
                var point = new Point(x, y);
                var (left, right) = point.GetYPointLine(width);
                axis.AddRange(new[] { point, left, point, right, point });
                var index = i * delta;
                canvas.DrawText(left.X - 30, left.Y - 10, index.ToString());
            }
            return axis;
        }

        public static List<Point> GetAxisX(this Canvas canvas, double x1, double x2, double y, int n, int height)
        {
            int delta = n > 10 ? n / 10 : 1;
            var centers = GetAxisCenters(x1, x2,  n, delta);
            var axis = new List<Point>();
            for (var i = 0; i < centers.Count; i++)
            {
                var x = centers[i];
                var point = new Point(x, y);
                var (top, bottom) = point.GetXPointLine(height);
                axis.AddRange(new[] { point, top, point, bottom, point });
                var index = i * delta;
                canvas.DrawText(bottom.X - 15, bottom.Y + 10, index.ToString());
            }
            return axis;
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
            var xCenters = CanvasExtensions.GetAxisCenters(rect.x1, rect.x2,  count.x, 1);
            var yCenters = CanvasExtensions.GetAxisCenters(rect.y1, rect.y2, count.y, 1);
            (double x, double y) point = (xCenters[index.x], yCenters[index.y]);
            var width = 10;
            var ellipse = new Ellipse
            {
                Stroke = brush,
                Fill = brush,
                Width = width * 2,
                Height = width * 2
            };
            Canvas.SetLeft(ellipse, point.x + pointOffset.x);
            Canvas.SetTop(ellipse, point.y + pointOffset.y);
            // Text
            var textBlock = new TextBlock
            {
                Text = text,
                FontStyle = FontStyles.Italic,
                FontWeight = FontWeights.DemiBold,
                Foreground = brush,
                FontSize = 22
            };
            Canvas.SetLeft(textBlock, point.x + textOffset.x);
            Canvas.SetTop(textBlock, point.y + textOffset.y);
            canvas.Children.Add(ellipse);
            canvas.Children.Add(textBlock);
        }

    }
}
