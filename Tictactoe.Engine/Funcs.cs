namespace Tictactoe.Engine;

public static class Funcs
{
    private static readonly List<(int, int, int)> _lines =
    [
        (0, 1, 2),
        (3, 4, 5),
        (6, 7, 8),
        (0, 3, 6),
        (1, 4, 7),
        (2, 5, 8),
        (0, 4, 8),
        (2, 4, 6)
    ];

    private static string? CheckWinner(string?[] squares)
    {
        var winnerLine = _lines.FirstOrDefault(line =>
            squares[line.Item1] != null &&
            squares[line.Item1] == squares[line.Item2] &&
            squares[line.Item1] == squares[line.Item3]);

        return winnerLine != default ? squares[winnerLine.Item1] : null;
    }

    public static Lobby CreateGame(Create create, string connectionId)
    {
        return new(create.GameId, [new(connectionId, true)], new(true, new string?[9]));
    }

    public static Result<Lobby, string> JoinGame(Lobby lobby, string connectionId)
    {
        if (lobby.Players.Count == 2)
        {
            return Result<Lobby, string>.Error("Game has already started");
        }

        lobby.Players.Add(new(connectionId, false));
        return Result<Lobby, string>.Ok(lobby);
    }

    private static Result<Lobby, string> ValidatePlayersTurn(Lobby lobby, Player player)
    {
        if (player.IsX && !lobby.Game.XIsNext || !player.IsX && lobby.Game.XIsNext)
        {
            return Result<Lobby, string>.Error("Not your turn");
        }
        return Result<Lobby, string>.Ok(lobby);
    }

    private static Result<Lobby, string> ValidateMove(Lobby lobby, Move move)
    {
        if (move.Square < 0 || move.Square > 8 || lobby.Game.Squares[move.Square] != null)
        {
            return Result<Lobby, string>.Error("Invalid move");
        }
        return Result<Lobby, string>.Ok(lobby);
    }

    private static Result<Lobby, string> GameOver(Lobby lobby)
    {
        return CheckWinner(lobby.Game.Squares) != null ? Result<Lobby, string>.Error("Game over") : Result<Lobby, string>.Ok(lobby);
    }

    public static Result<Lobby, string> MakeMove(Move move, Lobby lobby, string connectionId)
    {
        var player = lobby.Players.FirstOrDefault(p => p.Name == connectionId);

        if (player == null)
        {
            return Result<Lobby, string>.Error("Player not found");
        }

        var result = GameOver(lobby)
            .Bind(_ => ValidatePlayersTurn(lobby, player))
            .Bind(_ => ValidateMove(lobby, move));

        if (result.IsSuccess)
        {
            lobby.Game.Squares[move.Square] = player.IsX ? "X" : "O";
            lobby.Game.XIsNext = !lobby.Game.XIsNext;
        }

        return result;
    }

}
