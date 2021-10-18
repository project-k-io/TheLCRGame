using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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
        public Action<Action> Dispatcher { get; set; }


        #region Fields

        private readonly ObservableCollection<PlayerViewModel> _players = new();
        private readonly List<GameModel> _games = new();
        private readonly Random _random = new();
        private int _numberOfPlayers;
        private int _numberOfGames;
        private int? _selectedPresetGameIndex;
        private int? _shortestLengthGameIndex;
        private int? _longestLengthGameIndex;
        private int? _averageLengthGame;
        private int _selectedPlayerIndex;
        private bool _isRunning = false;

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
        public int SelectedPlayerIndex
        {
            get => _selectedPlayerIndex;
            set => Set(ref _selectedPlayerIndex, value);
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
        public ObservableCollection<PlayerViewModel> Players => _players;
        public List<GameModel> Games => _games;

        public bool IsRunning
        {
            get => _isRunning;
            set => Set(ref _isRunning, value);
        }


        #endregion

        #region Commands

        public ICommand PlayCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        #endregion

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

        public SimulatorViewModel()
        {
            // Init 
            PropertyChanged += async (s,e) => await OnPropertyChanged(s,e);

            // Set Commands
            PlayCommand = new RelayCommand(async () => await OnPlay());
            CancelCommand = new RelayCommand(OnCancel);

            // Update Settings
            SelectedPresetGameIndex = 0;
        }

        private async Task  OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedPresetGameIndex):
                    OnPresetChanged();
                    await OnPlay();
                    break;
                case nameof(NumberOfPlayers):
                    CreatePlayers(NumberOfPlayers);
                    await OnPlay();
                    break;
                case nameof(NumberOfGames):
                    CreateGames(NumberOfGames);
                    await OnPlay();
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

        private void CreateGames(int count)
        {
            // Logger.LogDebug($"Create {count} games.");
            _games.Clear();
            for (var i = 0; i < count; i++)
            {
                _games.Add(new GameModel { Index = i });
            }
        }

        private void CreatePlayers(int count)
        {
            // Logger.LogDebug($"Create {count} players.");
            _players.Clear();
            for (var i = 0; i < count; i++)
            {
                _players.Add(new PlayerViewModel { Index = i });
            }

        }
        private void ResetPlayers()
        {
            // Logger.LogDebug("Reset players");
            foreach (var player in _players)
            {
                player.Reset();
            }
        }

        private void ResetNumberOfWins()
        {
            // Logger.LogDebug("Reset players");
            foreach (var player in _players)
            {
                player.NumberOfWins = 0;
            }
        }

        private void ResetGames()
        {
            // Logger.LogDebug("Reset games");
            foreach (var game in _games)
            {
                game.Reset();
            }
        }

        public async Task OnPlay()
        {
            IsRunning = true;
            await Play();
            Analyze();
            DrawCharts?.Invoke();
            IsRunning = false;
        }
        private void OnCancel()
        {
            IsRunning = false;
        }


        public async Task Play()
        {
            Logger.LogDebug($"Game Started");
            Logger.LogDebug($"Players={NumberOfPlayers}, Games={NumberOfGames}");
            var rnd = _random;
            ResetGames();
            ResetNumberOfWins();
            foreach (var game in _games)
            {
                ResetPlayers();
                bool onlyOnePlayerHasChips = false;
                while (!onlyOnePlayerHasChips)
                {
                    foreach (var player in _players)
                    {
                        if(!IsRunning)
                            return;

                        game.Turns++;
                        var finished = await Task.Run(() => Play(player, rnd));
                        if (finished)
                        {
                            game.Winner = player.Index;
                            onlyOnePlayerHasChips = true;
                            break;
                        }
                    }
                }
                // Logger.LogDebug($"Game={game}");
            }
            Logger.LogDebug($"Game Finished");
        }

        private bool Play(PlayerViewModel player, Random rnd)
        {
            // if player doesn't have chips and it's his play, he is out  
            if (player.NumberOfChips == 0)
                player.Active = false;

            // check if we have only one active player
            var activePlayers = _players.Where(item => item.Active).ToList();
            if (activePlayers.Count == 1)
            {
                activePlayers[0].NumberOfWins++;
                return true;
            }

            // skip not active players
            if (!player.Active)
                return false;

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

            return false;
        }

        public void Analyze()
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

            // Find winner
            PlayerViewModel winner = null;
            foreach (var player in Players)
            {
                if (winner == null)
                {
                    winner = player;
                    continue;
                }

                if (player.NumberOfWins > winner.NumberOfWins)
                {
                    winner = player;
                }
            }

            if (winner != null)
            {
                winner.Winner = true;
                SelectedPlayerIndex = winner.Index;
            }

            NumberOfTurns = longestLengthTurns;
            ShortestLengthGameIndex = shortestLengthGameIndex;
            LongestLengthGameIndex = longestLengthGameIndex;
            if (!_games.IsNullOrEmpty())
            {
                AverageLengthGame = totalTurnsLength / _games.Count;
            }
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

