using TicTacToe.Enums;
using TicTacToe.Extensions;
using TicTacToe.Models;

var grid = new Grid();
grid.Print();

while (true)
{
    Console.WriteLine("Take your turn by placing at y,x:");
    try
    {
        var playerTurn = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(playerTurn))
        {
            throw new Exception("Invalid coordinates, try again!");
        }

        var coords = playerTurn.Split(",");
        var y = int.Parse(coords[0]);
        var x = int.Parse(coords[1]);

        if (grid.IsValidCoordinate(y, x) && grid.Get(y, x) == CellStates.E)
        {
            Console.Clear();
            grid.Set(y, x, CellStates.X);
            grid.Print();

            var gameState = grid.GetGameState();
            if (gameState != Results.NotOver)
            {
                Console.WriteLine($"Game result is:{gameState}");
                break;
            }

            Console.Clear();
            grid.TakeAiTurn();

            gameState = grid.GetGameState();
            if (gameState != Results.NotOver)
            {
                Console.WriteLine($"Game result is:{gameState}");
                break;
            }
        }
        else
        {
            throw new Exception("Invalid coordinates, try again!");
        }
    }
    catch (Exception e)
    {
        Console.Clear();
        Console.WriteLine(e.Message);
        grid.Print();
    }
}
