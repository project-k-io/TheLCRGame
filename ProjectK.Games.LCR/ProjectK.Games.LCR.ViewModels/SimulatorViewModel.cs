using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ProjectK.Games.LCR.Models;

namespace ProjectK.Games.LCR.ViewModels
{
    public class SimulatorViewModel: ViewModelBase
    {
        #region Fields

        private int _selectedPresetGameIndex;
        private int _numberOfPlayers;
        private int _numberOfGames;
        List<PlayerViewModel> _players = new List<PlayerViewModel>();
        List<GameViewModel> _games = new List<GameViewModel>();

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

        public List<GameSettings> PresetGames { get; set; } = new()
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

        void CreatePlayers()
        {
            _players.Clear();
            for (var i = 0; i < NumberOfPlayers; i++)
            {
                _players.Add(new PlayerViewModel { Index = i });
            }
        }

        void CreateGames()
        {
            _games.Clear();
            for (var i = 0; i < NumberOfGames; i++)
            {
                _games.Add(new GameViewModel { Index = i });
            }
        }

        private void OnPlay()
        {
            var rnd = new Random();
            Play(rnd);
        }

        private void Play(Random rnd)
        {
            List<int> xx = new List<int> { 1, 2, 3, 3 };

            Logger.LogDebug($"Players={NumberOfPlayers}, Games={NumberOfGames}");
            CreateGames();
            foreach (var game in _games)
            {
#if DEBUG_DETAILS
                Logger.LogDebug($"Game={game}");
#endif
                CreatePlayers();
                bool onlyOnePlayerHasChips = false;
                while (!onlyOnePlayerHasChips)
                {
                    for(var playerIndex=0;  playerIndex < _players.Count; playerIndex++)
                    {
                        var player = _players[playerIndex];
                        game.Turns++;
#if DEBUG_DETAILS
                        Logger.LogDebug($"Player={player}");
#endif
                        // if player doesn't have chips and it's his play, he is out  
                        if (player.NumberOfChips == 0)
                            player.Active = false;

                        // check if we have only one active player
                        var activePlayers = _players.Where(item => item.Active).ToList();
                        if (activePlayers.Count == 1)
                        {
                            activePlayers[0].NumberOfWins++;
                            onlyOnePlayerHasChips = true;
                            break;
                        }
                        // skip not active players
                        if (!player.Active)
                            continue;

                        var activePlayerIndex = activePlayers.IndexOf(player);
                        var leftPlayer = activePlayers.GetNextItem(activePlayerIndex);
                        var rightPlayer = activePlayers.GetPrevItem(activePlayerIndex);
#if DEBUG_DETAILS
                        Logger.LogDebug($"Left  ={leftPlayer}");
                        Logger.LogDebug($"Right ={rightPlayer}");
#endif

                        var sides = player.Roll(rnd);
#if DEBUG_DETAILS
                        Logger.LogDebug(sides.ToText());
#endif
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

#if DEBUG_DETAILS
                        Logger.LogDebug($"Player={player}");
                        Logger.LogDebug($"Left  ={leftPlayer}");
                        Logger.LogDebug($"Right ={rightPlayer}");
                        Logger.LogDebug("");
#endif
                    }
                }
                Logger.LogDebug($"Game={game}");
            }

#if DEBUG_DETAILS
            Logger.LogDebug($"Game results");
            foreach (var game in _games)
                Logger.LogDebug($"Game={game}");

            Logger.LogDebug($"Game Finished");
#endif
        }
    }
}

