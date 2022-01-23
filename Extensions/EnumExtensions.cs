namespace TicTacToe.Extensions
{
    using Enums;

    public static class EnumExtensions
    {
        public static Players ToPlayer(this CellStates state)
        {
            return state switch
            {
                CellStates.X => Players.X,
                CellStates.O => Players.O,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, "Only X and O can be converted to the player")
            };
        }

        public static CellStates ToCellState(this Players player)
        {
            return player switch
            {
                Players.O => CellStates.O,
                Players.X => CellStates.X,
                _ => throw new NotImplementedException()
            };
        }

        public static Results ToResult(this Players player)
        {
            return player switch
            {
                Players.O => Results.O,
                Players.X => Results.X,
                _ => throw new NotImplementedException()
            };
        }

        public static Players ToPlayer(this Results result)
        {
            return result switch
            {
                Results.X => Players.X,
                Results.O => Players.O,
                _ => throw new ArgumentOutOfRangeException(nameof(result), result, "Only X and O can be converted to the player")
            };
        }
    }
}