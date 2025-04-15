using BlockBlast_2._0.controllers;
using BlockBlast_2._0.model;

namespace BlockBlast_2._0;

public partial class GameForm : Form
{
    private const int GridSize = 10;
    private Panel[,] cells = new Panel[GridSize, GridSize];

    private Panel fieldPanel;
    private Label titleLabel;
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
            Height = 80
        };
        Controls.Add(titleLabel);

        leftPanel = new Panel { BackColor = Color.FromArgb(40, 40, 40) };
        Controls.Add(leftPanel);

        var leftLabel = new Label
        {
            Text = "Игрок 1",
            Dock = DockStyle.Top,
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter
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
            TextAlign = ContentAlignment.MiddleCenter
        };
        leftPanel.Controls.Add(leftScoreLabel);

        rightPanel = new Panel { BackColor = Color.FromArgb(40, 40, 40) };
        Controls.Add(rightPanel);

        var rightLabel = new Label
        {
            Text = "Игрок 2",
            Dock = DockStyle.Top,
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter
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
            TextAlign = ContentAlignment.MiddleCenter
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

        var topHeight = titleLabel.Height;
        var centerHeight = totalHeight - topHeight;

        var leftWidth = (int)(totalWidth * 0.3);
        var rightWidth = (int)(totalWidth * 0.3);
        var centerWidth = totalWidth - leftWidth - rightWidth;

        titleLabel.Location = new Point(0, 0);
        titleLabel.Size = new Size(totalWidth, topHeight);

        leftPanel.Location = new Point(0, topHeight);
        leftPanel.Size = new Size(leftWidth, centerHeight);

        rightPanel.Location = new Point(totalWidth - rightWidth, topHeight);
        rightPanel.Size = new Size(rightWidth, centerHeight);

        var cellSize = Math.Min(centerWidth, centerHeight) / (GridSize + 2);
        var fieldWidth = cellSize * GridSize;
        var fieldHeight = cellSize * GridSize;

        fieldPanel.Size = new Size(fieldWidth, fieldHeight);
        fieldPanel.Location = new Point(leftWidth + (centerWidth - fieldWidth) / 2, topHeight + (centerHeight - fieldHeight) / 2);

        for (var row = 0; row < GridSize; row++)
        for (var col = 0; col < GridSize; col++)
        {
            var cell = cells[row, col];
            cell.Size = new Size(cellSize - 2, cellSize - 2);
            cell.Location = new Point(col * cellSize, row * cellSize);
        }
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
        foreach (var control in targetPanel.Controls.OfType<Panel>().ToList())
        {
            targetPanel.Controls.Remove(control);
            control.Dispose();
        }

        var figures = player.Figures;

        for (var i = 0; i < figures.Count; i++)
        {
            var fig = figures[i];

            var figPanel = new Panel
            {
                Size = new Size(120, 120),
                Location = new Point(10, i * 130),
                Tag = fig,
                BackColor = Color.Transparent
            };

            DrawFigureIntoPanel(figPanel, fig, scale: 25);

            // Наведение мышки — увеличиваем
            figPanel.MouseEnter += (_, _) =>
            {
                figPanel.Size = new Size(140, 140);
                DrawFigureIntoPanel(figPanel, fig, scale: 30);
            };

            // Уход мышки — возвращаем размер
            figPanel.MouseLeave += (_, _) =>
            {
                figPanel.Size = new Size(120, 120);
                DrawFigureIntoPanel(figPanel, fig, scale: 25);
            };

            // Drag & Drop
            figPanel.MouseDown += (_, _) =>
            {
                if (controller.GetCurrentPlayer() != player) return;
                currentDraggingFigure = fig;
                isDragging = true;
                figPanel.DoDragDrop(fig, DragDropEffects.Copy);
            };

            targetPanel.Controls.Add(figPanel);
        }
    }

    private void DrawFigureIntoPanel(Panel panel, Figure fig, int scale)
    {
        panel.Controls.Clear();

        // Вычисляем границы фигуры
        var minX = fig.Pixels.Min(p => p.X);
        var minY = fig.Pixels.Min(p => p.Y);
        var maxX = fig.Pixels.Max(p => p.X);
        var maxY = fig.Pixels.Max(p => p.Y);

        var figWidth = (maxY - minY + 1) * scale;
        var figHeight = (maxX - minX + 1) * scale;

        var offsetX = (panel.Width - figWidth) / 2;
        var offsetY = (panel.Height - figHeight) / 2;

        foreach (var pix in fig.Pixels)
        {
            var p = new Panel
            {
                Size = new Size(scale - 2, scale - 2),
                BackColor = Color.FromArgb(pix.Color),
                Location = new Point(offsetX + (pix.Y - minY) * scale, offsetY + (pix.X - minX) * scale)
            };
            panel.Controls.Add(p);
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
            {
                cells[x, y].BackColor = Color.LightGreen;
            }
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
        
        if (controller.AreAllFiguresPlaced())
        {
            controller.AwardAdditionalFigures();
            DrawPlayerFigures();
        }

        var score = controller.CheckAndClearLines(cells, GridSize);
        controller.GetCurrentPlayer().AddScore(score);
        Console.WriteLine(controller.GetCurrentPlayer().Score);
        controller.SwitchPlayer();
    }
}