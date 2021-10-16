using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ProjectK.Games.LCR.Models;

namespace ProjectK.Games.LCR.ViewModels
{
    public class SimulatorViewModel : ViewModelBase
    {
        public Action DrawAxes { get; set; }
        public Action DrawCharts { get; set; }


        #region Fields

        private readonly List<PlayerViewModel> _players = new();
        private readonly List<GameModel> _games = new();
        private readonly Random _random = new();
        private int _numberOfPlayers;
        private int _numberOfGames;
        private int? _selectedPresetGameIndex;
        private int? _shortestLengthGameIndex;
        private int? _longestLengthGameIndex;
        private int? _averageLengthGame;


        #endregion

        #region Properties

        public int? SelectedPresetGameIndex
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

        public int? ShortestLengthGameIndex
        {
            get => _shortestLengthGameIndex;
            set => Set(ref _shortestLengthGameIndex, value);
        }
        public int? LongestLengthGameIndex
        {
            get => _longestLengthGameIndex;
            set => Set(ref _longestLengthGameIndex, value);
        }
        public int? AverageLengthGame
        {
            get => _averageLengthGame;
            set => Set(ref _averageLengthGame, value);
        }

        public List<GameSettings> PresetGames { get; set; } = new()
        {
            new GameSettings { NumberOfPlayers = 3, NumberOfGames = 100 },
            new GameSettings { NumberOfPlayers = 4, NumberOfGames = 100 },
            new GameSettings { NumberOfPlayers = 5, NumberOfGames = 100 },
            new GameSettings { NumberOfPlayers = 5, NumberOfGames = 1000 },
            new GameSettings { NumberOfPlayers = 5, NumberOfGames = 10000 },
            new GameSettings { NumberOfPlayers = 6, NumberOfGames = 100 },
            new GameSettings { NumberOfPlayers = 7, NumberOfGames = 100 },
        };

        public int NumberOfTurns { get; set; }
        public List<PlayerViewModel> Players => _players;
        public List<GameModel> Games => _games;


        public GameModel GetShortestLengthGame()
        {
            var games = Games;
            if (games.Count == 0)
                return null;

            if (ShortestLengthGameIndex == null)
                return null;

            var game = games[ShortestLengthGameIndex.Value];
            return game;
        }
        public GameModel GetLongestLengthGame()
        {
            var games = Games;
            if (games.Count == 0)
                return null;

            if (LongestLengthGameIndex == null)
                return null;

            var game = games[LongestLengthGameIndex.Value];
            return game;
        }

        #endregion

        #region Commands

        public ICommand PlayCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        #endregion

        public SimulatorViewModel()
        {
            // Init 
            PropertyChanged += OnPropertyChanged;

            // Set Commands
            PlayCommand = new RelayCommand(OnPlay);
            CancelCommand = new RelayCommand(OnCancel);

            // Update Settings
            SelectedPresetGameIndex = 0;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedPresetGameIndex):
                    OnPresetChanged();
                    break;
                case nameof(NumberOfPlayers):
                    CreatePlayers(NumberOfPlayers);
                    break;
                case nameof(NumberOfGames):
                    CreateGames(NumberOfGames);
                    break;
            }
        }

        private void OnPresetChanged()
        {
            if (SelectedPresetGameIndex == null)
                return;

            var selectedGame = PresetGames[SelectedPresetGameIndex.Value];
            NumberOfPlayers = selectedGame.NumberOfPlayers;
            NumberOfGames = selectedGame.NumberOfGames;
            ShortestLengthGameIndex = null;
            LongestLengthGameIndex = null;
            AverageLengthGame = null;
            DrawAxes?.Invoke();
        }

        private void OnCancel()
        {
            throw new NotImplementedException();
        }
        private void CreateGames(int count)
        {
            Logger.LogDebug($"Create {count} games.");
            _games.Clear();
            for (var i = 0; i < count; i++)
            {
                _games.Add(new GameModel { Index = i });
            }
        }

        private void CreatePlayers(int count)
        {
            Logger.LogDebug($"Create {count} players.");
            _players.Clear();
            for (var i = 0; i < count; i++)
            {
                _players.Add(new PlayerViewModel { Index = i });
            }
        }
        private void ResetPlayers()
        {
            Logger.LogDebug("Reset players");
            foreach (var player in _players)
            {
                player.Reset();
            }
        }
        private void ResetGames()
        {
            Logger.LogDebug("Reset games");
            foreach (var game in _games)
            {
                game.Reset();
            }
        }

        private void OnPlay()
        {
            Play();
            Analyze();
            DrawCharts?.Invoke();
        }
        private void Play()
        {
            Logger.LogDebug($"Game Started");
            Logger.LogDebug($"Players={NumberOfPlayers}, Games={NumberOfGames}");
            var rnd = _random;
            ResetGames();
            foreach (var game in _games)
            {
                ResetPlayers();
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
            NumberOfTurns = 0;

            foreach (var game in _games)
            {
                var turns = game.Turns;
                totalTurnsLength += turns;
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

            NumberOfTurns = longestLengthTurns;
            ShortestLengthGameIndex = shortestLengthGameIndex;
            LongestLengthGameIndex = longestLengthGameIndex;
            AverageLengthGame = totalTurnsLength / _games.Count;
            var game1 = _games[shortestLengthGameIndex];
            Logger.LogDebug($"Shortest=[Index={game1.Index}, Turns={game1.Turns}");
            var game2 = _games[longestLengthGameIndex];
            Logger.LogDebug($"Longest=[Index={game2.Index}, Turns={game2.Turns}");
            Logger.LogDebug($"Average=[{AverageLengthGame}]");
        }


        public List<(double x, double y)> DrawChart2((double x1, double y1, double x2, double y2) rect, (int x, int y) count)
        {
            var xCenters = GenericExtensions.GetAxisCenters(rect.x1, rect.x2, count.x, 1);
            var yCenters = GenericExtensions.GetAxisCenters(rect.y1, rect.y2, count.y, 1);
            var games = Games;
            var points = new List<(double x, double y)>();
            foreach (var game in games)
            {
                var x = xCenters[game.Index];
                var y = yCenters[game.Turns];
                points.Add((x, y));
            }
            return points;
        }

    }
}

