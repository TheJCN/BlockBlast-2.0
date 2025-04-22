using BlockBlast_2._0.model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BlockBlast_2._0.controllers
{
    public class GameController
    {
        public List<Player> Players { get; }
        private int _currentPlayerIndex;

        public GameController(int playerCount)
        {
            Players = new List<Player>();
            
            var colors = new[]
            {
                Color.Blue, Color.Red, Color.Green, Color.Yellow
            };

            for (int i = 0; i < playerCount; i++)
            {
                int playerColor = colors[i].ToArgb();
                Players.Add(new Player(playerColor, 0));
            }
            
            _currentPlayerIndex = 0;
        }

        public Player CurrentPlayer => Players[_currentPlayerIndex];

        public void NextPlayer()
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % Players.Count;
        }

        public void GenerateFigures()
        {
            foreach (var player in Players)
            {
                player.GenerateFigures();
            }
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
                    ClearLine(i, 0, 0, 1, cells, gridSize);
                }

                if (isColFull)
                {
                    linesCleared++;
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
                linesCleared++;
                ClearLine(0, 0, 1, 1, cells, gridSize);
            }

            if (isDiagonal2Full)
            {
                linesCleared++;
                ClearLine(0, gridSize - 1, 1, -1, cells, gridSize);
            }
                
            var score = linesCleared * 100;
            CurrentPlayer.AddScore(score);
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

        public bool AreAllFiguresPlaced()
        {
            if (Players.Count == 1)
                return false;
                
            return Players.All(p => p.Figures.Count == 0);
        }

        public bool CanPlayerPlaceAnyFigure(Player player, Panel[,] cells, int gridSize)
        {
            return player.Figures.Any(figure => CanPlaceFigure(figure, cells, gridSize));
        }

        private bool CanPlaceFigure(Figure figure, Panel[,] cells, int gridSize)
        {
            for (var row = 0; row < gridSize; row++)
            for (var col = 0; col < gridSize; col++)
            {
                var canPlace = true;
                foreach (var pix in figure.Pixels)
                {
                    var x = row + pix.X;
                    var y = col + pix.Y;

                    if (x >= 0 && x < gridSize && y >= 0 && y < gridSize &&
                        cells[x, y].BackColor == Color.White) continue;
                    canPlace = false;
                    break;
                }
                if (canPlace) return true;
            }
            return false;
        }
    }
}