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
            Board.Game.LastMovedPiece = _target;
        }

        public void ExecuteOnBoard(Board temp)
        {
            _kill = temp.Tiles[_target];
            temp.Tiles[_target] = _piece;
            temp.Tiles[_origin] = 0;
            temp.LastMovedPiece = _target;
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
            Board.Game.LastMovedPiece = Target;
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
                temp.EnPassant = Target;
            }
            else
            {
                temp.EnPassant = 0;
            }
            //Board.CheckForCheck(temp);
            //Board.Game.CheckChecking(temp);
            temp.LastMovedPiece = Target;
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
            Board.Game.LastMovedPiece = Target;
        }

        public void ExecuteOnBoard(Board temp)
        {
            temp.Tiles[Target] = 1;
            temp.Tiles[Origin] = 0;
            temp.LastMovedPiece = Target;
        }

        public void Undo()
        {
            Board.Game.Tiles[Target] = 1;
            Board.Game.Tiles[Origin] = 1;
        }
    }

    public class Castling : IMove
    {
        private byte _kingOrigin, _rookOrigin;
        private readonly byte _king;
        private readonly byte _rook;
        public int RookFile;
        private  int _kingTarget, _rookTarget;

        public Castling(byte kingOrigin, byte king, byte rookOrigin, byte rook)
        {
            _kingOrigin = kingOrigin;
            _rookOrigin = rookOrigin;
            _king = king;
            _rook = rook;
            if ((_rookOrigin & 0x07) == 0x07)
            {
                _rookTarget = _rookOrigin - 2;
                _kingTarget = _kingOrigin + 2;
            }
            else
            {
                _rookTarget = _rookOrigin + 3;
                _kingTarget = _kingOrigin - 2;
            }
        }

        public void Execute()
        {
            Board.Game.Tiles[_kingOrigin] = 0;
            Board.Game.Tiles[_rookOrigin] = 0;
            Board.Game.Tiles[_kingTarget] = _king;
            Board.Game.Tiles[_rookTarget] = _rook;
            Board.Game.LastMovedPiece = (byte)_rookTarget;

            Board.Game.EnPassant = 0;
            //Board.CheckForStuff(Board.Game, this);

//            tiles[112 + 4] = 0;
//                tiles[112 + RookFile] = 0;
//                tiles[112 + _rookTarget] = _rook;
//                tiles[112 + _kingTarget] = _king;
//            }
//            else
//            {
//                tiles[0 + 4] = 0;
//                tiles[0 + RookFile] = 0;
//                tiles[0 + _rookTarget] = _rook;
//                tiles[0 + _kingTarget] = _king;
//                Board.Game.LastMovedPiece = (byte)(0 + _rookTarget);
//            }
            
        }

        public void ExecuteOnBoard(Board temp)
        {
            temp.Tiles[_kingOrigin] = 0;
            temp.Tiles[_rookOrigin] = 0;
            temp.Tiles[_kingTarget] = _king;
            temp.Tiles[_rookTarget] = _rook;
            temp.LastMovedPiece = (byte)_rookTarget;

            temp.EnPassant = 0;
            //Board.CheckForStuff(temp, this);

//            if (_king * Logic.Player == 6)
//            {
//                tiles[112 + 4] = 0;
//                tiles[112 + RookFile] = 0;
//                tiles[112 + _rookTarget] = _rook;
//                tiles[112 + _kingTarget] = _king;
//                temp.LastMovedPiece = (byte)(112 + _rookTarget);
//            }
//            else
//            {
//                tiles[0 + 4] = 0;
//                tiles[0 + RookFile] = 0;
//                tiles[0 + _rookTarget] = _rook;
//                tiles[0 + _kingTarget] = _king;
//                temp.LastMovedPiece = (byte)(0 + _rookTarget);
//            }
//            tiles[(byte)(0 + RookFile)] = 0;
//            tiles[(byte)(0 + 4)] = 0;
            temp.EnPassant = 0;
        }

        public void Undo()
        {
            Board.Game.Tiles[_kingOrigin] = _king;
            Board.Game.Tiles[_rookOrigin] = _rook;
            Board.Game.Tiles[_kingTarget] = 0;
            Board.Game.Tiles[_rookTarget] = 0;
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
