using System;
using System.Collections.Generic;

namespace ChessEngine.Engine
{
    public class MoveGenerator
    {
        private Board _moveBoard;

        public List<IMove> GetAllMovesForPlayer(Board board, int player)
        {
            _moveBoard = board;
            var allMoves = new List<IMove>();
            if (_moveBoard.mate != 0)
            {
                Console.WriteLine("hit leaf");
                return allMoves;
            }/*else if(Board.CheckForCheck(moveBoard, player){
                return allMoves;
            }*/
            for (int h = 0; h < 8; h++)
            {
                for (int w = 0; w < 8; w++)
                {
                    if (Board.Game.tiles[h, w] * player > 0)
                    {
                        allMoves.AddRange(GetLegalMovements(new int[] { h, w }));
                    }
                }
            }
            //Console.WriteLine("number of moves for board is :" + allMoves.Count);
            return allMoves;
        }

        public List<IMove> GetLegalMovements(int[] origin)
        {
            var moves = new List<IMove>();
            var piece = Math.Abs(_moveBoard.tiles[origin[0], origin[1]]);

            if (piece == 1)
            {
                moves.AddRange(GetPawnMoves(origin));
            }
            if (piece == 2 || piece == 5) //Movement in straight lines
            {
                moves.AddRange(GetStraightMoves(origin));
            }
            if (piece == 4 || piece == 5) //Movement in diagonal lines
            {
                moves.AddRange(GetDiagonalMoves(origin));
            }
            if (piece == 3 || piece == 6) //Movement is an absolute distance
            {
                moves.AddRange(GetAbsoluteMoves(origin));
            }
            return moves;
        }

        public List<IMove> GetStraightMoves(int[] origin)
        {
            Move newMove;
            var straightMoves = new List<IMove>();
            for (var y = origin[0] + 1; y < 8; y++) //Vertical lower
            {
                newMove = new Move(origin, _moveBoard.tiles[origin[0], origin[1]])
                {
                    Moving = {Target = new[] {y, origin[1]}}
                };
                if (_moveBoard.tiles[newMove.Moving.Target[0], newMove.Moving.Target[1]] == 0)
                {
                    straightMoves.Add(newMove);
                }
                else
                {
                    if (CheckForKill(newMove))
                    {
                        straightMoves.Add(newMove);
                    }
                    break;
                }
            }
            for (var y = origin[0] - 1; y >= 0; y--) //Vertical upper
            {
                newMove = new Move(origin, _moveBoard.tiles[origin[0], origin[1]]);
                var target = new[] { y, origin[1] };
                newMove.Moving.Target = target;
                if (_moveBoard.tiles[target[0], target[1]] == 0)
                {
                    straightMoves.Add(newMove);
                }
                else
                {
                    if (CheckForKill(newMove))
                    {
                        straightMoves.Add(newMove);
                    }
                    break;
                }
            }
            for (var x = origin[1] + 1; x < 8; x++) //Horizontal right
            {
                newMove = new Move(origin, _moveBoard.tiles[origin[0], origin[1]]);
                var target = new[] { origin[0], x };
                newMove.Moving.Target = target;
                if (_moveBoard.tiles[target[0], target[1]] == 0)
                {
                    straightMoves.Add(newMove);
                }
                else
                {
                    if (CheckForKill(newMove))
                    {
                        straightMoves.Add(newMove);
                    }
                    break;
                }
            }
            for (var x = origin[1] - 1; x >= 0; x--) //Horizontal left
            {
                newMove = new Move(origin, _moveBoard.tiles[origin[0], origin[1]]);
                var target = new[] { origin[0], x };
                newMove.Moving.Target = target;
                if (_moveBoard.tiles[target[0], target[1]] == 0)
                {
                    straightMoves.Add(newMove);
                }
                else
                {
                    if (CheckForKill(newMove))
                    {
                        straightMoves.Add(newMove);
                    }
                    break;
                }
            }
            return straightMoves;
        }

        public List<IMove> GetDiagonalMoves(int[] origin)
        {
            var xR = origin[1];
            var xL = origin[1];
            Move newMove;
            var diagonalMoves = new List<IMove>();
            for (var y = origin[0] + 1; y < 8; y++) //Lower-Left diagonals
            {
                xL--;
                newMove = new Move(origin, _moveBoard.tiles[origin[0], origin[1]]);
                newMove.Moving.Target = new int[] { y, xL };
                if (xL >= 0 && Board.Game.tiles[y, xL] == 0)
                {
                    diagonalMoves.Add(newMove);
                }
                else if (xL >= 0)
                {
                    if (CheckForKill(newMove))
                    {
                        diagonalMoves.Add(newMove);
                    }
                    break;
                }
            }
            for (var y = origin[0] + 1; y < 8; y++) //Lower-Right diagonals
            {
                xR++;
                newMove = new Move(origin, _moveBoard.tiles[origin[0], origin[1]]);
                newMove.Moving.Target = new int[] { y, xR };
                if (xR < 8 && _moveBoard.tiles[y, xR] == 0)
                {
                    diagonalMoves.Add(newMove);
                }
                else if (xR <= 7)
                {
                    if (CheckForKill(newMove))
                    {
                        diagonalMoves.Add(newMove);
                    }
                    break;
                }
            }

            xL = xR = origin[1];
            //leftUnbroken = rightUnbroken = true;
            for (var y = origin[0] - 1; y >= 0; y--) //Upper-Left diagonals
            {
                xL--;
                newMove = new Move(origin, _moveBoard.tiles[origin[0], origin[1]]) {Moving = {Target = new[] {y, xL}}};
                if (xL >= 0 && _moveBoard.tiles[y, xL] == 0)
                {
                    diagonalMoves.Add(newMove);
                }
                else if (xL >= 0)
                {
                    if (CheckForKill(newMove))
                    {
                        diagonalMoves.Add(newMove);
                    }
                    break;
                }
            }
            for (var y = origin[0] - 1; y >= 0; y--){ //Upper-Right diagonals
                xR++;
                newMove = new Move(origin, _moveBoard.tiles[origin[0], origin[1]]) {Moving = {Target = new[] {y, xR}}};
                if (xR < 8 && _moveBoard.tiles[y, xR] == 0)
                {
                    diagonalMoves.Add(newMove);
                }
                else if (xR <= 7)
                {
                    if (CheckForKill(newMove))
                    {
                        diagonalMoves.Add(newMove);
                    }
                    break;
                }
            }
            return diagonalMoves;
        }

        public List<IMove> GetAbsoluteMoves(int[] origin)
        {
            String[] moves = null;
            var piece = _moveBoard.tiles[origin[0], origin[1]];
            var absMoves = new List<IMove>();
            switch (Math.Abs(piece))
            {
                case 3:
                    moves = new[] { "2,1", "1,2", "2,-1", "1,-2", "-2,1", "-1,2", "-2,-1", "-1,-2" };
                    break;
                case 6:
                    moves = new[] { "1,0", "-1,0", "0,1", "0,-1", "1,1", "1,-1", "-1,1", "-1,-1" };
                    absMoves.AddRange(GenerateCastling(piece / Math.Abs(piece)));
                    break;
            }
            foreach (String s in moves)
            {
                var newMove = new Move(origin, piece);
                var m = s.Split(new Char[] { ',' }, 2);
                var y = origin[0] + int.Parse(m[0]);
                var x = origin[1] + int.Parse(m[1]);
                if (y < 0 || x < 0 || y >= 8 || x >= 8) continue;
                newMove.Moving.Target = new[] { y, x };
                if (_moveBoard.tiles[newMove.Moving.Target[0], newMove.Moving.Target[1]] == 0)
                {
                    absMoves.Add(newMove);
                }
                else if (CheckForKill(newMove))
                {
                    absMoves.Add(newMove);
                }
            }
            return absMoves;
        }

        public List<IMove> GetPawnMoves(int[] origin)
        {
            var piece = _moveBoard.tiles[origin[0], origin[1]];
            var pawnMoves = new List<IMove>();
            var direction = piece * Board.aiColor;
            Move newMove;

            //Move one space
            if (_moveBoard.tiles[origin[0] - direction, origin[1]] == 0)
            {
                newMove = new Move(origin, piece) {Moving = {Target = new[] {origin[0] - direction, origin[1]}}};
                pawnMoves.Add(newMove);
                //Move ahead two spaces from start
                if ((origin[0] == 6 && piece * Board.aiColor == 1) && _moveBoard.tiles[origin[0] - (2 * direction), origin[1]] == 0)
                {
                    newMove = new Move(origin, piece) {Moving = {Target = new[] {origin[0] - (2*direction), origin[1]}}};
                    pawnMoves.Add(newMove);
                }
                if ((origin[0] == 1 && piece * Board.aiColor == -1) && _moveBoard.tiles[origin[0] - (2 * direction), origin[1]] == 0)
                {
                    newMove = new Move(origin, piece) {Moving = {Target = new[] {origin[0] - (2*direction), origin[1]}}};
                    pawnMoves.Add(newMove);
                }
            }
            //Kill piece right
            if (origin[1] < 7 && _moveBoard.tiles[origin[0] - direction, origin[1] + 1] * piece < 0)
            {
                newMove = new Move(origin, piece) {Moving = {Target = new[] {origin[0] - direction, origin[1] + 1}}};
                newMove.Killing = new TakenPiece(newMove.Moving.Target, piece * -1);
                //newMove.Killing.Position = new int[] { origin[0] - direction, origin[1] + 1 };
                //newMove.Killing.Piece = moveBoard.tiles[origin[0] - direction, origin[1] + 1];
                //CheckForPromotion(newMove);
                pawnMoves.Add(newMove);
            }
            //Kill piece left
            if (origin[1] > 0 && _moveBoard.tiles[origin[0] - direction, origin[1] - 1] * piece < 0)
            {
                newMove = new Move(origin, piece) {Moving = {Target = new[] {origin[0] - direction, origin[1] - 1}}};
                newMove.Killing = new TakenPiece(newMove.Moving.Target, piece * -1);
                //newMove.Killing.Position = new int[] { origin[0] - direction, origin[1] - 1 };
                //newMove.Killing.Piece = Board.Game.tiles[origin[0] - direction, origin[1] - 1];
                //CheckForPromotion(newMove);
                pawnMoves.Add(newMove);
            }
            //EnPassant
            if (_moveBoard.EnPassant != null)
            {
                if (_moveBoard.tiles[_moveBoard.EnPassant[0], _moveBoard.EnPassant[1]] * Board.aiColor == -1 && origin[0] == 3 && (_moveBoard.EnPassant[1] + 1 == origin[1] || _moveBoard.EnPassant[1] - 1 == origin[1]))
                {
                    newMove = new Move(origin, piece)
                    {
                        Moving = {Target = new[] {_moveBoard.EnPassant[0] - direction, _moveBoard.EnPassant[1]}},
                        Killing = new TakenPiece(_moveBoard.EnPassant, piece*-1)
                    };
                    //newMove.Killing.Position = moveBoard.EnPassant;
                    //newMove.Killing.Piece = moveBoard.tiles[moveBoard.EnPassant[0], moveBoard.EnPassant[1]];
                    pawnMoves.Add(newMove);
                }
                else if (_moveBoard.tiles[_moveBoard.EnPassant[0], _moveBoard.EnPassant[1]] * Board.aiColor == 1 && origin[0] == 4 && (_moveBoard.EnPassant[1] + 1 == origin[1] || _moveBoard.EnPassant[1] - 1 == origin[1]))
                {
                    newMove = new Move(origin, piece)
                    {
                        Moving = {Target = new[] {_moveBoard.EnPassant[0] - direction, _moveBoard.EnPassant[1]}},
                        Killing = new TakenPiece(_moveBoard.EnPassant, piece*-1)
                    };
                    //newMove.Killing.Position = moveBoard.EnPassant;
                    //newMove.Killing.Piece = moveBoard.tiles[moveBoard.EnPassant[0], moveBoard.EnPassant[1]];
                    pawnMoves.Add(newMove);
                }
            }
            return pawnMoves;
            /*if(piece * Board.aiColor == -1)
            {
                if (Board.Game.tiles[origin[0] + 1, origin[1]] == 0)
                {
                    newMove = new Move(origin, piece);
                    newMove.Moving.Target = new int[] { origin[0] - 1, origin[1] };
                    pawnMoves.Add(newMove);
                }
                if (Board.Game.tiles[origin[0] - 2, origin[1]] == 0)
                {
                    newMove = new Move(origin, piece);
                    newMove.Moving.Target = new int[] { origin[0] - 2, origin[1] };
                    pawnMoves.Add(newMove);
                }
                if (Board.Game.tiles[origin[0] - 1, origin[1] + 1] * piece < 0)
                {
                    newMove = new Move(origin, piece);
                    newMove.Moving.Target = new int[] { origin[0] + 1, origin[1] + 1 };
                    newMove.Killing.Position = new int[] { origin[0] + 1, origin[1] + 1 };
                    newMove.Killing.Piece = Board.Game.tiles[origin[0] + 1, origin[1] + 1];
                    pawnMoves.Add(newMove);
                }
                if (Board.Game.tiles[origin[0] - 1, origin[1] - 1] * piece < 0)
                {
                    newMove = new Move(origin, piece);
                    newMove.Moving.Target = new int[] { origin[0] - 1, origin[1] - 1 };
                    newMove.Killing.Position = new int[] { origin[0] - 1, origin[1] - 1 };
                    newMove.Killing.Piece = Board.Game.tiles[origin[0] - 1, origin[1] - 1];
                    pawnMoves.Add(newMove);
                }
            }*/
        }

        
        public List<IMove> GenerateCastling(int player)
        {
            var castling = new List<IMove>();
            Castling castle;
            int y;
            Boolean leftCastle, rightCastle;
            if (player == Board.aiColor)
            {
                y = 7;
                leftCastle = _moveBoard.aiLeftCastling;
                rightCastle = _moveBoard.aiRightCastling;
            }else{
                y = 0;
                leftCastle = _moveBoard.playerLeftCastling;
                rightCastle = _moveBoard.playerRightCastling;
            }
                var king = 6 * player;
                if (leftCastle && _moveBoard.tiles[y, 0] == 2*player && _moveBoard.tiles[y, 1] == 0 && _moveBoard.tiles[y, 2] == 0 && _moveBoard.tiles[y, 3] == 0 && _moveBoard.tiles[y, 4] == 6)
                {
                    if (!Board.CheckForCheck(_moveBoard, player, new[] { y, 2 }) && !Board.CheckForCheck(_moveBoard, player, new[] { y, 3 }))
                    {
                        castle = new Castling(king, y);
                        castling.Add(castle);
                    }
                }
                if (rightCastle && _moveBoard.tiles[y, 7] == 2*player && _moveBoard.tiles[y, 6] == 0 && _moveBoard.tiles[y, 5] == 0 && _moveBoard.tiles[y, 4] == 6)
                {
                    if (!Board.CheckForCheck(_moveBoard, player, new[] { y, 5 }) && !Board.CheckForCheck(_moveBoard, player, new[] { y, 6 }))
                    {
                        castle = new Castling(king, y);
                        castling.Add(castle);
                    }
                }
            /*}
            else
            {
                int king = 6 * -Board.aiColor;
                if (moveBoard.aiLeftCastling && moveBoard.tiles[0, 1] == 0 && moveBoard.tiles[0, 2] == 0 && moveBoard.tiles[0, 3] == 0)
                {
                    castle = new Castling(king, 0);
                    castling.Add(castle);
                }
                if (moveBoard.aiRightCastling && moveBoard.tiles[7, 6] == 0 && moveBoard.tiles[7, 5] == 0)
                {
                    castle = new Castling(king, 7);
                    castling.Add(castle);
                }
            }*/
            return castling;
        }

        /*
        public void CheckForPromotion(Move move)
        {
            if (move.Moving.Piece * Board.aiColor == 1 && move.Moving.Target[0] == 0)
            {
                move.Moving.Piece *= 5;
            }
            else if (move.Moving.Piece * Board.aiColor == -1 && move.Moving.Target[0] == 7)
            {
                move.Moving.Piece *= 5;
            }
        }*/

        public Boolean CheckForKill(Move move)
        {
            var piece = _moveBoard.tiles[move.Moving.Target[0], move.Moving.Target[1]];
            if ((move.Moving.Piece*_moveBoard.tiles[move.Moving.Target[0], move.Moving.Target[1]]) >= 0) return false;
            move.Killing = new TakenPiece(move.Moving.Target, piece);
            //Console.WriteLine("can take " + move.Killing.Piece + " at " + move.Killing.Position[0] + "," + move.Killing.Position[1]);
            //move.Killing.Position = move.Moving.Target;
            //move.Killing.Piece = move.Moving.Piece;
            return true;
        }
    }
}
