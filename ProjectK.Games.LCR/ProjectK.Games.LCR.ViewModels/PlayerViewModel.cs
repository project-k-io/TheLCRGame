using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using ProjectK.Games.LCR.Models;

namespace ProjectK.Games.LCR.ViewModels
{
    public class PlayerViewModel: ViewModelBase
    {
        public const int MaxNumberOfChips = 3;
        #region Fields

        private int _index;
        private bool _winner;
        private int _numberOfWins;

        #endregion

        #region Properties
        public int NumberOfChips { get; set; } = MaxNumberOfChips;
        public bool Active { get; set; } = true;

        public int Index
        {
            get => _index;
            set => Set(ref _index, value);
        }
        public bool Winner
        {
            get => _winner;
            set => Set(ref _winner, value);
        }
        public int NumberOfWins
        {
            get => _numberOfWins;
            set => Set(ref _numberOfWins, value);
        }
        #endregion



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

        public void Reset()
        {
            NumberOfChips = MaxNumberOfChips;
            Active = true;
            Winner = false;
        }
    }
}
