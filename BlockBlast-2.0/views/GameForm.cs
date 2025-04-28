using BlockBlast_2._0.controllers;
using BlockBlast_2._0.models;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BlockBlast_2._0
{
    public partial class GameForm : Form
    {
        private const int GridSize = 10;
        private Panel[,] cells = new Panel[GridSize, GridSize];
        private List<Panel> playerPanels = new List<Panel>();
        private List<Panel> figurePanels = new List<Panel>();

        private GameController controller;
        private Figure currentDraggingFigure;
        private bool isDragging;
        private int playerCount;
        private System.Windows.Forms.Timer turnTimer;
        private int timeLeft;
        private int timeLimit;
        private Label timeLabel;

        public GameForm(int playerCount, int timeLimit)
        {
            this.playerCount = playerCount;
            this.timeLimit = timeLimit;
            
            InitializeComponent();
            controller = new GameController(playerCount);
            SetupPreStartSetting();
            Resize += (_, _) => UpdateLayout();
            CreateLayout();
            InitializeTimer();
            GenerateFigures();
            UpdateTurnLabel();
            UpdateScoreLabels();
        }

        private void InitializeTimer()
        {
            turnTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            turnTimer.Tick += TurnTimer_Tick;

            if (timeLimit > 0)
            {
                timeLeft = timeLimit;
                UpdateTimeLabel();
                turnTimer.Start();
            }
            else
            {
                timeLabel.Text = "Время на ход не ограничено";
            }
        }

        private void TurnTimer_Tick(object sender, EventArgs e)
        {
            timeLeft--;
            UpdateTimeLabel();

            if (timeLeft > 0) return;
            turnTimer.Stop();
            TimeUp();
        }

        private void UpdateTimeLabel()
        {
            timeLabel.Text = $"Осталось времени: {timeLeft} сек.";
            timeLabel.ForeColor = timeLeft <= 5 ? Color.Red : Color.White;
        }

        private void TimeUp()
        {
            MessageBox.Show("Время на ход истекло! Ход переходит к следующему игроку.");
            EndTurn();
        }

        private void SetupPreStartSetting()
        {
            Text = $"BlockBlast 2.0 - {playerCount} игрока(ов)";
            ClientSize = new Size(1280, 720);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.DimGray;
        }

        private void CreateLayout()
        {
            var titleLabel = new Label
            {
                Text = $"BlockBlast 2.0 - {playerCount} игрока(ов)",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60
            };
            Controls.Add(titleLabel);

            timeLabel = new Label
            {
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(70, 70, 70)
            };
            Controls.Add(timeLabel);

            var currentPlayerPanel = new Panel
            {
                Name = "currentPlayerPanel",
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(80, 80, 80)
            };

            var currentPlayerLabel = new Label
            {
                Name = "currentPlayerLabel",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White
            };
            currentPlayerPanel.Controls.Add(currentPlayerLabel);
            Controls.Add(currentPlayerPanel);

            CreatePlayerPanels();
            
            var fieldPanel = new Panel 
            { 
                Name = "fieldPanel",
                BackColor = Color.LightSlateGray,
                AllowDrop = true
            };
            fieldPanel.DragEnter += (_, e) => e.Effect = DragDropEffects.Copy;
            fieldPanel.DragOver += FieldPanel_DragOver;
            fieldPanel.DragDrop += FieldPanel_DragDrop;
            Controls.Add(fieldPanel);
            
            for (var row = 0; row < GridSize; row++)
            for (var col = 0; col < GridSize; col++)
            {
                var cell = new Panel
                {
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };
                fieldPanel.Controls.Add(cell);
                cells[row, col] = cell;
            }

            UpdateLayout();
        }

        private void CreatePlayerPanels()
        {
            foreach (var panel in playerPanels)
            {
                Controls.Remove(panel);
                panel.Dispose();
            }
            playerPanels.Clear();

            for (var i = 0; i < playerCount; i++)
            {
                var panel = new Panel 
                { 
                    BackColor = Color.FromArgb(40, 40, 40),
                    Width = 200,
                };

                var label = new Label
                {
                    Text = $"Игрок {i + 1}",
                    Dock = DockStyle.Top,
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Height = 40
                };

                var figuresPanel = new Panel
                {
                    Name = $"figuresPanel{i}",
                    Dock = DockStyle.Fill,
                    Padding = new Padding(10),
                    AutoScroll = true
                };

                var scoreLabel = new Label
                {
                    Name = $"scoreLabel{i}",
                    Text = "Очки: 0",
                    Dock = DockStyle.Bottom,
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 12),
                    Height = 40
                };

                panel.Controls.Add(label);
                panel.Controls.Add(figuresPanel);
                panel.Controls.Add(scoreLabel);

                Controls.Add(panel);
                playerPanels.Add(panel);
            }
        }

        private void UpdateLayout()
        {
            var totalWidth = ClientSize.Width;
            var totalHeight = ClientSize.Height;
            
            var topPanelHeight = Controls.OfType<Label>().First(l => l.Text.StartsWith("BlockBlast")).Height 
                               + timeLabel.Height 
                               + Controls.Find("currentPlayerPanel", true).First().Height;

            var fieldPanel = Controls.Find("fieldPanel", true).FirstOrDefault() as Panel;
            if (fieldPanel == null) return;
            
            var fieldSize = Math.Min(
                totalHeight - topPanelHeight - 20,
                (int)(totalWidth * 0.6));
            
            fieldPanel.Size = new Size(fieldSize, fieldSize);
            fieldPanel.Location = new Point(
                (totalWidth - fieldSize) / 2,
                topPanelHeight + 10);

            var playerPanelHeight = fieldSize / 2;
            var playerPanelWidth = (totalWidth - fieldSize) / 2;

            if (playerCount == 4)
            {
                playerPanels[0].Size = new Size(playerPanelWidth, playerPanelHeight);
                playerPanels[0].Location = new Point(0, topPanelHeight + 10);
                
                playerPanels[2].Size = new Size(playerPanelWidth, playerPanelHeight);
                playerPanels[2].Location = new Point(0, topPanelHeight + 10 + playerPanelHeight);

                playerPanels[1].Size = new Size(playerPanelWidth, playerPanelHeight);
                playerPanels[1].Location = new Point(totalWidth - playerPanelWidth, topPanelHeight + 10);
                
                playerPanels[3].Size = new Size(playerPanelWidth, playerPanelHeight);
                playerPanels[3].Location = new Point(totalWidth - playerPanelWidth, topPanelHeight + 10 + playerPanelHeight);
            }
            else
            {
                for (var i = 0; i < playerCount; i++)
                {
                    playerPanels[i].Size = new Size(playerPanelWidth, fieldSize);
                    playerPanels[i].Location = new Point(
                        i % 2 == 0 ? 0 : totalWidth - playerPanelWidth,
                        topPanelHeight + 10);
                }
            }

            var cellSize = fieldSize / GridSize;
            
            for (var row = 0; row < GridSize; row++)
            for (var col = 0; col < GridSize; col++)
            {
                var cell = cells[row, col];
                cell.Size = new Size(cellSize - 2, cellSize - 2);
                cell.Location = new Point(col * cellSize, row * cellSize);
            }
        }

        private void UpdateTurnLabel()
        {
            var currentPlayerPanel = Controls.Find("currentPlayerPanel", true).FirstOrDefault() as Panel;
            var currentPlayerLabel = currentPlayerPanel?.Controls.Find("currentPlayerLabel", true).FirstOrDefault() as Label;
            if (currentPlayerLabel == null) return;

            var currentPlayer = controller.CurrentPlayer;
            var playerIndex = controller.Players.IndexOf(currentPlayer) + 1;
            var playerColor = Color.FromArgb(currentPlayer.Figures.FirstOrDefault()?.Pixels.FirstOrDefault()?.Color ?? Color.Gray.ToArgb());
            
            currentPlayerLabel.Text = $"Сейчас ходит: Игрок {playerIndex}";
            currentPlayerPanel.BackColor = Color.FromArgb(80, playerColor);
        }

        private void UpdateScoreLabels()
        {
            for (var i = 0; i < controller.Players.Count; i++)
            {
                if (Controls.Find($"scoreLabel{i}", true).FirstOrDefault() is Label scoreLabel)
                {
                    scoreLabel.Text = $"Очки: {controller.Players[i].Score}";
                }
            }
        }

        private void GenerateFigures()
        {
            controller.GenerateFigures();
            DrawPlayerFigures();
        }

        private void DrawPlayerFigures()
        {
            foreach (var panel in figurePanels)
            {
                panel.Dispose();
            }
            figurePanels.Clear();

            for (var i = 0; i < controller.Players.Count; i++)
            {
                if (Controls.Find($"figuresPanel{i}", true).FirstOrDefault() is Panel figuresPanel)
                {
                    DrawFiguresForPlayer(controller.Players[i], figuresPanel);
                }
            }
        }

        private void DrawFiguresForPlayer(Player player, Panel targetPanel)
        {
            targetPanel.Controls.Clear();

            var figures = player.Figures;
            if (figures.Count == 0) return;

            const int baseFigureSize = 180;
            var figureHeight = Math.Min(
                baseFigureSize, 
                (targetPanel.Height - 20) / Math.Max(3, figures.Count));

            for (var i = 0; i < figures.Count; i++)
            {
                var fig = figures[i];
                var figPanel = CreateFigurePanel(fig, figureHeight, player);
                figPanel.Top = i * (figureHeight + 10) + 10;
                targetPanel.Controls.Add(figPanel);
                figurePanels.Add(figPanel);
            }
        }

        private Panel CreateFigurePanel(Figure fig, int size, Player player)
        {
            var figPanel = new Panel
            {
                Size = new Size(size, size),
                Tag = new Tuple<Figure, Player>(fig, player),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Margin = new Padding(10)
            };

            DrawFigureIntoPanel(figPanel, fig, size, player);

            return figPanel;
        }

        private void DrawFigureIntoPanel(Panel panel, Figure fig, int panelSize, Player player)
        {
            panel.Controls.Clear();

            if (fig.Pixels.Length == 0) return;

            var minX = fig.Pixels.Min(p => p.X);
            var minY = fig.Pixels.Min(p => p.Y);
            var maxX = fig.Pixels.Max(p => p.X);
            var maxY = fig.Pixels.Max(p => p.Y);

            var figWidth = maxY - minY + 1;
            var figHeight = maxX - minX + 1;

            var pixelSize = Math.Min(panelSize / Math.Max(figWidth, figHeight), 45);

            var offsetX = (panel.Width - figWidth * pixelSize) / 2;
            var offsetY = (panel.Height - figHeight * pixelSize) / 2;

            var figureContainer = new Panel
            {
                Size = new Size(figWidth * pixelSize, figHeight * pixelSize),
                Location = new Point(offsetX, offsetY),
                BackColor = Color.Transparent,
                Tag = new Tuple<Figure, Player>(fig, player)
            };
            
            figureContainer.MouseDown += (sender, e) =>
            {
                if (e.Button != MouseButtons.Left || figureContainer.Tag is not Tuple<Figure, Player> tuple) return;
                if (controller.CurrentPlayer != tuple.Item2) return;
                currentDraggingFigure = tuple.Item1;
                isDragging = true;
                panel.DoDragDrop(tuple.Item1, DragDropEffects.Copy);
            };

            foreach (var pix in fig.Pixels)
            {
                var p = new Panel
                {
                    Size = new Size(pixelSize - 2, pixelSize - 2),
                    BackColor = Color.FromArgb(pix.Color),
                    Location = new Point(
                        (pix.Y - minY) * pixelSize,
                        (pix.X - minX) * pixelSize),
                    Cursor = Cursors.Hand
                };

                p.MouseDown += (sender, e) =>
                {
                    if (e.Button == MouseButtons.Left && figureContainer.Tag is Tuple<Figure, Player> tuple)
                    {
                        if (controller.CurrentPlayer == tuple.Item2)
                        {
                            currentDraggingFigure = tuple.Item1;
                            isDragging = true;
                            panel.DoDragDrop(tuple.Item1, DragDropEffects.Copy);
                        }
                    }
                };

                figureContainer.Controls.Add(p);
            }

            panel.Controls.Add(figureContainer);
        }

        private void FieldPanel_DragOver(object sender, DragEventArgs e)
        {
            var fieldPanel = Controls.Find("fieldPanel", true).FirstOrDefault() as Panel;
            if (fieldPanel == null) return;

            var clientPos = fieldPanel.PointToClient(new Point(e.X, e.Y));
            var cellSize = cells[0, 0].Width + 2;

            var row = clientPos.Y / cellSize;
            var col = clientPos.X / cellSize;
                
            foreach (var cell in cells)
            {
                if (cell.BackColor == Color.LightGreen)
                    cell.BackColor = Color.White;
            }
                
            foreach (var pix in currentDraggingFigure.Pixels)
            {
                var x = row + pix.X;
                var y = col + pix.Y;

                if (x < 0 || x >= GridSize || y < 0 || y >= GridSize) 
                    continue;

                if (cells[x, y].BackColor == Color.White)
                    cells[x, y].BackColor = Color.LightGreen;
            }
        }

        private void FieldPanel_DragDrop(object sender, DragEventArgs e)
        {
            var fieldPanel = Controls.Find("fieldPanel", true).FirstOrDefault() as Panel;
            if (fieldPanel == null) return;

            var clientPos = fieldPanel.PointToClient(new Point(e.X, e.Y));
            var cellSize = cells[0, 0].Width + 2;

            var row = clientPos.Y / cellSize;
            var col = clientPos.X / cellSize;
                
            foreach (var cell in cells)
                if (cell.BackColor == Color.LightGreen)
                    cell.BackColor = Color.White;

            var placed = GameController.PlaceFigure(currentDraggingFigure, row, col, cells, GridSize);

            if (!placed) return;

            controller.CurrentPlayer.RemoveFigure(currentDraggingFigure);
            currentDraggingFigure = null;
            isDragging = false;

            DrawPlayerFigures();

            controller.CheckAndClearLines(cells, GridSize);
            UpdateScoreLabels();
                
            if (controller.CurrentPlayer.Figures.Count == 0)
            {
                controller.CurrentPlayer.GenerateFigures();
                DrawPlayerFigures();
            }
            
            if (controller.Players.Count > 1)
            {
                var canAnyPlayerMove = controller.Players.Any(p =>
                    GameController.CanPlayerPlaceAnyFigure(p, cells, GridSize));

                if (!canAnyPlayerMove)
                {
                    EndGame();
                    return;
                }
            }

            EndTurn();
        }

        private void EndGame()
        {
            turnTimer.Stop();

            string winnerText;
            if (controller.Players.Count == 1)
            {
                winnerText = $"Игра окончена! Ваш счет: {controller.Players[0].Score}";
            }
            else
            {
                var winner = controller.Players
                    .OrderByDescending(p => p.Score)
                    .FirstOrDefault();

                winnerText = controller.Players
                    .GroupBy(p => p.Score)
                    .Count() == 1 
                    ? "Ничья!" 
                    : $"Победитель: Игрок {controller.Players.IndexOf(winner) + 1}";

                var scores = string.Join("\n", controller.Players.Select((p, i) => 
                    $"Игрок {i + 1}: {p.Score} очков"));
                    
                winnerText += $"\n{scores}";
            }

            MessageBox.Show(winnerText,
                "Конец игры",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
                
            Close();
        }

        private void EndTurn()
        {
            if (timeLimit > 0)
            {
                timeLeft = timeLimit;
                turnTimer.Start();
                UpdateTimeLabel();
            }
                
            controller.NextPlayer();
            UpdateTurnLabel();

            if (GameController.CanPlayerPlaceAnyFigure(controller.CurrentPlayer, cells, GridSize)) return;
            MessageBox.Show("Игрок не может сделать ход. Ход переходит к следующему игроку.");
            controller.NextPlayer();
            UpdateTurnLabel();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            turnTimer.Stop();
            turnTimer.Dispose();
        }
    }
}