using System;
using System.Linq;
using ChessEngine.Engine;

namespace ChessEngine.CommandLine
{
    
    class Winboard
    {
        private readonly Logic _ai = new Logic();

        public Winboard()
        {
            //Board board = Board.Game;
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
                    Console.WriteLine("feature usermove=1");
                    Console.WriteLine("feature setboard=1");
                    Console.WriteLine("feature analyze=1");
                    Console.WriteLine("feature done=1");
                    break;
                case "new":
                    Board.Game.ResetGame();
                    Logic.Player = Logic.BlackPlayer;
                    break;
                case "force":
                    Logic.Player = 0xFF;
                    break;
                case "go":
                    Logic.Player = Board.Game.WhosTurn;
                    StartThinking();
                    break;
                case "time":

                    break;
                case "usermove":
                    IMove move = Decode(argument);
                    move.Execute();
                    Board.Game.SwitchTurn();
                    Program.Logger.WriteLine(Board.Game.PrintBoard());
                    StartThinking();
                    break;
                case "?":
                    _ai.Thinking = false;
                    //Move now. If your engine is thinking, it should move immediately; otherwise, the command should be ignored (treated as a no-op).
                    break;
                case "draw":
                    //The engine's opponent offers the engine a draw. To accept the draw, send "offer draw". To decline, ignore the offer (that is, send nothing). 
                    //If you're playing on ICS, it's possible for the draw offer to have been withdrawn by the time you accept it, so don't assume the game is over because you accept a draw offer. Continue playing until xboard tells you the game is over. See also "offer draw" below.
                    break;
                case "setboard":
                    SetFEN(argument);
                    break;
                case "undo":

                    break;
                case "remove":

                    break;
                case "level":

                    break;
                case "analyze":

                    break;
                case "exit":

                    break;
                //custom stuff for debugging
                case "setlopez":
                    SetFEN("r1bqkbnr/pppp1ppp/2n5/1B2p3/4P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 3 3");
                    break;
                case "perft":
                    int perft = _ai.RunPerft(int.Parse(argument));
                    Console.WriteLine("perft result: " + perft + " at depth " + argument);
                    break;
            }
        }

        private void StartThinking()
        {
            IMove move = _ai.GetBestMove();
            String algebraicMove = Encode(move);
            move.Execute();
            Board.Game.SwitchTurn();

            Program.Logger.WriteLine("logic made move: " + algebraicMove);
            Console.WriteLine("move " + algebraicMove);
        }

        public String Encode(IMove move)
        {
            String moveNotation = "";
            if (move is Move || move is EnPassant)
            {
                Move moving = (Move)move;
                moveNotation += ChessConverter.AlgStrings[moving.Origin];
                moveNotation += ChessConverter.AlgStrings[moving.Target];
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
                if (Board.Game.WhosTurn == Logic.BlackPlayer)
                {
                    move = input == "O-O" ? new Castling(0x74, 0x0B, 0x77, 0x0E) : new Castling(0x74, 0x0B, 0x70, 0x0E);
                }
                else
                {
                    move = input == "O-O" ? new Castling(0x04, 0x03, 0x07, 0x06) : new Castling(0x04, 0x03, 0x00, 0x06);
                }
            }
            else
            {
                Program.Logger.WriteLine(input + " rank: " + input.Substring(0, 2) + " file: " + input.Substring(2, 2));
                byte origin = (byte)Array.IndexOf(ChessConverter.AlgStrings, input.Substring(0, 2));
                byte target = (byte)Array.IndexOf(ChessConverter.AlgStrings, input.Substring(2, 2));
                move = new EnemyMove(origin, target);
            }

            return move;
        }
        public void SetFEN(string fen)
        {
            Board board = Board.Game;
            board.ResetGame();
            string[] info = fen.Split(new [] {'/', ' '});
            for (int rank = 7; rank >= 0; rank--)
            {
                int file = 0;
                foreach (char piece in info[rank])
                {
                    if (EmptyTiles.Contains(piece))
                    {
                        int e = (int) Char.GetNumericValue(piece);
                        for (int f = file; f < (e + file); f++)
                        {
                            board.Tiles[16 * (7 - rank) + f] = 0;
                        }
                        file += e;
                    }
                    else
                    {
                        board.Tiles[16 * (7 - rank) + file] = ChessConverter.GetPiece(piece);
                        file++;
                    }
                }
            }
            board.SetTurn(info[8]);
            foreach (char c in info[9])
            {
                if (c.Equals('-'))
                {
                    break;
                }
                switch (c)
                {
                    case 'K':
                        board.WhiteKingCastle = true;
                        break;
                    case 'Q':
                        board.WhiteQueenCastle = true;
                        break;
                    case 'k':
                        board.BlackKingCastle = true;
                        break;
                    case 'q':
                        board.BlackQueenCastle = true;
                        break;
                }
            }
            if (!info[10].Contains("-"))
            {
                board.EnPassant = (byte)Array.IndexOf(ChessConverter.AlgStrings, info[10]);
            }
            board.FiftyMove = int.Parse(info[11]);
            board.MoveCount = int.Parse(info[12]);
        }

        public static readonly char[] EmptyTiles = {'1', '2', '3', '4', '5', '6', '7', '8'};
    }
}
