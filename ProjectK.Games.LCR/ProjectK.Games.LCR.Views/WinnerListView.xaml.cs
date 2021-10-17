using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProjectK.Games.LCR.ViewModels;

namespace ProjectK.Games.LCR.Views
{
    /// <summary>
    /// Interaction logic for WinnerListView.xaml
    /// </summary>
    public partial class WinnerListView : UserControl
    {
        private SimulatorViewModel _simulator;

        public WinnerListView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is SimulatorViewModel model)
            {
                _simulator = model;
                _simulator.DrawCharts += OnDrawCharts;
            }
        }
        private void OnDrawCharts()
        {
            PlayerList.ScrollIntoView(PlayerList.SelectedItem);
        }
    }
}
