namespace Tictactoe.App.Models;

public class Game(bool xIsNext, string?[] squares)
{
    public bool XIsNext { get; } = xIsNext;
    public string?[] Squares { get; } = squares;
}

public class Player(string name, bool isX)
{
    public string Name { get; } = name;
    public bool IsX { get; } = isX;
}

public class Lobby(string gameId, IReadOnlyList<Player> players, Game game)
{
    public string GameId { get; } = gameId;
    public IReadOnlyList<Player> Players { get; } = players;
    public Game Game { get; } = game;
}

public class Move(string gameId, int square)
{
    public string GameId { get; } = gameId;
    public int Square { get; } = square;
}

public class Create(string gameId)
{
    public string GameId { get; } = gameId;
}

public class Join(string gameId)
{
    public string GameId { get; } = gameId;
}

public class Spectate(string gameId)
{
    public string GameId { get; } = gameId;
}

public class Gamestate(string gameId, string[] squares, IReadOnlyList<Player> players, bool xIsNext)
{
    public string GameId { get; } = gameId;
    public string[] Squares { get; } = squares;
    public IReadOnlyList<Player> Players { get; } = players;
    public bool XIsNext { get; } = xIsNext;
}
