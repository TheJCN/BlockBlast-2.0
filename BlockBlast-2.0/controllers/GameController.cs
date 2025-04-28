using BlockBlast_2._0.models;
using System.Drawing;
using Timer = System.Windows.Forms.Timer;

namespace BlockBlast_2._0.controllers;

public class GameController
{
    public List<Player> Players { get; }
    private int currentPlayerIndex;
    private int gridSize;
    private Panel[,] cells;
    private int timeLeft;
    private int timeLimit;
    private Timer turnTimer;
    private Figure currentDraggingFigure;
    private bool isDragging;

    public GameController(int playerCount, int gridSize, int timeLimit)
    {
        Players = [];
        this.gridSize = gridSize;
        this.timeLimit = timeLimit;
        cells = new Panel[gridSize, gridSize];
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
            
        currentPlayerIndex = 0;
    }

    private void InitializeCells()
    {
        for (var row = 0; row < gridSize; row++)
        for (var col = 0; col < gridSize; col++)
        {
            cells[row, col] = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
        }
    }

    public Panel[,] GetCells() => cells;

    public Player CurrentPlayer => Players[currentPlayerIndex];

    public void NextPlayer()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % Players.Count;
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

            if (x < 0 || x >= gridSize || y < 0 || y >= gridSize)
                return false;

            if (cells[x, y].BackColor != Color.White)
                return false;
        }
                
        foreach (var pix in figure.Pixels)
        {
            var x = baseRow + pix.X;
            var y = baseCol + pix.Y;
            cells[x, y].BackColor = Color.FromArgb(pix.Color);
        }

        return true;
    }

    public void CheckAndClearLines()
    {
        var linesCleared = 0;
                
        for (var i = 0; i < gridSize; i++)
        {
            var isRowFull = true;
            var isColFull = true;

            for (var j = 0; j < gridSize; j++)
            {
                if (cells[i, j].BackColor == Color.White) isRowFull = false;
                if (cells[j, i].BackColor == Color.White) isColFull = false;
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

        for (var i = 0; i < gridSize; i++)
        {
            if (cells[i, i].BackColor == Color.White) isDiagonal1Full = false;
            if (cells[i, gridSize - i - 1].BackColor == Color.White) isDiagonal2Full = false;
        }

        if (isDiagonal1Full)
        {
            linesCleared++;
            ClearLine(0, 0, 1, 1);
        }

        if (isDiagonal2Full)
        {
            linesCleared++;
            ClearLine(0, gridSize - 1, 1, -1);
        }
                
        var score = linesCleared * 100;
        CurrentPlayer.AddScore(score);
    }

    private void ClearLine(int startX, int startY, int stepX, int stepY)
    {
        for (var i = 0; i < gridSize; i++)
        {
            var x = startX + i * stepX;
            var y = startY + i * stepY;
            cells[x, y].BackColor = Color.White;
        }
    }

    public bool CanPlayerPlaceAnyFigure(Player player) => 
        player.Figures.Any(figure => CanPlaceFigure(figure));

    private bool CanPlaceFigure(Figure figure)
    {
        for (var row = 0; row < gridSize; row++)
        for (var col = 0; col < gridSize; col++)
        {
            var canPlace = !(from pix in figure.Pixels 
                            let x = row + pix.X 
                            let y = col + pix.Y 
                            where x < 0 || x >= gridSize || y < 0 || y >= gridSize || cells[x, y].BackColor != Color.White 
                            select x).Any();
            if (canPlace) return true;
        }
        return false;
    }

    public void InitializeTimer(EventHandler tickHandler)
    {
        turnTimer = new Timer
        {
            Interval = 1000
        };
        turnTimer.Tick += tickHandler;

        if (timeLimit > 0)
        {
            timeLeft = timeLimit;
            turnTimer.Start();
        }
    }

    public void UpdateTimer()
    {
        timeLeft--;
        if (timeLeft <= 0)
        {
            turnTimer.Stop();
        }
    }

    public string GetTimeText()
    {
        return timeLimit > 0 
            ? $"Осталось времени: {timeLeft} сек." 
            : "Время на ход не ограничено";
    }

    public Color GetTimeLabelColor() => timeLeft <= 5 ? Color.Red : Color.White;

    public bool IsTimeUp() => timeLimit > 0 && timeLeft <= 0;

    public void ResetTimer()
    {
        if (timeLimit > 0)
        {
            timeLeft = timeLimit;
            turnTimer.Start();
        }
    }

    public void StopTimer()
    {
        turnTimer.Stop();
    }

    public void StartDragging(Figure figure)
    {
        currentDraggingFigure = figure;
        isDragging = true;
    }

    public void EndDragging()
    {
        currentDraggingFigure = null;
        isDragging = false;
    }

    public Figure GetCurrentDraggingFigure() => currentDraggingFigure;

    public void HighlightCells(Point clientPos, Panel fieldPanel)
    {
        var cellSize = cells[0, 0].Width + 2;
        var row = clientPos.Y / cellSize;
        var col = clientPos.X / cellSize;
                
        foreach (var cell in cells)
        {
            if (cell.BackColor == Color.LightGreen)
                cell.BackColor = Color.White;
        }
                
        if (currentDraggingFigure == null) return;
                
        foreach (var pix in currentDraggingFigure.Pixels)
        {
            var x = row + pix.X;
            var y = col + pix.Y;

            if (x < 0 || x >= gridSize || y < 0 || y >= gridSize) 
                continue;

            if (cells[x, y].BackColor == Color.White)
                cells[x, y].BackColor = Color.LightGreen;
        }
    }

    public bool TryPlaceFigure(Point clientPos, Panel fieldPanel)
    {
        if (currentDraggingFigure == null) return false;

        var cellSize = cells[0, 0].Width + 2;
        var row = clientPos.Y / cellSize;
        var col = clientPos.X / cellSize;
                
        foreach (var cell in cells)
            if (cell.BackColor == Color.LightGreen)
                cell.BackColor = Color.White;

        var placed = PlaceFigure(currentDraggingFigure, row, col);

        if (!placed) return false;

        CurrentPlayer.RemoveFigure(currentDraggingFigure);
        EndDragging();

        CheckAndClearLines();
                
        if (CurrentPlayer.Figures.Count == 0)
        {
            CurrentPlayer.GenerateFigures();
        }
            
        if (Players.Count > 1)
        {
            var canAnyPlayerMove = Players.Any(p => CanPlayerPlaceAnyFigure(p));

            if (!canAnyPlayerMove)
            {
                return true; // Game should end
            }
        }

        return false;
    }

    public string GetEndGameMessage()
    {
        if (Players.Count == 1)
        {
            return $"Игра окончена! Ваш счет: {Players[0].Score}";
        }
        
        var winner = Players.OrderByDescending(p => p.Score).FirstOrDefault();
        var winnerText = Players.GroupBy(p => p.Score).Count() == 1 
            ? "Ничья!" 
            : $"Победитель: Игрок {Players.IndexOf(winner) + 1}";

        var scores = string.Join("\n", Players.Select((p, i) => 
            $"Игрок {i + 1}: {p.Score} очков"));
                    
        return $"{winnerText}\n{scores}";
    }

    public string GetCurrentPlayerText()
    {
        var currentPlayer = CurrentPlayer;
        var playerIndex = Players.IndexOf(currentPlayer) + 1;
        var playerColor = Color.FromArgb(currentPlayer.Figures.FirstOrDefault()?.Pixels.FirstOrDefault()?.Color ?? Color.Gray.ToArgb());
        
        return $"Сейчас ходит: Игрок {playerIndex}";
    }

    public Color GetCurrentPlayerPanelColor()
    {
        var currentPlayer = CurrentPlayer;
        var playerColor = Color.FromArgb(currentPlayer.Figures.FirstOrDefault()?.Pixels.FirstOrDefault()?.Color ?? Color.Gray.ToArgb());
        return Color.FromArgb(80, playerColor);
    }

    public string GetPlayerScoreText(int playerIndex)
    {
        return $"Очки: {Players[playerIndex].Score}";
    }
}