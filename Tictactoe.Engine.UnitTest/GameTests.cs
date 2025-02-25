namespace Tictactoe.Engine.UnitTest;

public class GameTests
{
    [Fact]
    public void CreateGame_ShouldReturnOk()
    {
        var create = new Create("1");

        var lobby = Funcs.CreateGame(create, "connection-1");

        var expected = new Lobby("1", [new Player("connection-1", true)], new Game(true, new string?[9]));

        Assert.Equivalent(expected, lobby);
    }

    [Fact]
    public void JoinGame_ShouldReturnOk()
    {
        var lobby = new Lobby("1", [new Player("connection-1", true)], new Game(true, new string?[9]));

        var result = Funcs.JoinGame(lobby, "connection-2");

        var expected = Result<Lobby, string>.Ok(
            new Lobby("1",
            [new Player("connection-1", true), new Player("connection-2", false)],
            new Game(true, new string?[9])));

        Assert.Equivalent(expected, result);
    }

    [Fact]
    public void MakeMove_ShouldReturnOk()
    {
        var move = new Move("1", 0);

        var lobby = new Lobby(
            "1",
            [new Player("connection-1", true), new Player("connection-2", false)],
            new Game(true, new string?[9]));

        var result = Funcs.MakeMove(move, lobby, "connection-1");

        var expected = Result<Lobby, string>.Ok(new Lobby(
            "1",
            [new Player("connection-1", true), new Player("connection-2", false)],
            new Game(false, ["X", null, null, null, null, null, null, null, null])));

        Assert.Equivalent(expected, result);
    }

    [Fact]
    public void MakeMove_ShouldReturnNotYourTurn()
    {
        var move = new Move("1", 1);

        var lobby = new Lobby(
            "1",
            [new Player("connection-1", true), new Player("connection-2", false)],
            new Game(false, ["X", null, null, null, null, null, null, null, null]));

        var result = Funcs.MakeMove(move, lobby, "connection-1");

        var expected = Result<Lobby, string>.Error("Not your turn");

        Assert.Equivalent(expected, result);
    }

    [Fact]
    public void MakeMove_ShouldNotAllowToMakeInvalidMove()
    {
        var move = new Move("1", 0);
        
        var lobby = new Lobby(
            "1",
            [new Player("connection-1", true), new Player("connection-2", false)],
            new Game(true, ["X", null, null, null, null, null, null, null, null]));

        var result = Funcs.MakeMove(move, lobby, "connection-1");

        var expected = Result<Lobby, string>.Error("Invalid move");

        Assert.Equivalent(expected, result);
    }
}
