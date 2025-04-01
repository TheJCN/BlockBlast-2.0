namespace BlockBlast
{
    public partial class GameView : Form
    {
        private GameModel _gameModel;
        private GameController _gameController;
        private Point _dragStartPoint;
        private Point _currentDragPoint;
        private Figure _draggingFigure;

        public GameView()
        {
            InitializeComponent();
            _gameModel = new GameModel(10);
            _gameController = new GameController(_gameModel);
            DoubleBuffered = true;
            MouseDown += BoardPanel_MouseDown;
            MouseMove += BoardPanel_MouseMove;
            MouseUp += BoardPanel_MouseUp;
            Paint += GameView_Paint;
        }

        private void GameView_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.DrawString($"Счет: {_gameModel.Score}", new Font("Arial", 16), Brushes.Black, new PointF(300, 10));
        }

        private void BoardPanel_MouseDown(object sender, MouseEventArgs e)
        {
            for (var i = 0; i < _gameModel.AvailableFigures.Count; i++)
            {
                var figure = _gameModel.AvailableFigures[i];
                var figureX = _gameModel.Board.Size + i * 3;
                var figureY = 0;

                if (e.X >= figureX * 30 && e.X < (figureX + figure.Shape.GetLength(0)) * 30 &&
                    e.Y >= figureY * 30 && e.Y < (figureY + figure.Shape.GetLength(1)) * 30)
                {
                    _draggingFigure = figure;
                    _dragStartPoint = new Point(e.X, e.Y);
                    _currentDragPoint = new Point(figureX, figureY);
                    Invalidate();
                    break;
                }
            }
        }

        private void BoardPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_draggingFigure != null)
            {
                _currentDragPoint = new Point(e.X / 30, e.Y / 30);
                Invalidate();
            }
        }

        private void BoardPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (_draggingFigure != null)
            {
                var x = e.X / 30;
                var y = e.Y / 30;

                if (_gameModel.CanPlaceFigure(_draggingFigure, x, y))
                {
                    _gameModel.PlaceFigure(_draggingFigure, x, y);
                    _draggingFigure = null;
                    _dragStartPoint = Point.Empty;
                    _currentDragPoint = Point.Empty;
                    Invalidate();
                }
                else
                {
                    _draggingFigure = null;
                    Invalidate();
                }

                if (_gameModel.IsGameOver())
                {
                    ShowGameOver();
                }
            }
        }

        private void ShowGameOver()
        {
            var result = MessageBox.Show($"Игра окончена! Ваш счет: {_gameModel.Score}\nЗакрыть игру?", "Конец игры", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                Close();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            for (var x = 0; x < _gameModel.Board.Size; x++)
            {
                for (var y = 0; y < _gameModel.Board.Size; y++)
                {
                    var cell = new Rectangle(x * 30, y * 30, 30, 30);
                    g.FillRectangle(_gameModel.Board.Cells[x, y] == 1 ? Brushes.Green : Brushes.White, cell);
                    g.DrawRectangle(Pens.Black, cell);
                }
            }

            DrawFigures(g);
            GameView_Paint(this, e);
        }

        private void DrawFigures(Graphics g)
        {
            for (var i = 0; i < _gameModel.AvailableFigures.Count; i++)
            {
                var figure = _gameModel.AvailableFigures[i];
                var shape = figure.Shape;
                var shapeWidth = shape.GetLength(0);
                var shapeHeight = shape.GetLength(1);

                var startX = _gameModel.Board.Size + i * 3;
                var startY = 0;

                for (var x = 0; x < shapeWidth; x++)
                    for (var y = 0; y < shapeHeight; y++)
                        if (shape[x, y] == 1)
                        {
                            var cell = new Rectangle((startX + x) * 30, (startY + y) * 30, 30, 30);
                            g.FillRectangle(Brushes.Blue, cell);
                            g.DrawRectangle(Pens.Black, cell);
                        }
            }

            if (_draggingFigure != null)
            {
                var shape = _draggingFigure.Shape;
                var shapeWidth = shape.GetLength(0);
                var shapeHeight = shape.GetLength(1);

                for (var x = 0; x < shapeWidth; x++)
                    for (var y = 0; y < shapeHeight; y++)
                        if (shape[x, y] == 1)
                        {
                            var cell = new Rectangle((_currentDragPoint.X + x) * 30, (_currentDragPoint.Y + y) * 30, 30, 30);
                            g.FillRectangle(Brushes.Blue, cell);
                            g.DrawRectangle(Pens.Black, cell);
                        }
            }
        }
    }
}
