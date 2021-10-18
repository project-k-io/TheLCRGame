using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ProjectK.Games.LCR.Models;
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
            Loaded += async(s,e) => await OnLoaded(s, e);
        }

        private async Task OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is SimulatorViewModel model)
            {
                _simulator = model;
                _simulator.Dispatcher = a => Dispatcher.BeginInvoke(a);
                _simulator.DrawAxes += OnDrawAxes;
                _simulator.DrawCharts += OnDrawCharts;
                await _simulator.OnPlay();
            }
        }

        private void OnDrawAxes()
        {
            Draw();
        }
        private void OnDrawCharts()
        {
            Draw(true);
        }

        private void Draw(bool drawCharts = false)
        {
            canvas.Children.Clear();
            var c = canvas;
            var offset = 100;
            var r = new Rect(offset, offset, c.ActualWidth - 2 * offset, c.ActualHeight - 2 * offset);
            (double x1, double y1, double x2, double y2) rect = (r.X, r.Bottom, r.Width, r.Top);
            if (_simulator != null)
            {
                (int x, int y) count = (_simulator.NumberOfGames, _simulator.NumberOfTurns + 10);
                DrawAxis(rect, count);
                if (drawCharts)
                {
                    DrawChart(rect, count);
                    DrawAverage(rect, count.y);
                    DrawShortest(rect, count);
                    DrawLongest(rect, count);
                }
                DrawGameNotation(rect);
                DrawLabels(rect);
            }
        }

        private void DrawLabels((double x1, double y1, double x2, double y2) rect)
        {
            var point = new Point((rect.x2 - rect.x1) / 2, rect.y1);
            canvas.DrawText(point, (0, 20), Brushes.Black, 28, "Games");
            var point2 = new Point(rect.x1, (rect.y1 - rect.y2) / 2);
            canvas.DrawText(point2, (-80, 0), Brushes.Black, 28, "Turns", 270);
        }

        void DrawAxis((double x1, double y1, double x2, double y2) rect, (int x, int y) count)
        {
            var (xPoints, xLabels) = CanvasExtensions.GetAxisX(rect.x1, rect.x2, rect.y1, count.x, 1);
            var (yPoints, yLabels) = CanvasExtensions.GetAxisY(rect.y1, rect.y2, rect.x1, count.y, 1);
            canvas.DrawLine(xPoints, Colors.Black);
            canvas.DrawAxisLabels(xLabels, (-15, 10));
            canvas.DrawLine(yPoints, Colors.Black);
            canvas.DrawAxisLabels(yLabels, (-30, -10));
        }

        void DrawChart((double x1, double y1, double x2, double y2) rect, (int x, int y) count)
        {
            var games = _simulator.Games;
            var drawPoints = games.GetPoints(rect, count);
            var points = drawPoints.ToPoints();
            canvas.DrawLine(points, Colors.Red);
        }

        void DrawAverage((double x1, double y1, double x2, double y2) rect, int yCount)
        {
            if (_simulator.AverageLengthGame == null)
                return;

            var yCenters = GenericExtensions.GetAxisCenters(rect.y1, rect.y2, yCount, 1);
            var y = yCenters[_simulator.AverageLengthGame.Value];
            var p1 = new Point(rect.x1, y);
            var p2 = new Point(rect.x2, y);
            var points = new List<Point>
            {
                p1,
                p2
            };
            canvas.DrawLine(points, Colors.Green);
        }


        void DrawShortest((double x1, double y1, double x2, double y2) rect, (int x, int y) count)
        {
            var game = _simulator.GetShortestLengthGame();
            if (game == null)
                return;

            (int x, int y) index = (game.Index, game.Turns);
            var text = $"Shortest ({game.Turns})";
            canvas.DrawPointAndText(rect, count, index, (-10, 0), (-40, 20), text, Brushes.BlueViolet);

        }
        void DrawLongest((double x1, double y1, double x2, double y2) rect, (int x, int y) count)
        {
            var game = _simulator.GetLongestLengthGame();
            if(game == null)
                return;

            (int x, int y) index = (game.Index, game.Turns);
            var text = $"Longest ({game.Turns})";
            canvas.DrawPointAndText(rect, count, index, (-10, -20), (-40, -60), text, Brushes.Gold);

        }
        private void DrawGameNotation((double x1, double y1, double x2, double y2) rect)
        {
            var p1 = new Point(rect.x2 - 100, rect.y2 - 20);
            var p2 = new Point(p1.X + 50, p1.Y);
            var points = new List<Point>() { p1, p2 };
            canvas.DrawLine(points, Colors.Red);
            canvas.DrawText(p2, (10, -20), Brushes.Black, 22, "Game");

            var p3 = new Point(p1.X, p1.Y + 40);
            var p4 = new Point(p2.X, p2.Y + 40);
            var points2 = new List<Point>() { p3, p4 };
            canvas.DrawLine(points2, Colors.Green);
            canvas.DrawText(p4, (10, -20), Brushes.Black, 22, "Average");
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Draw();
        }
    }
}
