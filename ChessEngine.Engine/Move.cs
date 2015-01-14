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

    public struct MovingPiece
    {
        public byte Origin;
        public byte Target;
        public int Piece;

        public MovingPiece(byte origin, byte target, int piece)
        {
            Origin = origin;
            Target = target;
            Piece = piece;
        }
    }

    public struct TakenPiece
    {
        public byte Position;
        public int Piece;

        public TakenPiece(byte position, int piece)
        {
            Position = position;
            Piece = piece;
        }
    }

    public class UserMove : IMove
    {
        private byte _origin;
        private byte _target;

        public UserMove(string origin, string target)
        {
            _origin = (byte)Array.IndexOf(ChessConverter.AlgStrings, origin);
            _target = (byte)Array.IndexOf(ChessConverter.AlgStrings, target);
        }

        public void Execute()
        {
            byte piece = Board.Game.Tiles[_origin];
            Board.Game.Tiles[_origin] = 0;
            Board.Game.Tiles[_target] = piece;
        }

        public void ExecuteOnBoard(Board temp)
        {
            throw new NotImplementedException();
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }
    }

    public class Move : IMove
    {
        public MovingPiece Moving;
        public TakenPiece Killing;
        public Move Next;

        public byte origin;
        public byte target;
        public byte piece;

        public Move(byte origin, byte piece)
        {
            this.origin = origin;
            this.piece = piece;
        }

        public void Execute()
        {
            //Board.CheckForStuff(Board.Game, this);
            if (Killing.Position != null)
            {
                Board.Game.Tiles[Killing.Position] = 0;
            }
            Board.Game.Tiles[target] = piece;
            Board.Game.Tiles[origin] = 0;
            //Board.CheckForCheck(Board.Game);
            //Board.Game.CheckChecking(Board.Game);
        }

        public void ExecuteOnBoard(Board temp)
        {
            //Board.CheckForStuff(temp, this);
            if (Killing.Position != null)
            {
                temp.Tiles[Killing.Position] = 0;
            }
            temp.Tiles[target] = piece;
            temp.Tiles[origin] = 0;
            //Board.CheckForCheck(temp);
            //Board.Game.CheckChecking(temp);
        }

        public void Undo()
        {
//            Board.Game.tiles[Moving.Origin[0], Moving.Origin[1]] = Moving.Piece;
//            Board.Game.tiles[Moving.Target[0], Moving.Target[1]] = 0;
//            if (Killing.Position != null)
//            {
//                Board.Game.tiles[Killing.Position[0], Killing.Position[1]] = Killing.Piece;
//            }
        }

        

        public object GetKill()
        {
            throw new System.NotImplementedException();
        }

        public bool IsCheck()
        {
            throw new System.NotImplementedException();
        }
    }

    public class Castling : IMove
    {
        public byte King;
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
            }
            else
            {
                _kingTarget = 6;
                _rookTarget = 5;
            }
        }

        public void Execute()
        {
            byte[] tiles = Board.Game.Tiles;
            //Board.CheckForStuff(Board.Game, this);

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
        }

        public void Undo()
        {

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
