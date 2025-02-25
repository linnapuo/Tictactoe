namespace Tictactoe.Engine;

public record Game(bool XIsNext, string?[] Squares)
{
    public bool XIsNext { get; set; } = XIsNext;
}

public record Player(string Name, bool IsX);

public record Lobby(string GameId, List<Player> Players, Game Game);

public record Move(string GameId, int Square);

public record Create(string GameId);

public record Join(string GameId);

public record Spectate(string GameId);

public record GameState(string GameId, string?[] Squares, List<Player> Players, bool XIsNext);
