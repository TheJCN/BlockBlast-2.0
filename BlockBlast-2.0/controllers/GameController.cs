using BlockBlast_2._0.models;
using BlockBlast_2._0.utils;
using Timer = System.Windows.Forms.Timer;

namespace BlockBlast_2._0.controllers;

public class GameController
{
    public List<Player> Players { get; }
    private int _currentPlayerIndex;
    private readonly int _gridSize;
    private readonly Panel[,] _cells;
    private int _timeLeft;
    private readonly int _timeLimit;
    private Timer _turnTimer = null!;
    private Figure _currentDraggingFigure = null!;
    private bool _isDragging;
    private bool _soundEnabled;

    public event EventHandler OnFigurePlaced = null!;
    public event EventHandler OnTurnEnded = null!;

    public GameController(int playerCount, int gridSize, int timeLimit, bool _soundEnabled)
    {
        Players = [];
        _gridSize = gridSize;
        _timeLimit = timeLimit;
        _soundEnabled = _soundEnabled;
        _cells = new Panel[gridSize, gridSize];
        InitializeCells();
            
        var colors = new[]
        {
            Color.Blue, Color.Red, Color.Green, Color.Yellow
        };

        for (var i = 0; i < playerCount; i++)
        {
            var playerColor = colors[i].ToArgb();
            Players.Add(new Player(playerColor));
        }
            
        _currentPlayerIndex = 0;
    }

    public void SetupDragDropHandlers(Panel fieldPanel)
    {
        fieldPanel.DragEnter += (_, e) => e.Effect = DragDropEffects.Copy;
        fieldPanel.DragOver += HandleDragOver!;
        fieldPanel.DragDrop += HandleDragDrop!;
    }

    private void HandleDragOver(object sender, DragEventArgs e)
    {
        if (!_isDragging) return;

        if (sender is not Panel fieldPanel) return;

        var clientPos = fieldPanel.PointToClient(new Point(e.X, e.Y));
        HighlightCells(clientPos);
    }

    private void HandleDragDrop(object sender, DragEventArgs e)
    {
        if (!_isDragging) return;

        if (sender is not Panel fieldPanel) return;

        var clientPos = fieldPanel.PointToClient(new Point(e.X, e.Y));
        var placementSuccessful = TryPlaceFigure(clientPos);

        if (placementSuccessful)
            OnFigurePlaced(this, EventArgs.Empty);
    }

    private void InitializeCells()
    {
        for (var row = 0; row < _gridSize; row++)
        for (var col = 0; col < _gridSize; col++)
        {
            _cells[row, col] = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
        }
    }

    public Panel[,] GetCells() => _cells;

    public Player CurrentPlayer => Players[_currentPlayerIndex];

    public void NextPlayer()
    {
        _currentPlayerIndex = (_currentPlayerIndex + 1) % Players.Count;
        OnTurnEnded.Invoke(this, EventArgs.Empty);
    }

    public void GenerateFigures()
    {
        foreach (var player in Players)
            player.GenerateFigures();
    }

    public bool PlaceFigure(Figure figure, int baseRow, int baseCol)
    {
        foreach (var pix in figure.Pixels)
        {
            var x = baseRow + pix.X;
            var y = baseCol + pix.Y;

            if (x < 0 || x >= _gridSize || y < 0 || y >= _gridSize)
                return false;

            if (_cells[x, y].BackColor != Color.White)
                return false;
        }
                
        foreach (var pix in figure.Pixels)
        {
            var x = baseRow + pix.X;
            var y = baseCol + pix.Y;
            _cells[x, y].BackColor = Color.FromArgb(pix.Color);
        }

        return true;
    }

    public void CheckAndClearLines()
    {
        var linesCleared = 0;
                
        for (var i = 0; i < _gridSize; i++)
        {
            var isRowFull = true;
            var isColFull = true;

            for (var j = 0; j < _gridSize; j++)
            {
                if (_cells[i, j].BackColor == Color.White) isRowFull = false;
                if (_cells[j, i].BackColor == Color.White) isColFull = false;
            }

            if (isRowFull)
            {
                linesCleared++;
                ClearLine(i, 0, 0, 1);
            }

            if (isColFull)
            {
                linesCleared++;
                ClearLine(0, i, 1, 0);
            }
        }
                
        var isDiagonal1Full = true;
        var isDiagonal2Full = true;

        for (var i = 0; i < _gridSize; i++)
        {
            if (_cells[i, i].BackColor == Color.White) isDiagonal1Full = false;
            if (_cells[i, _gridSize - i - 1].BackColor == Color.White) isDiagonal2Full = false;
        }

        if (isDiagonal1Full)
        {
            linesCleared++;
            ClearLine(0, 0, 1, 1);
        }

        if (isDiagonal2Full)
        {
            linesCleared++;
            ClearLine(0, _gridSize - 1, 1, -1);
        }
                
        var score = linesCleared * 100;
        CurrentPlayer.AddScore(score);
    }

    private void ClearLine(int startX, int startY, int stepX, int stepY)
    {
        Task.Run(() => SoundPlayerUtil.Play("resources\\sounds\\blast.wav"));
        for (var i = 0; i < _gridSize; i++)
        {
            var x = startX + i * stepX;
            var y = startY + i * stepY;
            _cells[x, y].BackColor = Color.White;
        }
    }

    public bool CanPlayerPlaceAnyFigure(Player player) => 
        player.Figures.Any(CanPlaceFigure);

    public bool CanPlaceFigure(Figure figure)
    {
        for (var row = 0; row < _gridSize; row++)
        for (var col = 0; col < _gridSize; col++)
        {
            var canPlace = !(from pix in figure.Pixels 
                            let x = row + pix.X 
                            let y = col + pix.Y 
                            where x < 0 || x >= _gridSize || y < 0 || y >= _gridSize || _cells[x, y].BackColor != Color.White 
                            select x).Any();
            if (canPlace) return true;
        }
        return false;
    }

    public void InitializeTimer(EventHandler tickHandler)
    {
        _turnTimer = new Timer
        {
            Interval = 1000
        };
        _turnTimer.Tick += tickHandler;

        if (_timeLimit <= 0) return;
        _timeLeft = _timeLimit;
        _turnTimer.Start();
    }

    public void UpdateTimer()
    {
        _timeLeft--;
        if (_timeLeft <= 0)
            _turnTimer.Stop();
    }

    public string GetTimeText() => _timeLimit > 0 ? $"Осталось времени: {_timeLeft} сек." : "Время на ход не ограничено";

    public Color GetTimeLabelColor() => _timeLeft <= 5 ? Color.Red : Color.White;

    public bool IsTimeUp() => _timeLimit > 0 && _timeLeft <= 0;

    public void ResetTimer()
    {
        if (_timeLimit <= 0) return;
        _timeLeft = _timeLimit;
        _turnTimer.Start();
    }

    public void StopTimer()
    {
        _turnTimer.Stop();
    }

    public void StartDragging(Figure figure)
    {
        _currentDraggingFigure = figure;
        _isDragging = true;
    }

    private void EndDragging()
    {
        _currentDraggingFigure = null!;
        _isDragging = false;
    }

    private void HighlightCells(Point clientPos)
    {
        var cellSize = _cells[0, 0].Width + 2;
        var row = clientPos.Y / cellSize;
        var col = clientPos.X / cellSize;
                
        foreach (var cell in _cells)
        {
            if (cell.BackColor == Color.LightGreen)
                cell.BackColor = Color.White;
        }

        foreach (var pix in _currentDraggingFigure.Pixels)
        {
            var x = row + pix.X;
            var y = col + pix.Y;

            if (x < 0 || x >= _gridSize || y < 0 || y >= _gridSize) 
                continue;

            if (_cells[x, y].BackColor == Color.White)
                _cells[x, y].BackColor = Color.LightGreen;
        }
    }

    public bool TryPlaceFigure(Point clientPos)
    {
        var cellSize = _cells[0, 0].Width + 2;
        var row = clientPos.Y / cellSize;
        var col = clientPos.X / cellSize;
            
        foreach (var cell in _cells)
            if (cell.BackColor == Color.LightGreen)
                cell.BackColor = Color.White;

        var placed = PlaceFigure(_currentDraggingFigure, row, col);

        if (!placed) return false;

        CurrentPlayer.RemoveFigure(_currentDraggingFigure);
        EndDragging();

        CheckAndClearLines();
            
        if (CurrentPlayer.Figures.Count == 0)
            CurrentPlayer.GenerateFigures();

        return true; 
    }

    public string GetEndGameMessage()
    {
        if (Players.Count == 1)
            return $"Игра окончена! Ваш счет: {Players[0].Score}";
        
        var winner = Players.OrderByDescending(p => p.Score).FirstOrDefault();
        var winnerText = Players.GroupBy(p => p.Score).Count() == 1 
            ? "Ничья!" 
            : $"Победитель: Игрок {Players.IndexOf(winner!) + 1}";

        var scores = string.Join("\n", Players.Select((p, i) => 
            $"Игрок {i + 1}: {p.Score} очков"));
                    
        return $"{winnerText}\n{scores}";
    }

    public string GetCurrentPlayerText() => $"Сейчас ходит: Игрок {Players.IndexOf(CurrentPlayer) + 1}";

    public Color GetCurrentPlayerPanelColor() => Color.FromArgb(80, Color.FromArgb(CurrentPlayer.Figures.FirstOrDefault()?.Pixels.FirstOrDefault()?.Color ?? Color.Gray.ToArgb()));

    public string GetPlayerScoreText(int playerIndex) => $"Очки: {Players[playerIndex].Score}";
    
    public int GetTimeLimit() => _timeLimit;
    public int GetTimeLeft() => _timeLeft;
}