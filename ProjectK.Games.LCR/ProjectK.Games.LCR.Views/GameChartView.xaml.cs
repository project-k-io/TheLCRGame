using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using ScottPlot.Drawing.Colormaps;

namespace ProjectK.Games.LCR.Views
{
    /// <summary>
    /// Interaction logic for GameChartView.xaml
    /// </summary>
    public partial class GameChartView : UserControl
    {
        public GameChartView()
        {
            InitializeComponent();
        }

        void Draw()
        {
            canvas.Children.Clear();
            var c = canvas;
            var offset = 40;
            var r = new Rect(offset, offset, c.ActualWidth - 2*offset, c.ActualHeight - 2*offset);
            DrawAxis(r, 8, 1000);
        }

        void DrawAxis(Rect r, int xCount, int yCount)
        {
            var yPoints = GetAxisY(r.Top, r.Bottom, r.X, xCount, 1);
            var xPoints = GetAxisX(r.X, r.Width, r.Bottom, yCount,  1);
            var points = new List<Point>();
            points.AddRange(yPoints);
            points.AddRange(xPoints);
            DrawLine(points.ToArray());
        }

        internal void DrawRectWithText(double x, double y, string n)
        {

            var text = new TextBlock()
            {
                Text = n
            };
            // Use Canvas's static methods to position the text
            Canvas.SetLeft(text, x);
            Canvas.SetTop(text, y);

            // Draw the rectange and the text to my Canvas control.
            // DrawCanvas is the name of my Canvas control in the XAML code
            canvas.Children.Add(text);
        }

        (Point left, Point right) GetYPointLine(Point center, int width)
        {
            var left = new Point(center.X - width, center.Y);
            var right = new Point(center.X + width, center.Y);
            return (left, right);
        }
        (Point top, Point bottom) GetXPointLine(Point center, int height)
        {
            var top = new Point(center.X , center.Y - height);
            var bottom= new Point(center.X, center.Y + height);
            return (top, bottom);
        }


        List<Point> GetAxisY(double y1, double y2, double x, int n, int width)
        {
            int delta = n > 10 ? n / 10 : 1;
            var step = (y2 - y1) / n;
            var points = new List<Point>();
            for (var i = 0; i <= n; i+=delta)
            {
                var center = new Point(x, y2 - step * i);
                var (left, right) = GetYPointLine(center, width);
                points.AddRange(new [] {center, left, center, right, center});
                DrawRectWithText(left.X - 20, left.Y - 10, i.ToString());
            }
            return points;
        }
        List<Point> GetAxisX(double x1, double x2, double y, int n, int height)
        {
            int delta = n > 10 ? n / 10 : 1;
            var step = (x2 - x1) / n;
            var points = new List<Point>();
            for (var i = 0; i <= n; i += delta)
            {
                var center = new Point(x1 + step * i, y);
                var (top, bottom) = GetXPointLine(center, height);
                points.AddRange(new[] { center, top, center, bottom, center });
                DrawRectWithText(bottom.X-5, bottom.Y + 10, i.ToString());
            }
            return points;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Draw();
        }


        private void DrawLine(Point[] points)
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
    }
}
