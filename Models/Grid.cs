namespace TicTacToe.Models;

using System.Collections;
using Enums;

public class Grid : IEnumerable
{
    /// <summary>
    ///    x0, x1, x2
    /// y0: X,  X,  X
    /// y1: X,  X,  X
    /// y2: X,  X,  X
    ///
    /// saved as (y, x) or (row, col)
    /// </summary>
    public List<List<CellStates>> Content { get; set; }

    public string State { get; set; }

    public Grid()
    {
        Content = new List<List<CellStates>>();
        for (var y = 0; y < 3; y++)
        {
            var row = new List<CellStates>();
            for (var x = 0; x < 3; x++)
            {
                row.Add(CellStates.E);
            }
            Content.Add(row);
        }
    }

    public Grid(string state)
    {
        var elements = state.Split(",");
        var counter = 0;

        Content = new List<List<CellStates>>();
        for (var y = 0; y < 3; y++)
        {
            var row = new List<CellStates>();
            for (var x = 0; x < 3; x++)
            {
                var cellVal = elements[counter];
                Enum.TryParse(cellVal, out CellStates cellState);

                row.Add(string.IsNullOrWhiteSpace(cellVal) ? CellStates.E : cellState);
                counter++;
            }
            Content.Add(row);
        }
    }

    private void UpdateState()
    {
        State = $"{CharAt(0, 0)}, {CharAt(0, 1)}, {CharAt(0, 2)},\n " +
                $"{CharAt(1, 0)}, {CharAt(1, 1)}, {CharAt(1, 2)},\n " +
                $"{CharAt(2, 0)}, {CharAt(2, 1)}, {CharAt(2, 2)}";
    }

    public string CharAt(int y, int x)
    {
        return Get(y, x) == CellStates.E ? " " : Get(y, x).ToString();
    }

    public CellStates Get(int y, int x)
    {
        return Content[y][x];
    }

    public CellStates Get((int y, int x) coordinateTuple)
    {
        var (y, x) = coordinateTuple;
        return Content[y][x];
    }

    public void Set(int y, int x, CellStates state)
    {
        Content[y][x] = state;
        UpdateState();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public GridEnumerator<CellStates> GetEnumerator()
    {
        return new GridEnumerator<CellStates>(Content);
    }
}

public class GridEnumerator<T> : IEnumerator where T : IConvertible
{
    private readonly List<List<T>> _grid;
    private int _position = -1;

    public GridEnumerator(List<List<T>> grid)
    {
        _grid = grid;
    }

    public List<T> Current
    {
        get
        {
            try
            {
                return _grid[_position];
            }
            catch (IndexOutOfRangeException)
            {
                throw new InvalidOperationException();
            }
        }
    }

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        _position++;
        return _position < _grid.Count;
    }

    public void Reset()
    {
        _position = -1;
    }
}