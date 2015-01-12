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
                    if (Board.Game.tiles[16 * h + w] * player > 0)
                    {
                        allMoves.AddRange(GetLegalMovements((byte)(h + w)));
                    }
                }
            }
            //Console.WriteLine("number of moves for board is :" + allMoves.Count);
            return allMoves;
        }

        public List<IMove> GetLegalMovements(byte origin)
        {
            var moves = new List<IMove>();
            var piece = Math.Abs(_moveBoard.tiles[origin]);

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

        public List<IMove> GetStraightMoves(byte origin)
        {
            Move newMove;
            List<IMove> straightMoves = new List<IMove>();
            byte lower, upper, left, right;
            lower = upper = left = right = 0x00;
            for (int v = 1; v < 8; v++)
            {
                lower = upper = left = right = 0x00;
                //Lower
                lower = (byte)(v + (0x07 & origin));
                newMove = new Move(lower, _moveBoard.tiles[lower]);
                if ((0x88 & lower) == 0)
                {
                    if (_moveBoard.tiles[lower] == 0)
                    {
                        straightMoves.Add(newMove);
                    }
                    else if (CheckForKill(newMove))
                    {
                        straightMoves.Add(newMove);
                    }
                }
                //Upper
                upper = (byte)(-v + (0x07 & origin));
                newMove = new Move(upper, _moveBoard.tiles[upper]);
                if ((0x88 & upper) == 0)
                {
                    if (_moveBoard.tiles[upper] == 0)
                    {
                        straightMoves.Add(newMove);
                    }
                    else if (CheckForKill(newMove))
                    {
                        straightMoves.Add(newMove);
                    }
                }
                //Left
                left = (byte)((0x70 & origin) + v);
                newMove = new Move(left, _moveBoard.tiles[left]);
                if ((0x88 & left) == 0)
                {
                    if (_moveBoard.tiles[left] == 0)
                    {
                        straightMoves.Add(newMove);

                    }
                    else if (CheckForKill(newMove))
                    {
                        straightMoves.Add(newMove);
                    }
                }
                //Right
                right = (byte)((0x70 & origin) + (-v));
                newMove = new Move(right, _moveBoard.tiles[right]);
                if ((0x88 & right) == 0)
                {
                    if (_moveBoard.tiles[right] == 0)
                    {
                        straightMoves.Add(newMove);

                    }
                    else if (CheckForKill(newMove))
                    {
                        straightMoves.Add(newMove);
                    }
                }
            }
            return straightMoves;
        }

        //    do
        //    {
        //    } while ((target & 0x88) == 0);

        //    for (var y = origin[0] + 1; y < 8; y++) //Vertical lower
        //    {
        //        newMove = new Move(origin, _moveBoard.GetSpecificTile())
        //        {
        //            Moving = { Target = new[] { y, origin[1] } }
        //        };
        //        if (_moveBoard.tiles[newMove.Moving.Target[0], newMove.Moving.Target[1]] == 0)
        //        {
        //            straightMoves.Add(newMove);
        //        }
        //        else
        //        {
        //            if (CheckForKill(newMove))
        //            {
        //                straightMoves.Add(newMove);
        //            }
        //            break;
        //        }
        //    }
        //    for (var y = origin[0] - 1; y >= 0; y--) //Vertical upper
        //    {
        //        newMove = new Move(origin, _moveBoard.tiles[origin[0], origin[1]]);
        //        var target = new[] { y, origin[1] };
        //        newMove.Moving.Target = target;
        //        if (_moveBoard.tiles[target[0], target[1]] == 0)
        //        {
        //            straightMoves.Add(newMove);
        //        }
        //        else
        //        {
        //            if (CheckForKill(newMove))
        //            {
        //                straightMoves.Add(newMove);
        //            }
        //            break;
        //        }
        //    }
        //    for (var x = origin[1] + 1; x < 8; x++) //Horizontal right
        //    {
        //        newMove = new Move(origin, _moveBoard.tiles[origin[0], origin[1]]);
        //        var target = new[] { origin[0], x };
        //        newMove.Moving.Target = target;
        //        if (_moveBoard.tiles[target[0], target[1]] == 0)
        //        {
        //            straightMoves.Add(newMove);
        //        }
        //        else
        //        {
        //            if (CheckForKill(newMove))
        //            {
        //                straightMoves.Add(newMove);
        //            }
        //            break;
        //        }
        //    }
        //    for (var x = origin[1] - 1; x >= 0; x--) //Horizontal left
        //    {
        //        newMove = new Move(origin, _moveBoard.tiles[origin[0], origin[1]]);
        //        var target = new[] { origin[0], x };
        //        newMove.Moving.Target = target;
        //        if (_moveBoard.tiles[target[0], target[1]] == 0)
        //        {
        //            straightMoves.Add(newMove);
        //        }
        //        else
        //        {
        //            if (CheckForKill(newMove))
        //            {
        //                straightMoves.Add(newMove);
        //            }
        //            break;
        //        }
        //    }
        //    return straightMoves;
        //}

        public List<IMove> GetDiagonalMoves(byte origin)
        {
            Move newMove = null;
            List<IMove> diagonalMoves = new List<IMove>();
            byte lowerLeft, lowerRight, upperLeft, upperRight;
            for (int v = 1; v < 8; v++)
            {
                //UpperRight
                upperRight = (byte) (((0x70 & origin) + v) + ((0x07 & origin) + v));
                newMove = new Move(upperRight, _moveBoard.tiles[upperRight]);
                if ((0x88 & upperRight) == 0)
                {
                    if (_moveBoard.tiles[upperRight] == 0)
                    {
                        diagonalMoves.Add(newMove);
                    }
                    else if (CheckForKill(newMove))
                    {
                        diagonalMoves.Add(newMove);
                    }
                }
                //UpperLeft
                upperLeft = (byte) (((0x70 & origin) + v) + ((0x07 & origin) - v));
                newMove = new Move(upperLeft, _moveBoard.tiles[upperRight]);
                if ((0x88 & upperLeft) == 0)
                {
                    if (_moveBoard.tiles[upperLeft] == 0)
                    {
                        diagonalMoves.Add(newMove);

                    }
                    else if (CheckForKill(newMove))
                    {
                        diagonalMoves.Add(newMove);
                    }
                }
                //LowerRight
                lowerRight = (byte) (((0x70 & origin) - v) + ((0x07 & origin) + v));
                newMove = new Move(lowerRight, _moveBoard.tiles[upperRight]);
                if ((0x88 & lowerRight) == 0)
                {
                    if (_moveBoard.tiles[lowerRight] == 0)
                    {
                        diagonalMoves.Add(newMove);
                    }
                    else if (CheckForKill(newMove))
                    {
                        diagonalMoves.Add(newMove);
                    }
                }
                //LowerLeft
                lowerLeft = (byte) (((0x70 & origin) - v) + ((0x07 & origin) - v));
                newMove = new Move(lowerLeft, _moveBoard.tiles[upperRight]);
                if ((0x88 & lowerLeft) == 0)
                {
                    if (_moveBoard.tiles[lowerLeft] == 0)
                    {
                        diagonalMoves.Add(newMove);
                    }
                    else if (CheckForKill(newMove))
                    {
                        diagonalMoves.Add(newMove);
                    }
                }
            }
            return diagonalMoves;
        }


        public List<IMove> GetAbsoluteMoves(byte origin)
        {
            String[] moves = null;
            byte piece = _moveBoard.tiles[origin];
            var absMoves = new List<IMove>();
            switch (Math.Abs(piece))
            {
                case 3:
                    moves = new[] { "2,1", "1,2", "2,-1", "1,-2", "-2,1", "-1,2", "-2,-1", "-1,-2" };
                    break;
                case 6:
                    moves = new[] { "1,0", "-1,0", "0,1", "0,-1", "1,1", "1,-1", "-1,1", "-1,-1" };
//                    absMoves.AddRange(GenerateCastling(piece / Math.Abs(piece)));
                    break;
            }
            foreach (String s in moves)
            {
                string[] m = s.Split(new [] { ',' }, 2);
                byte target = (byte)((0x70 & origin + int.Parse(m[0])) + (0x07 & origin + int.Parse(m[1])));
                if ((0x88 & target) == 0) continue;
                Move newMove = new Move(origin, piece);
                newMove.Moving.Target = target;
                if (_moveBoard.tiles[target] == 0)
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

        public List<IMove> GetPawnMoves(byte origin)
        {
            byte piece = _moveBoard.tiles[origin];
            byte target;
            var pawnMoves = new List<IMove>();
            var direction = piece * Board.aiColor;
            Move newMove;

            //Move one space
            if (_moveBoard.tiles[origin + (16*direction)] == 0)
            {
                newMove = new Move(origin, piece);
                pawnMoves.Add(newMove);
                //Move ahead two spaces from start
                int rank = origin & 0x70;
                if (((rank == 6 && piece * Board.aiColor == 1) && _moveBoard.tiles[origin - 32] == 0) || ((rank == 1 && piece * Board.aiColor == 1) && _moveBoard.tiles[origin + 32] == 0))
                {
                    newMove = new Move(origin, piece);
                    newMove.target = (byte)(((0x70 & origin) + 0x20) + (0x07 & origin));
                    pawnMoves.Add(newMove);
                }
            }
            //Kill piece right
            target = (byte) (((0x70 & origin) + direction) + ((0x07 & origin) + 1));
            if ((target & 0x88) == 0)
            {
                newMove = new Move(origin, piece);
                if (CheckForKill(newMove))
                {
                    pawnMoves.Add(newMove);
                }
                //newMove.Killing = new TakenPiece(newMove.Moving.Target, piece * -1);
                //newMove.Killing.Position = new int[] { origin[0] - direction, origin[1] + 1 };
                //newMove.Killing.Piece = moveBoard.tiles[origin[0] - direction, origin[1] + 1];
                //CheckForPromotion(newMove);
            }
            //Kill piece left
            target = (byte)(((0x70 & origin) + direction) + ((0x07 & origin) - 1));
            if ((target & 0x88) == 0)
            {
                newMove = new Move(origin, piece);
                if (CheckForKill(newMove))
                {
                    pawnMoves.Add(newMove);
                }
//                newMove = new Move(origin, piece) { Moving = { Target = new[] { origin[0] - direction, origin[1] - 1 } } };
//                newMove.Killing = new TakenPiece(newMove.Moving.Target, piece * -1);
                //newMove.Killing.Position = new int[] { origin[0] - direction, origin[1] - 1 };
                //newMove.Killing.Piece = Board.Game.tiles[origin[0] - direction, origin[1] - 1];
                //CheckForPromotion(newMove);
            }
            //EnPassant
            /*
            if (_moveBoard.EnPassant != null)
            {
                if (_moveBoard.tiles[_moveBoard.EnPassant[0], _moveBoard.EnPassant[1]] * Board.aiColor == -1 && origin[0] == 3 && (_moveBoard.EnPassant[1] + 1 == origin[1] || _moveBoard.EnPassant[1] - 1 == origin[1]))
                {
                    newMove = new Move(origin, piece)
                    {
                        Moving = { Target = new[] { _moveBoard.EnPassant[0] - direction, _moveBoard.EnPassant[1] } },
                        Killing = new TakenPiece(_moveBoard.EnPassant, piece * -1)
                    };
                    //newMove.Killing.Position = moveBoard.EnPassant;
                    //newMove.Killing.Piece = moveBoard.tiles[moveBoard.EnPassant[0], moveBoard.EnPassant[1]];
                    pawnMoves.Add(newMove);
                }
                else if (_moveBoard.tiles[_moveBoard.EnPassant[0], _moveBoard.EnPassant[1]] * Board.aiColor == 1 && origin[0] == 4 && (_moveBoard.EnPassant[1] + 1 == origin[1] || _moveBoard.EnPassant[1] - 1 == origin[1]))
                {
                    newMove = new Move(origin, piece)
                    {
                        Moving = { Target = new[] { _moveBoard.EnPassant[0] - direction, _moveBoard.EnPassant[1] } },
                        Killing = new TakenPiece(_moveBoard.EnPassant, piece * -1)
                    };
                    //newMove.Killing.Position = moveBoard.EnPassant;
                    //newMove.Killing.Piece = moveBoard.tiles[moveBoard.EnPassant[0], moveBoard.EnPassant[1]];
                    pawnMoves.Add(newMove);
                }
            }
             * */
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

        public Move GenerateSimpleMove(byte origin, byte target)
        {
            Move newMove = new Move(origin, _moveBoard.tiles[origin]);

            if (_moveBoard.tiles[target] == 0)
            {
                return newMove;
            }
            if (CheckForKill(newMove))
            {
                return newMove;
            }

            return null;
        }

        /*
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
            }
            else
            {
                y = 0;
                leftCastle = _moveBoard.playerLeftCastling;
                rightCastle = _moveBoard.playerRightCastling;
            }
            var king = 6 * player;
            if (leftCastle && _moveBoard.tiles[y, 0] == 2 * player && _moveBoard.tiles[y, 1] == 0 && _moveBoard.tiles[y, 2] == 0 && _moveBoard.tiles[y, 3] == 0 && _moveBoard.tiles[y, 4] == 6)
            {
                if (!Board.CheckForCheck(_moveBoard, player, new[] { y, 2 }) && !Board.CheckForCheck(_moveBoard, player, new[] { y, 3 }))
                {
                    castle = new Castling(king, y);
                    castling.Add(castle);
                }
            }
            if (rightCastle && _moveBoard.tiles[y, 7] == 2 * player && _moveBoard.tiles[y, 6] == 0 && _moveBoard.tiles[y, 5] == 0 && _moveBoard.tiles[y, 4] == 6)
            {
                if (!Board.CheckForCheck(_moveBoard, player, new[] { y, 5 }) && !Board.CheckForCheck(_moveBoard, player, new[] { y, 6 }))
                {
                    castle = new Castling(king, y);
                    castling.Add(castle);
                }
            }
         * */
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
            }
            return castling;
        }*/

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
            byte piece = _moveBoard.tiles[move.target];
            if ((piece & 0x08) == (move.piece & 0x08)) return false;
            move.Killing = new TakenPiece(move.Moving.Target, piece);
            //Console.WriteLine("can take " + move.Killing.Piece + " at " + move.Killing.Position[0] + "," + move.Killing.Position[1]);
            //move.Killing.Position = move.Moving.Target;
            //move.Killing.Piece = move.Moving.Piece;
            return true;
        }
    }
}
