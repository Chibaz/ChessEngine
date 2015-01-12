using System;

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
        private string _origin;
        private string _target;

        public UserMove(string origin, string target)
        {
            _origin = origin;
            _target = target;
        }

        public void Execute()
        {
            /*
            int piece = Board.Game.tiles[int.Parse(_origin)];
            Board.Game.tiles[int.Parse(_origin)] = 0;
            Board.Game.tiles[int.Parse(_target)] = piece;
            */
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
                Board.Game.tiles[Killing.Position] = 0;
            }
            Board.Game.tiles[target] = piece;
            Board.Game.tiles[origin] = 0;
            //Board.CheckForCheck(Board.Game);
            //Board.Game.CheckChecking(Board.Game);
        }

        public void ExecuteOnBoard(Board temp)
        {
            //Board.CheckForStuff(temp, this);
            if (Killing.Position != null)
            {
                temp.tiles[Killing.Position] = 0;
            }
            temp.tiles[target] = piece;
            temp.tiles[origin] = 0;
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

        public string GetOrigin()
        {
            throw new System.NotImplementedException();
        }

        public object GetKill()
        {
            throw new System.NotImplementedException();
        }

        public string GetTarget()
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
        public byte king;
        public int rookX;
        public int nK, nR;

        public Castling(byte k, int rX)
        {
            king = k;
            rookX = rX;
            if (rookX == 0)
            {
                nK = 2;
                nR = 3;
            }
            else
            {
                nK = 6;
                nR = 5;
            }
        }

        public void Execute()
        {
            byte[] tiles = Board.Game.tiles;
            //Board.CheckForStuff(Board.Game, this);

            if (king * Board.aiColor == 6)
            {
                tiles[(byte)(7 + nR)] = tiles[(byte)(7 + rookX)];
                tiles[(byte)(7 + nK)] = king;
            }
            else
            {
                tiles[(byte)(0 + nR)] = tiles[(byte)(0 + rookX)];
                tiles[(byte)(0 + nK)] = king;
            }
            tiles[(byte)(0 + rookX)] = 0;
            tiles[(byte)(0 + 4)] = 0;
        }

        public void ExecuteOnBoard(Board temp)
        {
            byte[] tiles = temp.tiles;
            //Board.CheckForStuff(temp, this);

            if (king * Board.aiColor == 6)
            {
                tiles[(byte)(7 + nR)] = tiles[(byte)(7 + rookX)];
                tiles[(byte)(7 + nK)] = king;
            }
            else
            {
                tiles[(byte)(0 + nR)] = tiles[(byte)(0 + rookX)];
                tiles[(byte)(0 + nK)] = king;
            }
            tiles[(byte)(0 + rookX)] = 0;
            tiles[(byte)(0 + 4)] = 0;
        }

        public void Undo()
        {

        }


        public string GetSide()
        {
            throw new System.NotImplementedException();
        }
        /*
        private int[] origin;
        public int[] Origin { get { return origin; } }
        private int[] target;
        public int[] Target { get { return target; } set { target = value; } }
        private int[] toKill;
        public int[] ToKill { get { return toKill; } set { toKill = value; } }

        public Castling(int[] origin)
        {

        }

        public void Execute()
        {

        }
        public void ExecuteOnBoard(Board temp)
        {

        }
        public void Undo()
        {

        }
        */
    }
}
