using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ProjectK.Games.LCR.Views
{
    /// <summary>
    /// Interaction logic for GameChartView.xaml
    /// </summary>
    public partial class GameChartView : UserControl
    {
        StackPanel myStackPanel = new StackPanel();

        public GameChartView()
        {
            InitializeComponent();



        }

        void Draw()
        {
            canvas.Children.Clear();
            Rect bounds = this.TransformToVisual(canvas).TransformBounds(new Rect(this.RenderSize));
            var c = canvas;
            var dd = 40;
            var r = new Rect(dd, dd, c.ActualWidth - 2*dd, c.ActualHeight - 2*dd);

            var yPoints = GetAxisY(r.Bottom, r.Top, r.X, 10, 1);
            var xPoints = GetAxisX(r.X, r.Width, r.Bottom, 10, 1);
            var points = new List<Point>();
            points.AddRange(yPoints);
            points.AddRange(xPoints);
            DrawLine2(points.ToArray());
            DrawAxisX(r);
        }

        void DrawBorder(Rect r)
        {
            var x1 = r.X;
            var y1 = r.Y;
            var x2 = r.Right;
            var y2 = r.Bottom;
            var A = new Point(x1, y1);
            var B = new Point(x2, y1);
            var C = new Point(x2, y2);
            var D = new Point(x1, y2);
            var E = new Point(x1, y1);

            Point[] points = { A, B, C, D, E };
            DrawLine2(points);
        }
        void DrawAxisY(Rect r)
        {
            var x1 = r.X;
            var y1 = r.Y;
            var x2 = r.Right;
            var y2 = r.Bottom;
            var A = new Point(x1, y1);
            var D = new Point(x1, y2);
            Point[] points = { A, D };
            DrawLine2(points);
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
            var step = (y2 - y1) / n;
            var points = new List<Point>();
            for (var i = 0; i < n; i++)
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
            var step = (x2 - x1) / n;
            var points = new List<Point>();
            for (var i = 0; i < n; i++)
            {
                var center = new Point(x1 + step * i, y);
                var (top, bottom) = GetXPointLine(center, height);
                points.AddRange(new[] { center, top, center, bottom, center });
                DrawRectWithText(bottom.X-5, bottom.Y + 10, i.ToString());
            }
            return points;
        }

        void DrawAxisX(Rect r)
        {
            var x1 = r.X;
            var y1 = r.Y;
            var x2 = r.Right;
            var y2 = r.Bottom;
            var C = new Point(x2, y2);
            var D = new Point(x1, y2);
            Point[] points = { D, C };
            DrawLine2(points);
        }

        void DrawBorder()
        {
            var x1 = 10;
            var y1 = 10;
            var x2 = canvas.ActualWidth - 10;
            var y2 = canvas.ActualHeight - 10;
            var A = new Point(x1, y1);
            var B = new Point(x2, y1);
            var C = new Point(x2, y2);
            var D = new Point(x1, y2);
            var E = new Point(x1, y1);

            Point[] points = { A, B, C, D, E };
            DrawLine2(points);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Draw();
        }


        void AddLine(int x1, int y1, int x2, int y2)
        { // Create a red Ellipse.
            var line = new Line();
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;

            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
            line.Fill = mySolidColorBrush;
            line.StrokeThickness = 2;
            line.Stroke = Brushes.Black;

            myStackPanel.Children.Add(line);
        }

        private void DrawLine2(Point[] points)
        {
            Polyline line = new Polyline();
            PointCollection collection = new PointCollection();
            foreach (Point p in points)
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
