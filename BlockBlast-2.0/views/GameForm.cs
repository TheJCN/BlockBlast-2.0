using BlockBlast_2._0.controllers;
using BlockBlast_2._0.model;

namespace BlockBlast_2._0;

public partial class GameForm : Form
{
    private const int GridSize = 10;
    private Panel[,] cells = new Panel[GridSize, GridSize];

    private Panel fieldPanel;
    private Label titleLabel;
    private Label turnLabel;
    private Panel leftPanel;
    private Panel rightPanel;
    private Panel leftFiguresPanel;
    private Panel rightFiguresPanel;

    private GameController controller;

    private Figure? currentDraggingFigure;
    private bool isDragging;

    public GameForm()
    {
        InitializeComponent();
        controller = new GameController();
        SetupPreStartSetting();
        Resize += (_, _) => UpdateLayout();
        CreateLayout();
        GenerateFigures();
        UpdateTurnLabel();
    }

    private void SetupPreStartSetting()
    {
        Text = "BlockBlast 2.0";
        ClientSize = new Size(1280, 720);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.DimGray;
    }

    private void CreateLayout()
    {
        titleLabel = new Label
        {
            Text = "BlockBlast 2.0",
            Font = new Font("Segoe UI", 24, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Top,
            Height = 60
        };
        Controls.Add(titleLabel);
        
        turnLabel = new Label
        {
            Text = "Ход: Игрок 1",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Top,
            Height = 40,
            BackColor = Color.FromArgb(70, 70, 70)
        };
        Controls.Add(turnLabel);
        
        leftPanel = new Panel { BackColor = Color.FromArgb(40, 40, 40), Dock = DockStyle.Left };
        Controls.Add(leftPanel);

        var leftLabel = new Label
        {
            Text = "Игрок 1",
            Dock = DockStyle.Top,
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Height = 40
        };
        leftPanel.Controls.Add(leftLabel);

        leftFiguresPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            AutoScroll = true
        };
        leftPanel.Controls.Add(leftFiguresPanel);

        var leftScoreLabel = new Label
        {
            Text = "Очки: 0",
            Dock = DockStyle.Bottom,
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 12),
            Height = 40
        };
        leftPanel.Controls.Add(leftScoreLabel);
        
        rightPanel = new Panel { BackColor = Color.FromArgb(40, 40, 40), Dock = DockStyle.Right };
        Controls.Add(rightPanel);

        var rightLabel = new Label
        {
            Text = "Игрок 2",
            Dock = DockStyle.Top,
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Height = 40
        };
        rightPanel.Controls.Add(rightLabel);

        rightFiguresPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            AutoScroll = true
        };
        rightPanel.Controls.Add(rightFiguresPanel);

        var rightScoreLabel = new Label
        {
            Text = "Очки: 0",
            Dock = DockStyle.Bottom,
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 12),
            Height = 40
        };
        rightPanel.Controls.Add(rightScoreLabel);
        
        fieldPanel = new Panel { BackColor = Color.LightSlateGray };
        fieldPanel.AllowDrop = true;
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

    private void UpdateLayout()
    {
        var totalWidth = ClientSize.Width;
        var totalHeight = ClientSize.Height;
        
        var sidePanelWidth = (int)(totalWidth * 0.25);
        leftPanel.Width = sidePanelWidth;
        rightPanel.Width = sidePanelWidth;
        
        
        var fieldSize = Math.Min(totalHeight - titleLabel.Height - turnLabel.Height - 40, 
                                totalWidth - leftPanel.Width - rightPanel.Width - 40);
        fieldPanel.Size = new Size(fieldSize, fieldSize);
        fieldPanel.Location = new Point(
            leftPanel.Width + (totalWidth - leftPanel.Width - rightPanel.Width - fieldSize) / 2,
            titleLabel.Height + turnLabel.Height + 20);
        
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
        var currentPlayer = controller.GetCurrentPlayer();
        turnLabel.Text = currentPlayer == controller.Player1 ? "Ход: Игрок 1 (Синий)" : "Ход: Игрок 2 (Красный)";
        turnLabel.BackColor = currentPlayer == controller.Player1 ? Color.FromArgb(0, 50, 100) : Color.FromArgb(100, 50, 0);
    }

    private void GenerateFigures()
    {
        controller.GenerateFigures();
        DrawPlayerFigures();
    }

    private void DrawPlayerFigures()
    {
        DrawFiguresForPlayer(controller.Player1, leftFiguresPanel);
        DrawFiguresForPlayer(controller.Player2, rightFiguresPanel);
    }

    private void DrawFiguresForPlayer(Player player, Panel targetPanel)
    {
        targetPanel.Controls.Clear();

        var figures = player.Figures;
        if (figures.Count == 0) return;
        
        const int baseFigureSize = 180; 
        var figureHeight = Math.Min(baseFigureSize, (targetPanel.Height - 20) / Math.Max(3, figures.Count));

        for (var i = 0; i < figures.Count; i++)
        {
            var fig = figures[i];
            var figPanel = CreateFigurePanel(fig, figureHeight, player);
            figPanel.Top = i * (figureHeight + 10) + 10;
            targetPanel.Controls.Add(figPanel);
        }
    }

    private Panel CreateFigurePanel(Figure fig, int size, Player player)
    {
        var figPanel = new Panel
        {
            Size = new Size(size, size),
            Tag = fig,
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand,
            Margin = new Padding(10)
        };
        
        var hitBoxPanel = new Panel
        {
            Size = figPanel.Size,
            BackColor = Color.Transparent,
            Dock = DockStyle.Fill
        };
        figPanel.Controls.Add(hitBoxPanel);

        DrawFigureIntoPanel(figPanel, fig, size);
        
        hitBoxPanel.MouseEnter += (_, _) =>
        {
            figPanel.Size = new Size((int)(size * 1.1), (int)(size * 1.1));
            DrawFigureIntoPanel(figPanel, fig, (int)(size * 1.1));
        };

        hitBoxPanel.MouseLeave += (_, _) =>
        {
            figPanel.Size = new Size(size, size);
            DrawFigureIntoPanel(figPanel, fig, size);
        };

        hitBoxPanel.MouseDown += (_, e) =>
        {
            if (e.Button != MouseButtons.Left || controller.GetCurrentPlayer() != player) return;
            currentDraggingFigure = fig;
            isDragging = true;
            figPanel.DoDragDrop(fig, DragDropEffects.Copy);
        };

        return figPanel;
    }

    private void DrawFigureIntoPanel(Panel panel, Figure fig, int panelSize)
    {
        panel.Controls.OfType<Panel>().Where(p => p.Tag as string == "pixel").ToList().ForEach(p => p.Dispose());

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
        
        foreach (var pix in fig.Pixels)
        {
            var p = new Panel
            {
                Size = new Size(pixelSize - 2, pixelSize - 2),
                BackColor = Color.FromArgb(pix.Color),
                Location = new Point(
                    offsetX + (pix.Y - minY) * pixelSize,
                    offsetY + (pix.X - minX) * pixelSize),
                Tag = "pixel"
            };
            panel.Controls.Add(p);
            p.BringToFront();
        }
    }

    private void FieldPanel_DragOver(object? sender, DragEventArgs e)
    {
        if (currentDraggingFigure == null) return;

        var clientPos = fieldPanel.PointToClient(new Point(e.X, e.Y));
        var cellSize = cells[0, 0].Width + 2;

        var row = clientPos.Y / cellSize;
        var col = clientPos.X / cellSize;
        
        foreach (var cell in cells)
            if (cell.BackColor == Color.LightGreen)
                cell.BackColor = Color.White;
        
        foreach (var pix in currentDraggingFigure.Pixels)
        {
            var x = row + pix.X;
            var y = col + pix.Y;

            if (x < 0 || x >= GridSize || y < 0 || y >= GridSize) continue;

            if (cells[x, y].BackColor == Color.White)
                cells[x, y].BackColor = Color.LightGreen;
        }
    }

    private void FieldPanel_DragDrop(object? sender, DragEventArgs e)
    {
        if (currentDraggingFigure == null) return;

        var clientPos = fieldPanel.PointToClient(new Point(e.X, e.Y));
        var cellSize = cells[0, 0].Width + 2;

        var row = clientPos.Y / cellSize;
        var col = clientPos.X / cellSize;
        
        foreach (var cell in cells)
            if (cell.BackColor == Color.LightGreen)
                cell.BackColor = Color.White;
        
        var placed = controller.PlaceFigure(currentDraggingFigure, row, col, cells, GridSize);

        if (!placed) return;
        controller.GetCurrentPlayer().RemoveFigure(currentDraggingFigure);
        currentDraggingFigure = null;
        isDragging = false;
            
        DrawPlayerFigures();
            
        var score = controller.CheckAndClearLines(cells, GridSize);
        controller.GetCurrentPlayer().AddScore(score);
            
        if (controller.AreAllFiguresPlaced())
        {
            controller.AwardAdditionalFigures();
            DrawPlayerFigures();
        }
            
        controller.SwitchPlayer();
        UpdateTurnLabel();
    }
}