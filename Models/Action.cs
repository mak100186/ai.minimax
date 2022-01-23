namespace TicTacToe.Models
{
    using Enums;

    public class Action
    {
        public Players Player { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Grid StateAfterThisAction { get; set; }
        public Results ResultAfterThisAction { get; set; }
        public int MinimaxValue { get; set; }
        public Action ParentAction { get; set; }
    }
}