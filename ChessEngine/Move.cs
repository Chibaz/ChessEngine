namespace Chess
{
    public interface IMove
    {
        void Execute();
        void ExecuteOnBoard(Board temp);
        void Undo();
    }

    public struct MovingPiece
    {
        public int[] Origin;
        public int[] Target;
        public int Piece;

        public MovingPiece(int[] origin, int[] target, int piece)
        {
            Origin = origin;
            Target = target;
            Piece = piece;
        }
    }

    public struct TakenPiece
    {
        public int[] Position;
        public int Piece;

        public TakenPiece(int[] position, int piece)
        {
            Position = position;
            Piece = piece;
        }
    }

    public class Move : IMove
    {
        public MovingPiece Moving;
        public TakenPiece Killing;
        public Move Next;

        public Move(int[] origin, int piece)
        {
            Moving = new MovingPiece(origin, null, piece);
            Killing = new TakenPiece(null, 0);
        }

        public void Execute()
        {
            Board.CheckForStuff(Board.Game, this);
            if (Killing.Position != null)
            {
                Board.Game.tiles[Killing.Position[0], Killing.Position[1]] = 0;
            }
            Board.Game.tiles[Moving.Target[0], Moving.Target[1]] = Moving.Piece;
            Board.Game.tiles[Moving.Origin[0], Moving.Origin[1]] = 0;
            //Board.CheckForCheck(Board.Game);
            Board.Game.CheckChecking(Board.Game/*, (Moving.Piece / Math.Abs(Moving.Piece))*-1*/);
        }

        public void ExecuteOnBoard(Board temp)
        {
            Board.CheckForStuff(temp, this);
            if (Killing.Position != null)
            {
                temp.tiles[Killing.Position[0], Killing.Position[1]] = 0;
            }
            temp.tiles[Moving.Target[0], Moving.Target[1]] = Moving.Piece;
            temp.tiles[Moving.Origin[0], Moving.Origin[1]] = 0;
            //Board.CheckForCheck(temp);
            Board.Game.CheckChecking(temp/*, (Moving.Piece / Math.Abs(Moving.Piece))*-1*/);
        }

        public void Undo()
        {
            Board.Game.tiles[Moving.Origin[0], Moving.Origin[1]] = Moving.Piece;
            Board.Game.tiles[Moving.Target[0], Moving.Target[1]] = 0;
            if (Killing.Position != null)
            {
                Board.Game.tiles[Killing.Position[0], Killing.Position[1]] = Killing.Piece;
            }
        }

        /*
        private int[] origin;
        public int[] Origin { get { return origin; } }
        private int[] target;
        public int[] Target { get { return target; } set { target = value; } }
        private int toMove;
        public int ToMove { get { return toMove; } set { toMove = value; } }
        private int toKill;
        public int ToKill { get { return toKill; } set { toKill = value; } }

        public Move(int[] origin)
        {
            this.origin = origin;
            toMove = Board.Game.Tiles[origin[0], origin[1]];
        }

        public void Execute()
        {
            int[,] tiles = Board.Game.Tiles;
            toKill = tiles[target[0], target[1]];
            tiles[target[0], target[1]] = toMove;
            tiles[origin[0], origin[1]] = 0;
        }
        
        public void ExecuteOnBoard(Board temp)
        {
            int[,] tiles = temp.Tiles;
            if (toKill != null)
            {
                //tiles[toKill[0], toKill[1]] = 0;
            }
            tiles[target[0], target[1]] = toMove;
            tiles[origin[0], origin[1]] = 0;
        }

        public void Undo()
        {
            int[,] tiles = Board.Game.Tiles;
            tiles[target[0], target[1]] = toKill;
            tiles[origin[0], origin[1]] = toMove;
        }
        */
    }

    public class Castling : IMove
    {
        public int king;
        public int rookX;
        public int nK, nR;

        public Castling(int k, int rX)
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
            int[,] tiles = Board.Game.tiles;
            Board.CheckForStuff(Board.Game, this);

            if (king * Board.aiColor == 6)
            {
                tiles[7, nR] = tiles[7, rookX];
                tiles[7, nK] = king;
            }
            else
            {
                tiles[0, nR] = tiles[0, rookX];
                tiles[0, nK] = king;
            }
            tiles[7, rookX] = 0;
            tiles[7, 4] = 0;
        }

        public void ExecuteOnBoard(Board temp)
        {
            int[,] tiles = temp.tiles;
            Board.CheckForStuff(temp, this);

            if (king * Board.aiColor > 0)
            {
                tiles[7, nR] = tiles[7, rookX];
                tiles[7, nK] = king;
            }
            else
            {
                tiles[0, nR] = tiles[0, rookX];
                tiles[0, nK] = king;
            }
            tiles[0, rookX] = 0;
            tiles[0, 4] = 0;
        }

        public void Undo()
        {

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
