using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using ProjectK.Games.LCR.Models;

namespace ProjectK.Games.LCR.ViewModels
{
    public class PlayerViewModel: ViewModelBase
    {
        public const int MaxNumberOfChips = 3;

        public int Index { get; set; }
        public int NumberOfChips { get; set; } = MaxNumberOfChips;
        public bool Active { get; set; } = true;
        public int NumberOfWins { get; set; }

        public override string ToString()
        {
            return $"[Index={Index}, Chips={NumberOfChips},  Active={Active}]";
        }

        public List<DiceSide> Roll(Random rnd)
        {
            var numberOfRollingDices = NumberOfChips > MaxNumberOfChips ? MaxNumberOfChips : NumberOfChips;
            var dice = new DiceModel();
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
