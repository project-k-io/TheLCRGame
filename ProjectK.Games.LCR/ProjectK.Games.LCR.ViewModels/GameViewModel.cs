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
        public int NumberOfPlayers { get; set; }
        public int NumberOfGames{ get; set; }
        public override string ToString()
        {
            return $"{NumberOfPlayers} players x {NumberOfGames} games";
        }
    }
}
