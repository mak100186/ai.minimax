namespace TicTacToe.Extensions
{
    using System.Diagnostics;
    using Enums;
    using Models;

    public static class GridExtensions
    {
        public static Random randomNumberGenerator = new(DateTime.UtcNow.Millisecond);

        public static void TakeAiTurn(this Grid grid)
        {
            var possibleActions = grid.GetPossibleActionsForNextTurn(null);
            
            var minVal = int.MaxValue;
            foreach (var possibleAction in possibleActions)
            {
                possibleAction.StateAfterThisAction = grid.GetStateCopyAfterAction(possibleAction);
                possibleAction.ResultAfterThisAction = possibleAction.StateAfterThisAction.GetGameState();
                possibleAction.MinimaxValue = MinValue(possibleAction.StateAfterThisAction, possibleAction);

                minVal = Math.Min(minVal, possibleAction.MinimaxValue);

                Console.Write($"Move:{possibleAction.Y},{possibleAction.X} \n {possibleAction.StateAfterThisAction.State} \n {possibleAction.ResultAfterThisAction}, {possibleAction.MinimaxValue}\n");
            }

            var priorityOfActions = new List<Results> { Results.O, Results.NotOver, Results.Draw, Results.X };

            foreach (var action in priorityOfActions)
            {
                var selectedActions = possibleActions.Where(a => a.MinimaxValue == minVal && a.ResultAfterThisAction == action).ToList();
                if (!selectedActions.Any()) continue;

                var chosenAction = selectedActions.Skip(randomNumberGenerator.Next(0, selectedActions.Count)).Take(1).First();

                if (chosenAction != null)
                {
                    grid.ApplyAction(chosenAction);
                    grid.Print();

                    return;
                }
            }

            foreach (var action in priorityOfActions)
            {
                var selectedActions = possibleActions.Where(a => a.ResultAfterThisAction == action).ToList();
                if (!selectedActions.Any()) continue;

                var chosenAction = selectedActions.Skip(randomNumberGenerator.Next(0, selectedActions.Count)).Take(1).First();
                
                if (chosenAction != null)
                {
                    grid.ApplyAction(chosenAction);
                    grid.Print();

                    return;
                }
            }
        }

        public static bool IsValidCoordinate(this Grid grid, int y, int x)
        {
            return x is >= 0 and <= 2 && y is >= 0 and <= 2;
        }

        public static void Print(this Grid grid)
        {
            for (var y = 0; y < 3; y++)
            {
                var row = " ";
                for (var x = 0; x < 3; x++)
                {
                    row += grid.CharAt(y, x);
                    row += x != 2 ? ", " : "";
                }
                Console.WriteLine(row);
            }
        }

        public static (int countOfX, int countOfO, int countOfEmpty) GetStateCounts(this Grid grid)
        {
            int countOfX = 0, countOfO = 0, countOfEmpty = 0;

            for (var y = 0; y < 3; y++)
            {
                for (var x = 0; x < 3; x++)
                {
                    switch (grid.Get(y, x))
                    {
                        case CellStates.E:
                            countOfEmpty++;
                            break;
                        case CellStates.O:
                            countOfO++;
                            break;
                        case CellStates.X:
                            countOfX++;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return (countOfX, countOfO, countOfEmpty);
        }

        //equivalent of Players(s)
        public static Players GetPlayerWithNextTurn(this Grid grid)
        {
            var (countOfX, countOfO, countOfEmpty) = grid.GetStateCounts();
            if (countOfEmpty == 9 || countOfO == countOfX)
                //starting state, first turn goes to X
                return Players.X;

            return countOfO > countOfX ? Players.X : Players.O;
        }

        //equivalent of Actions(s)
        public static List<Action> GetPossibleActionsForNextTurn(this Grid grid, Action parentAction)
        {
            var playerWithNextTurn = grid.GetPlayerWithNextTurn();
            var possibleActions = new List<Action>();

            for (var y = 0; y < 3; y++)
            {
                for (var x = 0; x < 3; x++)
                {
                    if (grid.Get(y, x) != CellStates.E) continue;

                    possibleActions.Add(new Action
                    {
                        ParentAction = parentAction,
                        Player = playerWithNextTurn,
                        X = x,
                        Y = y
                    });
                }
            }

            return possibleActions;
        }

        //equivalent of Result(s, a)
        public static Grid GetStateCopyAfterAction(this Grid grid, Action action)
        {
            var newGrid = grid.Copy();
            newGrid.Set(action.Y, action.X, action.Player.ToCellState());
            return newGrid;
        }

        public static Grid Copy(this Grid grid)
        {
            var newGrid = new Grid();

            for (var y = 0; y < 3; y++)
            {
                for (var x = 0; x < 3; x++)
                {
                    newGrid.Set(y, x, grid.Get(y, x));
                }
            }

            return newGrid;
        }

        public static Grid ApplyAction(this Grid grid, Action action)
        {
            grid.Set(action.Y, action.X, action.Player.ToCellState());
            return grid;
        }

        //equivalent of Terminal(s)
        public static Results GetGameState(this Grid grid)
        {
            var (countOfX, countOfO, countOfEmpty) = grid.GetStateCounts();
            
            var winner = GetPlayerWithWinningCombination(grid);

            if (winner != null) 
                return winner.Value.ToResult();

            if (countOfX + countOfO == 9 && countOfEmpty == 0)
                return Results.Draw;

            return Results.NotOver;
        }

        public static Players? GetPlayerWithWinningCombination(this Grid grid)
        {
            var listOfCombinations = new List<CellStates?>
            {
                grid.AreCellStatesSame((0, 0), (0, 1), (0, 2)),
                grid.AreCellStatesSame((1, 0), (1, 1), (1, 2)),
                grid.AreCellStatesSame((2, 0), (2, 1), (2, 2)),
                grid.AreCellStatesSame((0, 0), (1, 0), (2, 0)),
                grid.AreCellStatesSame((0, 1), (1, 1), (2, 1)),
                grid.AreCellStatesSame((0, 2), (1, 2), (2, 2)),
                grid.AreCellStatesSame((0, 0), (1, 1), (2, 2)),
                grid.AreCellStatesSame((0, 2), (1, 1), (2, 0))
            };

            var state = listOfCombinations.FirstOrDefault(combination => combination.HasValue && combination.Value != CellStates.E);

            return state?.ToPlayer();
        }

        public static CellStates? AreCellStatesSame(this Grid grid, params (int y, int x)[] cells)
        {
            if (cells.Length <= 1) return null;

            var state = grid.Get(cells.First());
            var counter = 1;

            while (counter < cells.Length)
            {
                var nextState = grid.Get(cells[counter]);

                if (nextState != state) return null;

                counter++;
            }

            return state;
        }

        //equivalent of Utility(s)
        public static int GetScore(this Grid grid)
        {
            var result = grid.GetGameState();

            return result switch
            {
                Results.O => -1,
                Results.X => 1,
                Results.Draw => 0
            };
        }

        //equivalent of Max-Value(s)
        public static int MaxValue(this Grid grid, Action parentAction)
        {
            if (GetGameState(grid) != Results.NotOver)
            {
                return grid.GetScore();
            }

            var v = int.MinValue;

            var possibleActions = grid.GetPossibleActionsForNextTurn(parentAction);
            foreach (var possibleAction in possibleActions)
            {
                v = Math.Max(v, MinValue(grid.GetStateCopyAfterAction(possibleAction), possibleAction));
            }

            return v;
        }

        //equivalent of Min-Value(s)
        public static int MinValue(this Grid grid, Action parentAction)
        {
            var gameState = GetGameState(grid);
            if (gameState != Results.NotOver)
            {
                var message = $"{gameState} achieved using: \n";
                var parent = parentAction;
                while (parent != null)
                {
                    message += $"{parent.Player}({parent.Y},{parent.X}) - {parent.MinimaxValue}\n";
                    parent = parent.ParentAction;
                }
                Console.WriteLine(message);

                return grid.GetScore();
            }

            var v = int.MaxValue;

            var possibleActions = grid.GetPossibleActionsForNextTurn(parentAction);
            foreach (var possibleAction in possibleActions)
            {
                v = Math.Min(v, MaxValue(grid.GetStateCopyAfterAction(possibleAction), possibleAction));
            }

            return v;
        }
    }
}