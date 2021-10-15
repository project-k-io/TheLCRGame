namespace ProjectK.Games.LCR.Models
{

    public class GameSettings
    {
        public int NumberOfPlayers { get; set; }
        public int NumberOfGames { get; set; }

        public override string ToString()
        {
            return $"{NumberOfPlayers} players x {NumberOfGames} games";
        }
    }
}