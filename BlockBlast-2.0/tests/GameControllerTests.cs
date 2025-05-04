using BlockBlast_2._0.controllers;
using NUnit.Framework;

namespace BlockBlast_2._0.tests;

[TestFixture]
public class GameControllerTests
{
    private GameController _controller;
    private const int GridSize = 10;
    private const int TimeLimit = 30;

    [SetUp]
    public void Setup()
    {
        _controller = new GameController(2, GridSize, TimeLimit);
    }

    [Test]
    public void GameController_Constructor_InitializesCorrectly()
    {
        Assert.AreEqual(2, _controller.Players.Count);
        Assert.AreEqual(GridSize, _controller.GetCells().GetLength(0));
        Assert.AreEqual(GridSize, _controller.GetCells().GetLength(1));
        Assert.AreEqual(_controller.Players[0], _controller.CurrentPlayer);
    }

    [Test]
    public void NextPlayer_RotatesPlayersCorrectly()
    {
        _controller.NextPlayer();
        
        Assert.AreEqual(_controller.Players[1], _controller.CurrentPlayer);
        
        _controller.NextPlayer();
        
        Assert.AreEqual(_controller.Players[0], _controller.CurrentPlayer);
    }

    [Test]
    public void PlaceFigure_ReturnsFalse_WhenOutOfBounds()
    {
        var figure = _controller.CurrentPlayer.Figures[0];
        
        var result = _controller.PlaceFigure(figure, GridSize, GridSize);
        
        Assert.IsFalse(result);
    }

    [Test]
    public void PlaceFigure_ReturnsTrue_WhenPlacementIsValid()
    {
        var figure = _controller.CurrentPlayer.Figures[0];
        
        var result = _controller.PlaceFigure(figure, 0, 0);
        
        Assert.IsTrue(result);
    }

    [Test]
    public void CheckAndClearLines_AddsScore_WhenLineIsCleared()
    {
        var cells = _controller.GetCells();
        for (var i = 0; i < GridSize; i++)
            cells[0, i].BackColor = Color.FromArgb(_controller.CurrentPlayer.Figures[0].Pixels[0].Color);
        var initialScore = _controller.CurrentPlayer.Score;
        
        _controller.CheckAndClearLines();
        
        Assert.AreEqual(initialScore + 100, _controller.CurrentPlayer.Score);
    }

    [Test]
    public void CanPlaceFigure_ReturnsTrue_WhenPlacementIsPossible()
    {
        var figure = _controller.CurrentPlayer.Figures[0];
        var result = _controller.CanPlaceFigure(figure);
        
        Assert.IsTrue(result);
    }

    [Test]
    public void CanPlayerPlaceAnyFigure_ReturnsFalse_WhenNoPlacementsPossible()
    {
        var cells = _controller.GetCells();
        for (var i = 0; i < GridSize; i++)
        for (var j = 0; j < GridSize; j++)
            cells[i, j].BackColor = Color.Red;
        
        var result = _controller.CanPlayerPlaceAnyFigure(_controller.CurrentPlayer);
        
        Assert.IsFalse(result);
    }
    
    [Test]
    public void InitializeTimer_SetsUpTimerCorrectly()
    {
        EventHandler handler = (_, _) => _ = true;
        
        _controller.InitializeTimer(handler);
        _controller.UpdateTimer(); 
        
        StringAssert.Contains((TimeLimit - 1).ToString(), _controller.GetTimeText());
        Assert.AreEqual(TimeLimit - 1, _controller.GetTimeLeft());
    }

    [Test]
    public void GetTimeLabelColor_ReturnsRed_WhenTimeIsLow()
    {
        _controller.InitializeTimer((_, _) => { });
        for (var i = 0; i < TimeLimit - 5; i++)
            _controller.UpdateTimer();
        
        var color = _controller.GetTimeLabelColor();
        Assert.AreEqual(Color.Red, color);
    }

    [Test]
    public void GetEndGameMessage_ReturnsCorrectMessage_ForSinglePlayer()
    {
        var singlePlayerController = new GameController(1, GridSize, TimeLimit);
        singlePlayerController.CurrentPlayer.AddScore(500);
        
        var message = singlePlayerController.GetEndGameMessage();
        
        StringAssert.Contains("500", message);
    }

    [Test]
    public void GetEndGameMessage_ReturnsCorrectMessage_ForMultiplePlayers()
    {
        _controller.Players[0].AddScore(100);
        _controller.Players[1].AddScore(200);
        
        var message = _controller.GetEndGameMessage();
        
        StringAssert.Contains("Игрок 1: 100 очков", message);
        StringAssert.Contains("Игрок 2: 200 очков", message);
    }

    [Test]
    public void TryPlaceFigure_ReturnsFalse_WhenPlacementIsInvalid()
    {
        var figure = _controller.CurrentPlayer.Figures[0];
        _controller.StartDragging(figure);
        
        var invalidPosition = new Point(-10000, -10000);
        
        var result = _controller.TryPlaceFigure(invalidPosition);
        Console.WriteLine($"Result: {result}");
        
        Assert.IsFalse(result, "Метод должен возвращать false для позиции за пределами сетки");
    }
}