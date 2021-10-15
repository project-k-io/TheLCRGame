using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ProjectK.Games.LCR.Models;

namespace ProjectK.Games.LCR.ViewModels
{
    public class SimulatorViewModel : ViewModelBase
    {
        #region Fields

        private readonly List<PlayerViewModel> _players = new List<PlayerViewModel>();
        private readonly List<GameViewModel> _games = new List<GameViewModel>();
        private Random _random = new Random();
        private int _selectedPresetGameIndex;
        private int _numberOfPlayers;
        private int _numberOfGames;
        private int _shortestLengthGameIndex;
        private int _longestLengthGameIndex;
        private int _averageLengthGame;


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
        public int ShortestLengthGameIndex
        {
            get => _shortestLengthGameIndex;
            set => Set(ref _shortestLengthGameIndex, value);
        }
        public int LongestLengthGameIndex
        {
            get => _longestLengthGameIndex;
            set => Set(ref _longestLengthGameIndex, value);
        }
        public int AverageLengthGame
        {
            get => _averageLengthGame;
            set => Set(ref _averageLengthGame, value);
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
            SetGameCommand = new RelayCommand(OnSetGame);
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

        private void CreatePlayers()
        {
            _players.Clear();
            for (var i = 0; i < NumberOfPlayers; i++)
            {
                _players.Add(new PlayerViewModel { Index = i });
            }
        }

        private void CreateGames()
        {
            _games.Clear();
            for (var i = 0; i < NumberOfGames; i++)
            {
                _games.Add(new GameViewModel { Index = i });
            }
        }



        private void OnPlay()
        {
            Play();
            Analyze();
        }

        private void Play()
        {
            Logger.LogDebug($"Game Started");
            Logger.LogDebug($"Players={NumberOfPlayers}, Games={NumberOfGames}");
            var rnd = _random;
            CreateGames();
            foreach (var game in _games)
            {
                CreatePlayers();
                bool onlyOnePlayerHasChips = false;
                while (!onlyOnePlayerHasChips)
                {
                    foreach (var player in _players)
                    {
                        game.Turns++;
                        // if player doesn't have chips and it's his play, he is out  
                        if (player.NumberOfChips == 0)
                            player.Active = false;

                        // check if we have only one active player
                        var activePlayers = _players.Where(item => item.Active).ToList();
                        if (activePlayers.Count == 1)
                        {
                            activePlayers[0].NumberOfWins++;
                            game.Winner = player.Index;
                            onlyOnePlayerHasChips = true;
                            break;
                        }
                        // skip not active players
                        if (!player.Active)
                            continue;

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
                Logger.LogDebug($"Game={game}");
            }
            Logger.LogDebug($"Game Finished");
        }
        private void Analyze()
        {
            var shortestLengthTurns = int.MaxValue;
            int shortestLengthGameIndex = 0;
            var longestLengthTurns = int.MinValue;
            int longestLengthGameIndex = 0;
            var totalTurnsLength = 0;
            var totalTurnsCount = 0;

            foreach (var game in _games)
            {
                var turns = game.Turns;
                totalTurnsLength += turns;
                totalTurnsCount++;
                if (turns < shortestLengthTurns)
                {
                    shortestLengthTurns = turns;
                    shortestLengthGameIndex = game.Index;
                }

                else if (turns > longestLengthTurns)
                {
                    longestLengthTurns = turns;
                    longestLengthGameIndex = game.Index;
                }
            }

            ShortestLengthGameIndex = shortestLengthGameIndex;
            LongestLengthGameIndex = longestLengthGameIndex;
            AverageLengthGame = totalTurnsCount != 0 ? totalTurnsLength / totalTurnsCount : 0;
            var game1 = _games[shortestLengthGameIndex];
            Logger.LogDebug($"Shotest=[Index={game1.Index}, Turns={game1.Turns}");
            var game2 = _games[longestLengthGameIndex];
            Logger.LogDebug($"Longest=[Index={game2.Index}, Turns={game2.Turns}");
            Logger.LogDebug($"Average=[{AverageLengthGame}]");
        }
    }
}

