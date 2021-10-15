using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace ProjectK.Games.LCR.ViewModels
{
    public class PlayerViewModel: ViewModelBase
    {
        public const int MaxNumberOfChips = 3;
        public int NumberOfChips { get; set; } = MaxNumberOfChips;
        public bool Active { get; set; } = true;
        public int NumberOfWins { get; set; }

        public List<DiceSide> Roll(Random rnd)
        {
            var numberOfRollingDices = NumberOfChips > MaxNumberOfChips ? MaxNumberOfChips : NumberOfChips;
            var dice = new DiceViewModel();
            var sides = new List<DiceSide>();
            for (var index = 0; index < numberOfRollingDices; index++)
            {
                  dice.Roll(rnd);
                  sides.Add(dice.RolledSide);
            }
            return sides;
        }
    }
}
