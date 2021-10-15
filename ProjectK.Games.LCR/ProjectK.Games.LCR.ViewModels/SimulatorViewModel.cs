using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ProjectK.Games.LCR.ViewModels
{
    public class SimulatorViewModel: ViewModelBase
    {
        #region Fields

        private int _selectedPresetGameIndex;
        private int _numberOfPlayers;
        private int _numberOfGames;

        #endregion

        #region Properties

        public int SelectedPresetGameIndex
        {
            get => _selectedPresetGameIndex;
            set => Set(ref _selectedPresetGameIndex, value);
        }
        public int NumberOfPlayers
        {
            get => _numberOfPlayers;
            set => Set(ref _numberOfPlayers, value);
        }
        public int NumberOfGames
        {
            get => _numberOfGames;
            set => Set(ref _numberOfGames, value);
        }
        public GameViewModel Game { get; set; } = new() { NumberOfPlayers = 3, NumberOfGames = 100 };

        public List<GameViewModel> PresetGames { get; set; } = new()
        {
            new() { NumberOfPlayers = 3, NumberOfGames = 100 },
            new() { NumberOfPlayers = 4, NumberOfGames = 100 },
            new() { NumberOfPlayers = 5, NumberOfGames = 100 },
            new() { NumberOfPlayers = 5, NumberOfGames = 1000 },
            new() { NumberOfPlayers = 5, NumberOfGames = 10000 },
            new() { NumberOfPlayers = 6, NumberOfGames = 100 },
            new() { NumberOfPlayers = 7, NumberOfGames = 100 },
        };


        #endregion

        #region Commands

        public ICommand PlayCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand SetGameCommand { get; set; }

        #endregion

        public SimulatorViewModel()
        {
            // Init 
            SelectedPresetGameIndex = 0;
            OnSetGame();

            // Set Commands
            PlayCommand = new RelayCommand(OnPlay);
            CancelCommand = new RelayCommand(OnCancel);
            SetGameCommand  = new RelayCommand(OnSetGame);
        }

        private void OnSetGame()
        {
            var selectedGame = PresetGames[SelectedPresetGameIndex];
            NumberOfPlayers = selectedGame.NumberOfPlayers;
            NumberOfGames = selectedGame.NumberOfGames;
        }
        private void OnCancel()
        {
            throw new NotImplementedException();
        }
        private void OnPlay()
        {
            var numberOfGames = Game.NumberOfGames;
            var numberOfPlayers = Game.NumberOfPlayers;
            PlayerViewModel[] players = new PlayerViewModel[numberOfPlayers];

            Random rnd = new Random();
            for (var gameIndex = 0; gameIndex < numberOfGames; gameIndex++)
            {
                int playerIndex = 0;
                while(true) 
                {
                    var player = players[playerIndex++];

                    // if player doesn't have chips and it's his play, he is out  
                    if (player.NumberOfChips == 0)
                        player.Active = false;

                    // skip not active players
                    if (!player.Active)
                        continue;

                    var activePlayers = players.Where(item => item.Active).ToList();
                    if (activePlayers.Count == 1)
                    {
                        activePlayers[0].NumberOfWins++;
                        break;
                    }

                    var activePlayerIndex = activePlayers.IndexOf(player);
                    var leftPlayer = activePlayers.GetNextItem(activePlayerIndex);
                    var rightPlayer = activePlayers.GetPrevItem(activePlayerIndex);

                    var sides = player.Roll(rnd);
                    foreach (var side in sides)
                    {
                        switch (side)
                        {
                            case DiceSide.Dot:
                                break;
                            case DiceSide.Left:
                                leftPlayer.NumberOfChips++;
                                player.NumberOfChips--;
                                break;
                            case DiceSide.Right:
                                rightPlayer.NumberOfChips++;
                                player.NumberOfChips--;
                                break;
                            case DiceSide.Center:
                                player.NumberOfChips--;
                                break;
                        }
                    }
                }
            }
        }
    }
}

