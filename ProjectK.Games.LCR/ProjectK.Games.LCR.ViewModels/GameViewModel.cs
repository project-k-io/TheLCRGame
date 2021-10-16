using GalaSoft.MvvmLight;

namespace ProjectK.Games.LCR.ViewModels
{

    public class GameViewModel : ViewModelBase
    {
        public const int NoWinner = -1;
        public int Turns { get; set; }
        public int Index { get; set; }
        public int? Winner { get; set; }

        public override string ToString()
        {
            return $"[Index={Index,3}, Turns={Turns,3}, Winner={Winner,2}]";
        }

        public void Reset()
        {
            Turns = 0;
            Winner = null;
        }
    }
}
