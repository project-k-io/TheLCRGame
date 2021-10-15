using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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
            this.Loaded += OnLoaded; 
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
            var offset = 40;
            var r = new Rect(offset, offset, c.ActualWidth - 2*offset, c.ActualHeight - 2*offset);
            if (_simulator != null)
            {
                var numberOfGames = _simulator.NumberOfGames;
                var numberOfTunes = _simulator.NumberOfTurns;
                DrawAxises(r, numberOfGames, numberOfTunes);
            }
        }

        private void DrawAxises(Rect r, int xCount, int yCount)
        {
            var (xAxis, xCenters) = canvas.GetAxisX(r.X, r.Width, r.Bottom, xCount, 1);
            var (yAxis, yCenters) = canvas.GetAxisY(r.Bottom, r.Top, r.X, yCount, 1);
            var points = new List<Point>();
            points.AddRange(yAxis);
            points.AddRange(xAxis);
            canvas.DrawLine(points.ToArray());
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Draw();
        }
    }
}
