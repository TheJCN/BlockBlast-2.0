using BlockBlast_2._0.controllers;
using BlockBlast_2._0.models;

namespace BlockBlast_2._0;

public partial class GameForm : Form
{
    private const int GridSize = 10;
    private List<Panel> playerPanels = [];
    private List<Panel> figurePanels = [];
    private GameController controller;
    private int playerCount;
    private Label timeLabel;

    public GameForm(int playerCount, int timeLimit)
    {
        this.playerCount = playerCount;
            
        InitializeComponent();
        controller = new GameController(playerCount, GridSize, timeLimit);
        SetupPreStartSetting();
        Resize += (_, _) =>
        {
            UpdateLayout();
            RedrawPlayerFigures();
        };
        CreateLayout();
        InitializeTimer();
        GenerateFigures();
        UpdateTurnLabel();
        UpdateScoreLabels();
    }

    private void InitializeTimer()
    {
        controller.InitializeTimer(TurnTimer_Tick);
        timeLabel.Text = controller.GetTimeText();
        timeLabel.ForeColor = controller.GetTimeLabelColor();
    }

    private void TurnTimer_Tick(object sender, EventArgs e)
    {
        controller.UpdateTimer();
        timeLabel.Text = controller.GetTimeText();
        timeLabel.ForeColor = controller.GetTimeLabelColor();

        if (!controller.IsTimeUp()) return;
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
            BackColor = controller.GetCurrentPlayerPanelColor()
        };

        var currentPlayerLabel = new Label
        {
            Name = "currentPlayerLabel",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.White,
            Text = controller.GetCurrentPlayerText()
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
            
        // Add cells to the field panel
        var cells = controller.GetCells();
        for (var row = 0; row < GridSize; row++)
        for (var col = 0; col < GridSize; col++)
        {
            fieldPanel.Controls.Add(cells[row, col]);
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
                Text = controller.GetPlayerScoreText(i),
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
        var cells = controller.GetCells();
            
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
            
        currentPlayerLabel.Text = controller.GetCurrentPlayerText();
        currentPlayerPanel.BackColor = controller.GetCurrentPlayerPanelColor();
    }

    private void UpdateScoreLabels()
    {
        for (var i = 0; i < playerCount; i++)
            if (Controls.Find($"scoreLabel{i}", true).FirstOrDefault() is Label scoreLabel)
                scoreLabel.Text = controller.GetPlayerScoreText(i);
    }

    private void GenerateFigures()
    {
        controller.GenerateFigures();
        DrawPlayerFigures();
    }

    private void DrawPlayerFigures()
    {
        foreach (var panel in figurePanels)
            panel.Dispose();
        
        figurePanels.Clear();

        for (var i = 0; i < playerCount; i++)
            if (Controls.Find($"figuresPanel{i}", true).FirstOrDefault() is Panel figuresPanel)
                DrawFiguresForPlayer(controller.Players[i], figuresPanel);
    }

    private void DrawFiguresForPlayer(Player player, Panel targetPanel)
    {
        targetPanel.Controls.Clear();

        var figures = player.Figures;
        if (figures.Count == 0) return;

        const int baseFigureSize = 120;
        var figureHeight = Math.Min(
            baseFigureSize, 
            (targetPanel.Height - 20) / Math.Max(3, figures.Count));

        var totalFiguresHeight = figures.Count * (figureHeight + 10) - 10;
        var startY = Math.Max(10, (targetPanel.Height - totalFiguresHeight) / 2);

        for (var i = 0; i < figures.Count; i++)
        {
            var fig = figures[i];
            var figPanel = CreateFigurePanel(fig, figureHeight, player);
            figPanel.Top = startY + i * (figureHeight + 10);
            figPanel.Left = (targetPanel.Width - figPanel.Width) / 2;
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

    private void RedrawPlayerFigures()
    {
        for (var i = 0; i < playerCount; i++)
            if (Controls.Find($"figuresPanel{i}", true).FirstOrDefault() is Panel figuresPanel)
                DrawFiguresForPlayer(controller.Players[i], figuresPanel);
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

        var pixelSize = Math.Min(panelSize / Math.Max(figWidth, figHeight), 30);

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
            controller.StartDragging(tuple.Item1);
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
                if (e.Button != MouseButtons.Left || figureContainer.Tag is not Tuple<Figure, Player> tuple) return;
                if (controller.CurrentPlayer != tuple.Item2) return;
                controller.StartDragging(tuple.Item1);
                panel.DoDragDrop(tuple.Item1, DragDropEffects.Copy);
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
        controller.HighlightCells(clientPos, fieldPanel);
    }

    private void FieldPanel_DragDrop(object sender, DragEventArgs e)
    {
        var fieldPanel = Controls.Find("fieldPanel", true).FirstOrDefault() as Panel;
        if (fieldPanel == null) return;

        var clientPos = fieldPanel.PointToClient(new Point(e.X, e.Y));
        var shouldEndGame = controller.TryPlaceFigure(clientPos, fieldPanel);

        if (shouldEndGame)
        {
            EndGame();
            return;
        }

        DrawPlayerFigures();
        UpdateScoreLabels();
        EndTurn();
    }

    private void EndGame()
    {
        controller.StopTimer();
        MessageBox.Show(controller.GetEndGameMessage(),
            "Конец игры",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
                
        Close();
    }

    private void EndTurn()
    {
        controller.ResetTimer();
        timeLabel.Text = controller.GetTimeText();
        timeLabel.ForeColor = controller.GetTimeLabelColor();
                
        controller.NextPlayer();
        UpdateTurnLabel();

        if (controller.CanPlayerPlaceAnyFigure(controller.CurrentPlayer)) return;
        MessageBox.Show("Игрок не может сделать ход. Ход переходит к следующему игроку.");
        controller.NextPlayer();
        UpdateTurnLabel();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        controller.StopTimer();
    }
}