using System;
using System.Collections.Generic;

namespace ChessEngine.Engine
{
    public class MoveGenerator
    {
        private Board _moveBoard;
        private byte _cPlayer;

        public List<IMove> GetAllMovesForPlayer(Board board, byte player)
        {
            _cPlayer = player;
            _moveBoard = board;
            var allMoves = new List<IMove>();
            if (_moveBoard.Mate != 0)
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
                    if ((Board.Game.Tiles[(byte)(16*h + w)] & 0x88) == _cPlayer)
                    {
                        allMoves.AddRange(GetLegalMovements((byte)(16*h + w)));
                    }
                }
            }
            //Console.WriteLine("number of moves for board is :" + allMoves.Count);
            return allMoves;
        }

        public List<IMove> GetLegalMovements(byte origin)
        {
            List<IMove> moves = new List<IMove>();
            byte piece = _moveBoard.Tiles[origin];

            switch (piece)
            {
                    //pawn
                case 0x01: case 0x09:
                    moves.AddRange(GetPawnMoves(origin));
                    break;
                    //knight
                case 0x02: case 0x0A:
                    moves.AddRange(GetAbsoluteMoves(origin));
                    break;
                    //king
                case 0x03: case 0x0B:
                    moves.AddRange(GetAbsoluteMoves(origin));
                    break;
                    //bishop
                case 0x05: case 0x0D:
                    moves.AddRange(GetDiagonalMoves(origin));
                    break;
                    //rook
                case 0x06: case 0x0E:
                    moves.AddRange(GetStraightMoves(origin));
                    break;
                    //queen
                case 0x07: case 0x0F:
                    moves.AddRange(GetStraightMoves(origin));
                    moves.AddRange(GetDiagonalMoves(origin));
                    break;
            }
            return moves;
        }

        public List<IMove> GetStraightMoves(byte origin)
        {
            Move newMove;
            List<IMove> straightMoves = new List<IMove>();
            Boolean upperBreak, lowerBreak, leftBreak, rightBreak;
            upperBreak = lowerBreak = leftBreak = rightBreak = false;
            for (int v = 1; v < 8; v++)
            {
                byte target = 0x00;
                //upper
                target = (byte)(16*v + origin);
                if ((0x88 & target) == 0 && !upperBreak)
                {
                    newMove = new Move(origin, _moveBoard.Tiles[origin]) {target = target};
                    if (_moveBoard.Tiles[target] == 0)
                    {
                        straightMoves.Add(newMove);
                    }
                    else if (CheckForKill(newMove))
                    {
                        straightMoves.Add(newMove);
                        upperBreak = true;
                    }
                    else
                    {
                        upperBreak = true;
                    }
                }
                //lower
                target = (byte)(-16*v + origin);
                if ((0x88 & target) == 0 && !lowerBreak)
                {
                    newMove = new Move(origin, _moveBoard.Tiles[origin]) {target = target};
                    if (_moveBoard.Tiles[target] == 0)
                    {
                        straightMoves.Add(newMove);
                    }
                    else if (CheckForKill(newMove))
                    {
                        straightMoves.Add(newMove);
                        lowerBreak = true;
                    }
                    else
                    {
                        lowerBreak = true;
                    }
                }
                //Left
                target = (byte)(origin + v);
                if ((0x88 & target) == 0 && !leftBreak)
                {
                    newMove = new Move(origin, _moveBoard.Tiles[origin]) {target = target};
                    if (_moveBoard.Tiles[target] == 0)
                    {
                        straightMoves.Add(newMove);

                    }
                    else if (CheckForKill(newMove))
                    {
                        straightMoves.Add(newMove);
                        leftBreak = true;
                    }
                    else
                    {
                        leftBreak = true;
                    }
                }
                //Right
                target = (byte)(origin - v);
                if ((0x88 & target) == 0 && !rightBreak)
                {
                    newMove = new Move(origin, _moveBoard.Tiles[origin]) {target = target};
                    if (_moveBoard.Tiles[target] == 0)
                    {
                        straightMoves.Add(newMove);
                    }
                    else if (CheckForKill(newMove))
                    {
                        straightMoves.Add(newMove);
                        rightBreak = true;
                    }
                    else
                    {
                        rightBreak = true;
                    }
                }
            }
            //Console.WriteLine("from " + origin + " piece: " + _moveBoard.Tiles[origin] + " has " + straightMoves.Count);
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
            Boolean upperRight, upperLeft, lowerRight, lowerLeft;
            upperRight = upperLeft = lowerRight = lowerLeft = false;
            for (int v = 1; v < 8; v++)
            {
                //UpperRight
                byte target = (byte) (origin + 16*v + v);
                if (!upperRight && (0x88 & target) == 0)
                {
                    newMove = new Move(origin, _moveBoard.Tiles[origin]) {target = target};
                    if (_moveBoard.Tiles[target] == 0)
                    {
                        diagonalMoves.Add(newMove);
                    }
                    else if (CheckForKill(newMove))
                    {
                        diagonalMoves.Add(newMove);
                        upperRight = true;
                    }
                    else
                    {
                        upperRight = true;
                    }
                }
                //UpperLeft
                target = (byte) (origin + 16*v - v);
                if (!upperLeft && (0x88 & target) == 0)
                {
                    newMove = new Move(origin, _moveBoard.Tiles[origin]) {target = target};
                    if (_moveBoard.Tiles[target] == 0)
                    {
                        diagonalMoves.Add(newMove);

                    }
                    else if (CheckForKill(newMove))
                    {
                        diagonalMoves.Add(newMove);
                        upperLeft = true;
                    }
                    else
                    {
                        upperLeft = true;
                    }
                }
                //LowerRight
                target = (byte) (origin - 16*v + v);
                if (!lowerRight && (0x88 & target) == 0)
                {
                    newMove = new Move(origin, _moveBoard.Tiles[origin]) {target = target};
                    if (_moveBoard.Tiles[target] == 0)
                    {
                        diagonalMoves.Add(newMove);
                    }
                    else if (CheckForKill(newMove))
                    {
                        diagonalMoves.Add(newMove);
                        lowerRight = true;
                    }
                    else
                    {
                        lowerRight = true;
                    }
                }
                //LowerLeft
                target = (byte) (origin - 16*v - v);
                if (!lowerLeft && (0x88 & target) == 0)
                {
                    newMove = new Move(origin, _moveBoard.Tiles[origin]) {target = target};
                    if (_moveBoard.Tiles[target] == 0)
                    {
                        diagonalMoves.Add(newMove);
                    }
                    else if (CheckForKill(newMove))
                    {
                        diagonalMoves.Add(newMove);
                        lowerLeft = true;
                    }
                    else
                    {
                        lowerLeft = true;
                    }
                }
            }
            //Console.WriteLine("from " + origin + " piece: " + _moveBoard.Tiles[origin] + " has " + diagonalMoves.Count);
            return diagonalMoves;
        }


        public List<IMove> GetAbsoluteMoves(byte origin)
        {
            string[] moves = null;
            byte piece = _moveBoard.Tiles[origin];
            var absMoves = new List<IMove>();
            switch (piece & 0x07)
            {
                case 0x02:
                    moves = new[] { "2,1", "1,2", "2,-1", "1,-2", "-2,1", "-1,2", "-2,-1", "-1,-2" };
                    break;
                case 0x03:
                    moves = new[] { "1,0", "-1,0", "0,1", "0,-1", "1,1", "1,-1", "-1,1", "-1,-1" };
//                    absMoves.AddRange(GenerateCastling(piece / Math.Abs(piece)));
                    break;
            }
            foreach (string s in moves)
            {
                string[] m = s.Split(new [] { ',' }, 2);
                byte target = (byte)(origin + 16*int.Parse(m[0])+ int.Parse(m[1]));
                if ((0x88 & target) == 0)
                {
                    //Console.WriteLine(origin + " " + target);
                    Move newMove = new Move(origin, piece) {target = target};
                    if (_moveBoard.Tiles[target] == 0)
                    {
                        absMoves.Add(newMove);
                    }
                    else if (CheckForKill(newMove))
                    {
                        absMoves.Add(newMove);
                    }
                }
            }
            //Console.WriteLine("from " + origin + " piece: " + _moveBoard.Tiles[origin] + " has " + absMoves.Count);
            return absMoves;
        }

        public List<IMove> GetPawnMoves(byte origin)
        {
            byte piece = _moveBoard.Tiles[origin];
            byte target;
            List<IMove> pawnMoves = new List<IMove>();
            int direction;
            if ((piece & 0x08) == _cPlayer)
            {
                direction = -16;
            }
            else
            {
                direction = 16;
            }
            Move newMove;

            //Move one space
            target = (byte)(direction + origin);
            if ((target & 0x88) == 0)
            {
                newMove = new Move(origin, piece);
                newMove.target = target;
                if (_moveBoard.Tiles[target] == 0)
                {
                    pawnMoves.Add(newMove);
                }
                //Move ahead two spaces from start
                int rank = (origin & 0x70)/16;
                if ((rank == 6 && (piece & 0x08) == _cPlayer) || (rank == 1 && (piece & 0x00) == _cPlayer)) //ERROR PLAYER
                {
                    target = (byte)(2 * direction + origin/*(0x70 & origin) + (0x07 & origin)*/);
                    if ((target & 0x88) == 0 && _moveBoard.Tiles[target] == 0)
                    {
                        newMove = new Move(origin, piece);
                        newMove.target = target;
                        pawnMoves.Add(newMove);
                    }
                }
            }
            //Kill piece right
            target = (byte) (((0x70 & origin) + direction) + ((0x07 & origin) + 1));
            if ((target & 0x88) == 0)
            {
                newMove = new Move(origin, piece);
                newMove.target = target;
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
                newMove.target = target;
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
            //Console.WriteLine("from " + origin + " piece: " + _moveBoard.Tiles[origin] + " has " + pawnMoves.Count);
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
            Move newMove = new Move(origin, _moveBoard.Tiles[origin]);

            if (_moveBoard.Tiles[target] == 0)
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
            byte piece = _moveBoard.Tiles[move.target];
            if (piece == 0 || (piece & 0x88) == _cPlayer) return false;
            move.kill = piece;
            //Console.WriteLine("can take " + move.Killing.Piece + " at " + move.Killing.Position[0] + "," + move.Killing.Position[1]);
            //move.Killing.Position = move.Moving.Target;
            //move.Killing.Piece = move.Moving.Piece;
            return true;
        }
    }
}
