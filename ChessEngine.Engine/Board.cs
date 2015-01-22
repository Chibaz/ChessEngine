using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Engine
{
    /*
     * Board is based on a 2-dimensional array of ints, pieces are defined as:
     * 1 - Pawn
     * 2 - Rook
     * 3 - Knight
     * 4 - Bishop
     * 5 - Queen
     * 6 - King
     */
    public class Board
    {
        public static Board Game
        {
            get { return _board ?? (_board = new Board()); }
        }
        public byte EnPassant;
        public Boolean WhiteKingCastle, WhiteQueenCastle, BlackKingCastle, BlackQueenCastle;
        public Boolean aiCheck, playerCheck;
        //public int[,] tiles;
        public byte[] Tiles;
        public byte WhosTurn;
        public int Mate;
        public int MoveCount, FiftyMove;
        public byte LastMovedPiece;
        //public static int aiColor = 1;

        public Board()
        {
            //tiles = new int[8, 8];
            Tiles = new byte[128];
            ResetGame();
        }

        //Used for resetting the pieces on the board
        public void ResetGame()
        {
            WhiteKingCastle = WhiteQueenCastle = BlackKingCastle = BlackQueenCastle = true;
            aiCheck = playerCheck = false;
            Mate = 0;
            Tiles = StartBoard;
//            for(int rank = 0; rank < 8; rank++) 
//            {
//                for (int file = 0; file < 8; file++)
//                {
//                    //tiles[rank, file] = GetStartPiece(rank, file);
//                    Tiles[16 * rank + file] = GetStartPiece(rank, file);
//                }
//            }
            LastMovedPiece = 0xFF;
            WhosTurn = Logic.WhitePlayer;
        }

//        //Used for getting which piece will be at the a specified tile at the start of a game
//        public byte GetStartPiece(int rank, int file)
//        {
//            byte piece = 0x00;
//
//            //Gets which piece is supposed to be at what position
//            if (rank == 1 || rank == 6)
//            {
//                piece = 0x01;
//            }
//            else if(rank == 0 || rank == 7)
//            {
//                switch (file)
//                {
//                    case 0: case 7:
//                        piece = 0x06;
//                        break;
//                    case 1: case 6:
//                        piece = 0x02;
//                        break;
//                    case 2: case 5:
//                        piece = 0x05;
//                        break;
//                    case 3:
//                        piece = 0x07;
//                        break;
//                    case 4:
//                        piece = 0x03;
//                        break;
//                }
//            }
//
//            if (rank == 6 || rank == 7)
//            {
//                piece += 0x08;
//            }
//
//            return piece;
//        }

        
        public byte GetSpecificTile(int rank, int file)
        {
            return Tiles[16 * rank + file];
        }

        /*
        public int GetSpecificTile(int[] tile)
        {
            return tiles[tile[0], tile[1]];
        }*/

        public Board CloneBoard()
        {
            var newBoard = new Board();
            for (var h = 0; h < 8; h++)
            {
                for (var w = 0; w < 8; w++)
                {
                    newBoard.Tiles[16 * h + w] = GetSpecificTile(h, w);
                }
            }
            newBoard.Mate = this.Mate;
            newBoard.aiCheck = this.aiCheck;
//            newBoard.aiLeftCastling = this.aiLeftCastling;
//            newBoard.aiRightCastling = this.aiRightCastling;
//            newBoard.playerCheck = this.playerCheck;
//            newBoard.playerLeftCastling = this.playerLeftCastling;
//            newBoard.playerRightCastling = this.playerRightCastling;
            return newBoard;
        }

        

        /*
         * Check if specified player is in check
         * By checking if any moves of the opposing player can take the king
         * Should save all moves that puts the player in check in order to check for mate
         */
        /*
        public static Boolean CheckForCheck(Board board, int player, int[] king)
        {
            var mg = new MoveGenerator();
            var checkMoves = mg.GetAllMovesForPlayer(board, -player);

            foreach (Move nMove in checkMoves.OfType<Move>().Where(nMove => nMove.Moving.Target[0] == king[0] && nMove.Moving.Target[1] == king[1]))
            {
                if (player == Board.aiColor)
                {
                    //Console.WriteLine("ai in check");
                    board.aiCheck = true;
                    return true;
                }
                //Console.WriteLine("player in check");
                board.playerCheck = true;
                return true;
            }
            return false;
            /*
            foreach (Move move in checkMovesAI)
            {
                if (move.Moving.Target[0] == playerKing[0] && move.Moving.Target[1] == playerKing[1])
                {
                    board.playerCheck = true;
                    Console.WriteLine("player in check");
                }
            }
        }

        /*
         * Check if specified player has been set in mate
         * Should function by checking if any moves of the player makes the king not in check
         */
        /*
        public static Boolean CheckForMate(Board board, int player, int[] king)
        {
            var mg = new MoveGenerator();

            if ((player != Board.aiColor || !board.aiCheck) && (player == Board.aiColor || !board.playerCheck))
                return false;
            var checkAI = mg.GetAllMovesForPlayer(board, -player);
            if (checkAI.Cast<Move>().Any(move => move.Moving.Target[0] == king[0] && move.Moving.Target[1] == king[1]))
            {
                board.mate = player;
                //Console.WriteLine("mate for " + player);
                return true;
            }
            return false;
        }
        
        public void CheckChecking(Board board)
        {
            int[] aiKing, playerKing;
            aiKing = playerKing = null;
            Parallel.For(0, 8, row =>
            {
                //if (king != null)
                //{
                for (var col = 0; col < 8; col++)
                {
                    if ((board.tiles[row, col] * Board.aiColor) == 6)
                    {
                        aiKing = new[] { row, col };
                    }
                    else if ((board.tiles[row, col] * -Board.aiColor) == 6)
                    {
                        playerKing = new[] { row, col };
                    }
                }
            });
            if (aiKing != null)
            {
                board.aiCheck = false;
                CheckForCheck(board, aiColor, aiKing);
            }
            if (playerKing != null)
            {
                board.playerCheck = false;
                CheckForCheck(board, -aiColor, playerKing);
            }
        }
        
        public void CheckMate()
        {

        }
        */
        /*
        public static void CheckForStuff(Board board, IMove pMove)
        {
            if (!(pMove is Move)) return;
            var move = (Move)pMove;
            var piece = move.Moving.Piece;
            board.EnPassant = null;
            if (piece * Board.aiColor == 2)
            {
                if (board.aiLeftCastling && move.Moving.Origin[1] == 0)
                {
                    board.aiLeftCastling = false;
                }
                else if (board.aiRightCastling && move.Moving.Origin[1] == 7)
                {
                    board.aiRightCastling = false;
                }
            }
            else if (piece * Board.aiColor == -2)
            {
                if (board.playerLeftCastling && move.Moving.Origin[1] == 0)
                {
                    board.playerLeftCastling = false;
                }
                else if (board.playerRightCastling && move.Moving.Origin[1] == 7)
                {
                    board.playerRightCastling = false;
                }
            }
            else if (Math.Abs(piece) == 1)
            {
                if (move.Moving.Target[0] == 0 || move.Moving.Target[0] == 7)
                {
                    move.Moving.Piece = 5 * piece;
                }
                else if (piece * Board.aiColor == 1 && Math.Abs(move.Moving.Origin[0] - move.Moving.Target[0]) == 2)
                {
                    board.EnPassant = move.Moving.Target;
                }
                else if (piece * Board.aiColor == -1 && Math.Abs(move.Moving.Origin[0] - move.Moving.Target[0]) == 2)
                {
                    board.EnPassant = move.Moving.Target;
                }
            }
            else if (piece * Board.aiColor == 6)
            {
                board.aiLeftCastling = false;
                board.aiRightCastling = false;
            }
            else if (piece * Board.aiColor == -6)
            {
                board.playerLeftCastling = false;
                board.playerRightCastling = false;
            }
        }*/

        public string PrintBoard()
        {
            StringBuilder sb = new StringBuilder();
            for (int h = 7; h >= 0; h--)
            {
                for (int w = 0; w < 8; w++)
                {
                    int piece = Tiles[16*h + w];
                    if (piece < 10)
                    {
                        sb.Append(" ");
                    }
                    sb.Append(piece + " ");
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }

        private static Board _board;

        public void SwitchTurn()
        {
            if (WhosTurn.Equals(Logic.WhitePlayer))
            {
                WhosTurn = Logic.BlackPlayer;
            }
            else
            {
                WhosTurn = Logic.WhitePlayer;
            }
        }

        public byte[] StartBoard =
        {
            6, 2, 5, 7, 3, 5, 2, 6,             0, 0, 0, 0, 0, 0, 0, 0,
            1, 1, 1, 1, 1, 1, 1, 1,             0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,             0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,             0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,             0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,             0, 0, 0, 0, 0, 0, 0, 0,
            9, 9, 9, 9, 9, 9, 9, 9,             0, 0, 0, 0, 0, 0, 0, 0,
            14, 10, 13, 15, 11, 13, 10, 14,     0, 0, 0, 0, 0, 0, 0, 0

        };

        public void SetTurn(string s)
        {
            WhosTurn = s.Equals("w") ? (byte)0x00 : (byte)0x08;
        }
    }
}

