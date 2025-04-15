using BlockBlast_2._0.model;

namespace BlockBlast_2._0.controllers;

public class GameController
{
    public Player Player1 { get; }
    public Player Player2 { get; }
    private int _currentPlayer;

    public GameController()
    {
        Player1 = new Player(Color.Blue.ToArgb(), 0);
        Player2 = new Player(Color.Red.ToArgb(), 0);
        _currentPlayer = 1;
    }

    public Player GetCurrentPlayer() => _currentPlayer == 1 ? Player1 : Player2;

    public void SwitchPlayer()
    {
        _currentPlayer = _currentPlayer == 1 ? 2 : 1;
    }

    public void GenerateFigures()
    {
        Player1.GenerateFigures(Color.Blue.ToArgb());
        Player2.GenerateFigures(Color.Red.ToArgb());
    }

    public bool PlaceFigure(Figure figure, int baseRow, int baseCol, Panel[,] cells, int gridSize)
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

    public int CheckAndClearLines(Panel[,] cells, int gridSize)
    {
        var score = 0;

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
                score += 100;
                ClearLine(i, 0, 0, 1, cells, gridSize);
            }

            if (isColFull)
            {
                score += 100;
                ClearLine(0, i, 1, 0, cells, gridSize);
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
            score += 100;
            ClearLine(0, 0, 1, 1, cells, gridSize);
        }

        if (isDiagonal2Full)
        {
            score += 100;
            ClearLine(0, gridSize - 1, 1, -1, cells, gridSize);
        }

        return score;
    }

    private void ClearLine(int startX, int startY, int stepX, int stepY, Panel[,] cells, int gridSize)
    {
        for (var i = 0; i < gridSize; i++)
        {
            var x = startX + i * stepX;
            var y = startY + i * stepY;
            cells[x, y].BackColor = Color.White;
        }
    }

    public bool AreAllFiguresPlaced() => Player1.Figures.Count == 0 && Player2.Figures.Count == 0;

    public void AwardAdditionalFigures()
    {
        Player1.GenerateFigures(Color.Blue.ToArgb());
        Player2.GenerateFigures(Color.Red.ToArgb());
    }
}