namespace BlockBlast
{
    public class GameController
    {
        private GameModel _gameModel;

        public GameController(GameModel gameModel)
        {
            _gameModel = gameModel;
        }

        public void PlaceFigure(Figure figure, int x, int y)
        {
            _gameModel.PlaceFigure(figure, x, y);
        }
    }
}