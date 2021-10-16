using System.Collections.Generic;
using System.Drawing.Text;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using ProjectK.Games.LCR.ViewModels;

namespace ProjectK.Games.LCR.Views
{
    /// <summary>
    /// Interaction logic for GameChartView.xaml
    /// </summary>
    public partial class GameChartView : UserControl
    {
        private SimulatorViewModel _simulator;

        public GameChartView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is SimulatorViewModel model)
            {
                _simulator = model;
                _simulator.PlayFinished += PlayFinished;
            }
        }

        private void PlayFinished()
        {
            Draw();
        }

        private void Draw()
        {
            canvas.Children.Clear();
            var c = canvas;
            var offset = 50;
            var r = new Rect(offset, offset, c.ActualWidth - 2 * offset, c.ActualHeight - 2 * offset);
            (double x1, double y1, double x2, double y2) rect = (r.X, r.Bottom, r.Width, r.Top);
            if (_simulator != null)
            {
                (int x, int y) count = (_simulator.NumberOfGames, _simulator.NumberOfTurns);
                DrawAxis(rect, count);
                DrawChart(rect, count);
                DrawAverage(rect, count.y);
                DrawShortest(rect, count);
                DrawLongest(rect, count);
            }
        }

        void DrawAxis((double x1, double y1, double x2, double y2) rect, (int x, int y) count)
        {
            var xAxis = canvas.GetAxisX(rect.x1, rect.x2, rect.y1, count.x, 1);
            var yAxis = canvas.GetAxisY(rect.y1, rect.y2, rect.x1, count.y, 1);
            var points = new List<Point>();
            points.AddRange(yAxis);
            points.AddRange(xAxis);
            canvas.DrawLine(points, Colors.Black);
        }


        void DrawChart((double x1, double y1, double x2, double y2) rect, (int x, int y) count)
        {
            var xCenters = CanvasExtensions.GetAxisCenters(rect.x1, rect.x2, count.x, 1);
            var yCenters = CanvasExtensions.GetAxisCenters(rect.y1, rect.y2, count.y, 1);
            var games = _simulator.Games;
            var points = new List<Point>();
            foreach (var game in games)
            {
                var x = xCenters[game.Index];
                var y = yCenters[game.Turns];
                var point = new Point(x, y);
                points.Add(point);
            }
            canvas.DrawLine(points, Colors.Red);
        }
        void DrawAverage((double x1, double y1, double x2, double y2) rect, int yCount)
        {
            var yCenters = CanvasExtensions.GetAxisCenters(rect.y1, rect.y2, yCount, 1);
            var y = yCenters[_simulator.AverageLengthGame];
            var p1 = new Point(rect.x1, y);
            var p2 = new Point(rect.x2, y);
            var points = new List<Point>();
            points.Add(p1);
            points.Add(p2);
            canvas.DrawLine(points, Colors.Green);
        }


        void DrawShortest((double x1, double y1, double x2, double y2) rect, (int x, int y) count)
        {
            var games = _simulator.Games;
            if(games.Count == 0)
                return;

            var game = games[_simulator.ShortestLengthGameIndex];
            (int x, int y) index = (game.Index, game.Turns);
            var text = $"Shortest ({game.Turns})";
            canvas.DrawPointAndText(rect, count, index, (-10, 0), (-40, 20), text, Brushes.BlueViolet);

        }
        void DrawLongest((double x1, double y1, double x2, double y2) rect, (int x, int y) count)
        {
            var games = _simulator.Games;
            if (games.Count == 0)
                return;

            var game = games[_simulator.LongestLengthGameIndex];
            (int x, int y) index = (game.Index, game.Turns);
            var text = $"Longest ({game.Turns})";
            canvas.DrawPointAndText(rect, count, index, (-10, -20), (-40, -60), text, Brushes.Gold);

        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Draw();
        }
    }
}
