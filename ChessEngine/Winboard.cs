using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Engine;

namespace ChessEngine.CommandLine
{
    
    class Winboard
    {
        private readonly Logic _ai = new Logic();

        public Winboard()
        {
            Board board = Board.Game;
        }

        public void ProcessCmd(String input)
        {
            if (input.Length >= 4
                && string.Format("abcdefgh").IndexOf(input.Substring(0, 1)) >= 0
                && string.Format("12345678").IndexOf(input.Substring(1, 1)) >= 0
                && string.Format("abcdefgh").IndexOf(input.Substring(2, 1)) >= 0
                && string.Format("12345678").IndexOf(input.Substring(3, 1)) >= 0)
            {
                input = "usermove " + input;
            }

            string command = input;
            string argument = "";

            int spaceIdx = input.IndexOf(' ');
            if (spaceIdx >= 0)
            {
                command = input.Substring(0, spaceIdx);
                argument = input.Substring(spaceIdx + 1);
            }
            command = command.ToLower();

            
            switch (command)
            {
                case "protover":
                    /*
                    Program.ConsoleWriteline("feature usermove=1");
                    Program.ConsoleWriteline("feature setboard=1");
                    Program.ConsoleWriteline("feature analyze=1");
                    Program.ConsoleWriteline("feature done=1");*/
                    break;
                case "new":
                    /*
                    _board.FENCurrent = new FEN(FEN.FENStart);
                    _myplayer = Player.Black;*/
                    break;
                case "force":
                    /*_myplayer = Player.None;*/
                    break;
                case "go":
                    /*_myplayer = _board.WhosTurn;
                    StartThinking();*/
                    break;
                case "time":
                    /*_timeLeft = TimeSpan.FromMilliseconds(int.Parse(argument) * 10);*/
                    break;
                case "usermove":
                    IMove move = Decode(argument);
                    move.Execute();
                    StartThinking();
                    break;
                case "?":
                    //implement later
                    //player.ForceMove();
                    //Move now. If your engine is thinking, it should move immediately; otherwise, the command should be ignored (treated as a no-op).
                    break;
                case "draw":
                    //The engine's opponent offers the engine a draw. To accept the draw, send "offer draw". To decline, ignore the offer (that is, send nothing). 
                    //If you're playing on ICS, it's possible for the draw offer to have been withdrawn by the time you accept it, so don't assume the game is over because you accept a draw offer. Continue playing until xboard tells you the game is over. See also "offer draw" below.
                    break;
                case "setboard":
                    /*_board.FENCurrent = new FEN(argument);*/
                    break;
                case "undo":
                    /*_board.MoveUndo();*/
                    break;
                case "remove":
                    /*_board.MoveUndo();
                    _board.MoveUndo();*/
                    break;
                case "level":
                    /*_timeControl = TimeControl.Parse(argument);*/
                    break;
                case "analyze":
                    /*_player.YourTurn(_board, new TimeControl(TimeSpan.FromDays(365), TimeSpan.FromDays(1), 0), TimeSpan.FromDays(365));*/
                    break;
                case "exit":
                    /*_player.TurnStop();*/
                    break;
                //custom stuff for debugging
                case "setpos1":
                    /*_board.FENCurrent = new FEN("rr3bk1/pppq1p2/1nn1pB1p/3bP3/P2P1Q2/1P1B1P2/4N1PP/R4RK1 b - - 4 18 ");*/
                    break;
                case "eval":
                    /*Evaluator eval = new Evaluator();
                    int e = eval.EvalFor(_board, _board.WhosTurn);*/
                    break;
            }
        }

        private void StartThinking()
        {
            IMove move = _ai.GetBestMove();
            String algebraicMove = Encode(move);
            Console.WriteLine(algebraicMove);
        }

        public String Encode(IMove move)
        {
            String moveNotation = "";
            if (move is Move)
            {
                Move moving = (Move)move;
                moveNotation += moving.GetOrigin();
                if (moving.GetKill() != null)
                {
                    moveNotation += "x";
                }
                moveNotation += moving.GetTarget();
                if (moving.IsCheck())
                {
                    moveNotation += "+";
                }
            }
            else if (move is Castling)
            {
                Castling castling = (Castling)move;
                moveNotation = castling.GetSide() == "king" ? "O-O" : "O-O-O";
            }
            return moveNotation;
        }

        public IMove Decode(String input)
        {
            IMove move;
            if (input.Contains("O"))
            {
                move = input == "O-O" ? new Castling(0, 0) : new Castling(0, 7);
            }
            else
            {
                
                move = new UserMove(input.Substring(0,2), input.Substring(2,2));

            }
            return null;
        }
    }
}
