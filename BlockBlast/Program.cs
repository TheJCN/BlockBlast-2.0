namespace BlockBlast
{
    public static class Program
    {
        public static void Main()
        {
            var gameModel = new GameModel(10);
            var gameController = new GameController(gameModel);
            var gameView = new GameView();
            Application.Run(gameView);
        }
    }
}