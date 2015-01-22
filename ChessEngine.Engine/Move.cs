namespace ChessEngine.Engine
{
    public interface IMove
    {
        void Execute();
        void ExecuteOnBoard(Board temp);
        void Undo();
    }

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
        }

        public void ExecuteOnBoard(Board temp)
        {
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
        private readonly byte _kingOrigin;
        private readonly byte _rookOrigin;
        private readonly int _kingTarget;
        private readonly int _rookTarget;
        private readonly byte _king;
        private readonly byte _rook;
        public int RookFile;

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
        }

        public void ExecuteOnBoard(Board temp)
        {
            temp.Tiles[_kingOrigin] = 0;
            temp.Tiles[_rookOrigin] = 0;
            temp.Tiles[_kingTarget] = _king;
            temp.Tiles[_rookTarget] = _rook;
            temp.LastMovedPiece = (byte)_rookTarget;

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
