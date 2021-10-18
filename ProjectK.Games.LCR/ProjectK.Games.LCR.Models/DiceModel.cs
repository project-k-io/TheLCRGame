using System;

namespace ProjectK.Games.LCR.Models
{
    public class DiceModel
    {
        public DiceSide[] Sides { get; set; } =
            { DiceSide.Left, DiceSide.Center, DiceSide.Right, DiceSide.Dot, DiceSide.Dot, DiceSide.Dot };

        public DiceSide RolledSide { get; set; } = DiceSide.None;

        public void Roll(Random rnd)
        {
            var index = rnd.Next(0, 5);
            RolledSide = Sides[index];
        }
    }
}
