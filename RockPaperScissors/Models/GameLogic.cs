namespace RockPaperScissorsAPI.Models
{
    public class GameLogic
    {
        public enum Move { Rock, Paper, Scissors }
        public enum Result { Win, Lose, Draw }

        public static Result GetResult(Move playerMove, Move opponentMove)
        {
            if (playerMove == opponentMove)
                return Result.Draw;

            return (playerMove == Move.Rock && opponentMove == Move.Scissors) ||
                   (playerMove == Move.Paper && opponentMove == Move.Rock) ||
                   (playerMove == Move.Scissors && opponentMove == Move.Paper)
                   ? Result.Win
                   : Result.Lose;
        }
    }
}