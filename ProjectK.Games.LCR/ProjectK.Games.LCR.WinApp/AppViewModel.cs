using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectK.Games.LCR.ViewModels;

namespace ProjectK.Games.LCR.WinApp
{
    public class AppViewModel: SimulatorViewModel
    {
        public string AppName { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }

        public AppViewModel()
        {
            AppName = "The LCR Game";
            Version = "1.0.0";
            Title = $"{AppName} {Version}";
        }
    }
}
