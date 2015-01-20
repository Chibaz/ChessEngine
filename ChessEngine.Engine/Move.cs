using System;
using System.Globalization;
using ChessEngine;

namespace ChessEngine.Engine
{
    public interface IMove
    {
        void Execute();
        void ExecuteOnBoard(Board temp);
        void Undo();
    }

//    public struct MovingPiece
//    {
//        public byte Origin;
//        public byte Target;
//        public int Piece;
//
//        public MovingPiece(byte origin, byte target, int piece)
//        {
//            Origin = origin;
//            Target = target;
//            Piece = piece;
//        }
//    }
//
//    public struct TakenPiece
//    {
//        public byte Position;
//        public int Piece;
//
//        public TakenPiece(byte position, int piece)
//        {
//            Position = position;
//            Piece = piece;
//        }
//    }

    public class EnemyMove : IMove
    {
        private readonly byte _origin;
        private readonly byte _target;
        private readonly byte _piece;
        private byte _kill;

        public EnemyMove(byte origin, byte target)
        {
            _origin = origin;
            _target = target;
            _piece = Board.Game.Tiles[_origin];
        }

        public void Execute()
        {
            _kill = Board.Game.Tiles[_target];
            Board.Game.Tiles[_target] = _piece;
            Board.Game.Tiles[_origin] = 0;
        }

        public void ExecuteOnBoard(Board temp)
        {
            _kill = temp.Tiles[_target];
            temp.Tiles[_target] = _piece;
            temp.Tiles[_origin] = 0;
        }

        public void Undo()
        {
            Board.Game.Tiles[_target] = _kill;
            Board.Game.Tiles[_origin] = _piece;
        }
    }

    public class Move : IMove
    {
        public byte Origin;
        public byte Target;
        public byte Piece;
        public byte Kill;

        public Move(byte origin, byte piece)
        {
            Origin = origin;
            Piece = piece;
            Kill = 0;
        }

        public void Execute()
        {
            //Board.CheckForStuff(Board.Game, this);
            //kill = Board.Game.Tiles[target];
            Board.Game.Tiles[Target] = Piece;
            Board.Game.Tiles[Origin] = 0;
            if ((Piece & 0x07) == 1 && (Origin & 0x70) - (Target & 0x70) == 32)
            {
                Board.Game.EnPassant = Target;
            }
            else
            {
                Board.Game.EnPassant = 0;
            }
            //Board.CheckForCheck(Board.Game);
            //Board.Game.CheckChecking(Board.Game);
        }

        public void ExecuteOnBoard(Board temp)
        {
            //Board.CheckForStuff(temp, this);
            //kill = temp.Tiles[target];
            temp.Tiles[Target] = Piece;
            temp.Tiles[Origin] = 0;
            if ((Piece & 0x07) == 1 && (Origin & 0x70) - (Target & 0x70) == 32)
            {
                Board.Game.EnPassant = Target;
            }
            else
            {
                Board.Game.EnPassant = 0;
            }
            //Board.CheckForCheck(temp);
            //Board.Game.CheckChecking(temp);
        }

        public void Undo()
        {
            Board.Game.Tiles[Target] = Kill;
            Board.Game.Tiles[Origin] = Piece;
        }
    }

    public class EnPassant : IMove
    {
        public byte Origin;
        public byte Target;

        public EnPassant(byte origin, byte target)
        {
            Origin = origin;
            Target = target;
        }

        public void Execute()
        {
            Board.Game.Tiles[Target] = 1;
            Board.Game.Tiles[Origin] = 0;
        }

        public void ExecuteOnBoard(Board temp)
        {
            temp.Tiles[Target] = 1;
            temp.Tiles[Origin] = 0;
        }

        public void Undo()
        {
            Board.Game.Tiles[Target] = 1;
            Board.Game.Tiles[Origin] = 1;
        }
    }

    public class Castling : IMove
    {
        public byte King, Rook;
        public int RookFile;
        private readonly int _kingTarget, _rookTarget;

        public Castling(byte k, int rFile)
        {
            King = k;
            RookFile = rFile;
            if (RookFile == 0)
            {
                _kingTarget = 2;
                _rookTarget = 3;
                Rook = Board.Game.Tiles[0 + RookFile];
            }
            else
            {
                _kingTarget = 6;
                _rookTarget = 5;
                Rook = Board.Game.Tiles[112 + RookFile];
            }
        }

        public void Execute()
        {
            byte[] tiles = Board.Game.Tiles;
            //Board.CheckForStuff(Board.Game, this);
            if (Logic.Player == Logic.BlackPlayer)
            {
                tiles[112 + 4] = 0;
                tiles[112 + RookFile] = 0;
                tiles[112 + _rookTarget] = Rook;
                tiles[112 + _kingTarget] = King;
            }
            else
            {
                tiles[0 + 4] = 0;
                tiles[0 + RookFile] = 0;
                tiles[(byte)(0 + _rookTarget)] = Rook;
                tiles[(byte)(0 + _kingTarget)] = King;
            }
            Board.Game.EnPassant = 0;
        }

        public void ExecuteOnBoard(Board temp)
        {
            byte[] tiles = temp.Tiles;
            //Board.CheckForStuff(temp, this);

            if (King * Logic.Player == 6)
            {
                tiles[(byte)(7 + _rookTarget)] = tiles[(byte)(7 + RookFile)];
                tiles[(byte)(7 + _kingTarget)] = King;
            }
            else
            {
                tiles[(byte)(0 + _rookTarget)] = tiles[(byte)(0 + RookFile)];
                tiles[(byte)(0 + _kingTarget)] = King;
            }
            tiles[(byte)(0 + RookFile)] = 0;
            tiles[(byte)(0 + 4)] = 0;
            temp.EnPassant = 0;
        }

        public void Undo()
        {
            byte[] tiles = Board.Game.Tiles;
            if (Logic.Player == Logic.BlackPlayer)
            {
                tiles[112 + 4] = King;
                tiles[112 + RookFile] = Rook;
                tiles[112 + _rookTarget] = 0;
                tiles[112 + _kingTarget] = 0;
            }
            else
            {
                tiles[0 + 4] = King;
                tiles[0 + RookFile] = Rook;
                tiles[0 + _rookTarget] = 0;
                tiles[0 + _kingTarget] = 0;
            }
        }

        public string GetSide()
        {
            if (RookFile == 0)
            {
                return "queen";
            }
            else
            {
                return "king";
            }
        }
    }
}
