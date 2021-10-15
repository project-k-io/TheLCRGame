using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace ProjectK.Games.LCR.ViewModels
{

    public class GameViewModel : ViewModelBase
    {
        public int Turns { get; set; }
        public int Index { get; set; }
        public override string ToString()
        {
            return $"[Index={Index,3}, Turns={Turns,3}]";
        }
    }
}
