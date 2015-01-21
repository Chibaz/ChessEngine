using System;

namespace ChessEngine.Engine
{
    internal class Evaluation
    {
        private static byte[] _eBoard;
        public static Boolean _endgame;
        private static int _losing = 0xFF;

        public static int Evaluate(Board toEvaluate
            /*, int depth, int lastAI, int lastPlayer, out int wPieces, out int bPieces*/)
        {
            _eBoard = toEvaluate.Tiles;
            int logicPieces = CalculatePieceScore(Logic.Player);
            //Console.WriteLine("logic piece score is " + logicPieces + " endgame? " + (logicPieces < 600));
            int enemyPieces = CalculatePieceScore(Logic.GetEnemy());
            //Console.WriteLine("enemy piece score is " + enemyPieces + " endgame? " + (enemyPieces < 600));
            if (logicPieces <= 600 || enemyPieces <= 600)
            {
                _endgame = true;
                if (logicPieces < enemyPieces)
                {
                    _losing = Logic.Player;
                    //Console.WriteLine("ai is currently losing");
                }
                else if (logicPieces > enemyPieces)
                {
                    _losing = Logic.GetEnemy();
                    //Console.WriteLine("player is currently losing");
                }
                else
                {
                    _losing = 0;
                }
            }
            else if (!_endgame && (logicPieces <= 1600 || enemyPieces <= 1600))
            {
                _endgame = true;
                //Console.WriteLine("endgame enabled");
                _losing = 0;
            }
            int logicScore = EvaluateSide(Logic.Player);
            int enemyScore = EvaluateSide(Logic.GetEnemy());
            logicScore += logicPieces;
            enemyScore += enemyPieces;
            int total = logicScore - enemyScore;
            //Console.WriteLine(logicPieces + " " + enemyPieces + " " + total);
            return total;
        }

        public static int EvaluateSide(byte player)
        {
            ScoreTable sTable;
            if (player == Logic.WhitePlayer)
            {
                sTable = new ScoreWhite();
            }
            else
            {
                sTable = new ScoreBlack();
            }
            //int pieces = 0;
            int score = 0;
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    byte thisPiece = _eBoard[16*rank + file];
                    //pieces += ScoreTable.PieceValue(thisPiece);
                    if ((player != 0x00 || thisPiece > 0x08) && (player != 0x08 || thisPiece < 0x08)) continue;
                    switch (thisPiece & 0x07)
                    {
                        case 0x01:
                            score += sTable.Pawn[16*rank + file];
                            break;
                        case 0x02:
                            if (_losing != player)
                            {
                                score += sTable.Knight[16*rank + file];
                            }
                            break;
                        case 0x03:
                            int kingScore = sTable.King[16*rank + file];
                            if (_losing == player)
                            {
                                kingScore *= 8;
                            }
                            score += kingScore;
                            score += 10000;
                            break;
                        case 0x05:
                            if (_losing != player)
                            {
                                score += sTable.Bishop[16*rank + file];
                            }
                            break;
                        case 0x06:
                            if (_losing != player)
                            {
                                score += sTable.Rook[16*rank + file];
                            }
                            break;
                        case 0x07:
                            if (_losing != player)
                            {
                                score += sTable.Queen[16*rank + file];
                            }
                            break;
                    }
                }
            }
            return score;
        }

        public static int CalculatePieceScore(byte player)
        {
            int pieces = 0;
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    byte piece = _eBoard[16*rank + file];

//                    Console.WriteLine(((player == 0) && (piece > 8)));
//                    Console.WriteLine(((player == 8) && (piece < 8)));
                    if(((player == 0) && (piece > 8)) ||((player == 8) && (piece < 8))) continue;
                    switch (piece & 0x07)
                    {
                        case 0x01:
                            pieces += 100;
                            break;
                        case 0x02:
                            pieces += 300;
                            break;
                        case 0x05:
                            pieces += 325;
                            break;
                        case 0x06:
                            pieces += 500;
                            break;
                        case 0x07:
                            pieces += 900;
                            break;
                    }
                }
            }
            return pieces;
        }

        public static int EvaluateBonus(IMove move, int depth)
        {
            if (!(move is Move) && !(move is EnPassant)) return 0;
            if (move is EnPassant)
            {
                return ScoreTable.PieceValue(0x01) / 10 * depth;
            }
            Move m = (Move)move;
            if (m.Kill != 0)
            {
                /*
                    Console.WriteLine("at depth " + depth + " applied bonus/penalty");
                    Console.WriteLine("for " + m.Moving.Piece + " taking " + m.Killing.Piece + "\n");
                     * */
                return ScoreTable.PieceValue((m.Kill & 0x07)) / 10 * depth;
            }
            return 0;
        }
    }

    internal abstract class ScoreTable
    {
        public abstract int[] Pawn { get; }
        public abstract int[] Rook { get; }
        public abstract int[] Knight { get; }
        public abstract int[] Bishop { get; }
        public abstract int[] Queen { get; }
        public abstract int[] King { get; }

        public static int PieceValue(int piece)
        {
            var p = Math.Abs(piece);
            if (p == 1)
            {
                return 100;
            }
            if (p == 2)
            {
                return 500;
            }
            if (p == 3)
            {
                return 300;
            }
            if (p == 4)
            {
                return 325;
            }
            if (p == 5)
            {
                return 900;
            }
            if (p == 6)
            {
                return 10000;
            }
            return 0;
        }
    }

    internal class ScoreBlack : ScoreTable
    {
        private readonly int[] _pawn =
        {
            0, 0, 0, 0, 0, 0, 0, 0,         999, 0, 0, 0, 0, 0, 0, 0,
            7, 7, 13, 23, 26, 13, 7, 7,     999, 0, 0, 0, 0, 0, 0, 0,
            -2, -2, 4, 12, 15, 4, -2, -2,   999, 0, 0, 0, 0, 0, 0, 0,
            -3, -3, 2, 9, 11, 2, -3, -3,    999, 0, 0, 0, 0, 0, 0, 0,
            -4, -4, 0, 6, 8, 0, -4, -4,     999, 0, 0, 0, 0, 0, 0, 0,
            -4, -4, 0, 4, 6, 0, -4, -4,     999, 0, 0, 0, 0, 0, 0, 0,
            -1, -1, 1, 5, 6, 1, -1, -1,     999, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,         999, 0, 0, 0, 0, 0, 0, 0
        };

//        private readonly int[,] _pawn =
//        {
//            {0, 0, 0, 0, 0, 0, 0, 0},
//            {7, 7, 13, 23, 26, 13, 7, 7},
//            {-2, -2, 4, 12, 15, 4, -2, -2},
//            {2, -3, 2, 9, 11, 2, -3, -3},
//            {1, 2, 0, 6, 9, 0, -4, -4},
//            {-4, -4, 2, 4, 7, 0, -4, -4},
//            {-1, -1, 1, 5, 6, 1, -1, -1},
//            {0, 0, 0, 0, 0, 0, 0, 0}
//        };

        private readonly int[] _rook =
        {
            9, 9, 11, 10, 11, 9, 9, 9,      999, 0, 0, 0, 0, 0, 0, 0,
            4, 6, 7, 9, 9, 7, 6, 4,         999, 0, 0, 0, 0, 0, 0, 0,
            9, 10, 10, 11, 11, 10, 10, 9,   999, 0, 0, 0, 0, 0, 0, 0,
            8, 8, 8, 9, 9, 8, 8, 8,         999, 0, 0, 0, 0, 0, 0, 0,
            6, 6, 5, 6, 6, 5, 6, 6,         999, 0, 0, 0, 0, 0, 0, 0,
            4, 5, 5, 5, 5, 5, 5, 4,         999, 0, 0, 0, 0, 0, 0, 0,
            0, 3, 3, 5, 5, 3, 3, 0,         999, 0, 0, 0, 0, 0, 0, 0,
            0, -2, 0, 5, 5, 3, 0, 0,        999, 0, 0, 0, 0, 0, 0, 0
        };

//        private readonly int[,] _rook =
//        {
//            {9, 9, 11, 10, 11, 9, 9, 9},
//            {4, 6, 7, 9, 9, 7, 6, 4},
//            {9, 10, 10, 11, 11, 10, 10, 9},
//            {8, 8, 8, 9, 9, 8, 8, 8},
//            {6, 6, 5, 6, 6, 5, 6, 6},
//            {4, 5, 5, 5, 5, 5, 5, 4},
//            {0, 3, 3, 5, 5, 3, 3, 0},
//            {0, -2, 0, 5, 5, 3, 0, 0}
//        };

        private readonly int[] _knight =
        {
            -2, 2, 7, 9, 9, 7, 2, -2,       999, 0, 0, 0, 0, 0, 0, 0,
            1, 4, 12, 13, 13, 12, 4, 1,     999, 0, 0, 0, 0, 0, 0, 0,
            5, 11, 18, 19, 19, 18, 11, 5,   999, 0, 0, 0, 0, 0, 0, 0,
            3, 10, 14, 14, 14, 14, 10, 3,   999, 0, 0, 0, 0, 0, 0, 0,
            0, 5, 8, 9, 9, 8, 5, 0,         999, 0, 0, 0, 0, 0, 0, 0,
            -3, 1, 3, 4, 4, 3, 1, -3,       999, 0, 0, 0, 0, 0, 0, 0,
            -5, -3, -1, 0, 0, -1, -3, -5,   999, 0, 0, 0, 0, 0, 0, 0,
            -7, -5, -4, -2, -2, -4, -5, -7, 999, 0, 0, 0, 0, 0, 0, 0
        };

//        private readonly int[,] _knight =
//        {
//            {-2, 2, 7, 9, 9, 7, 2, -2},
//            {1, 4, 12, 13, 13, 12, 4, 1},
//            {5, 11, 18, 19, 19, 18, 11, 5},
//            {3, 10, 14, 14, 14, 14, 10, 3},
//            {0, 5, 8, 9, 9, 8, 5, 0},
//            {-3, 1, 3, 4, 4, 3, 1, -3},
//            {-5, -3, -1, 0, 0, -1, -3, -5},
//            {-7, -5, -4, -2, -2, -4, -5, -7}
//        };

        private readonly int[] _bishop =
        {
            2, 3, 4, 4, 4, 4, 3, 2,     999, 0, 0, 0, 0, 0, 0, 0,
            4, 7, 7, 7, 7, 7, 7, 4,     999, 0, 0, 0, 0, 0, 0, 0,
            3, 5, 6, 6, 6, 6, 5, 3,     999, 0, 0, 0, 0, 0, 0, 0,
            3, 5, 7, 7, 7, 7, 5, 3,     999, 0, 0, 0, 0, 0, 0, 0,
            4, 5, 6, 8, 8, 6, 5, 4,     999, 0, 0, 0, 0, 0, 0, 0,
            4, 5, 5, -2, -2, 5, 5, 4,   999, 0, 0, 0, 0, 0, 0, 0,
            5, 5, 5, 3, 3, 5, 5, 5,     999, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,     999, 0, 0, 0, 0, 0, 0, 0
        };

//        private readonly int[,] _bishop =
//        {
//            {2, 3, 4, 4, 4, 4, 3, 2},
//            {4, 7, 7, 7, 7, 7, 7, 4},
//            {3, 5, 6, 6, 6, 6, 5, 3},
//            {3, 5, 7, 7, 7, 7, 5, 3},
//            {4, 5, 6, 8, 8, 6, 5, 4},
//            {4, 5, 5, -2, -2, 5, 5, 4},
//            {5, 5, 5, 3, 3, 5, 5, 5},
//            {0, 0, 0, 0, 0, 0, 0, 0}
//        };

        private readonly int[] _queen =
        {
            2, 3, 4, 3, 4, 3, 3, 2,     999, 0, 0, 0, 0, 0, 0, 0,
            2, 3, 4, 4, 4, 4, 3, 2,     999, 0, 0, 0, 0, 0, 0, 0,
            3, 4, 4, 4, 4, 4, 4, 3,     999, 0, 0, 0, 0, 0, 0, 0,
            3, 3, 4, 4, 4, 4, 3, 3,     999, 0, 0, 0, 0, 0, 0, 0,
            2, 3, 3, 4, 4, 3, 3, 2,     999, 0, 0, 0, 0, 0, 0, 0,
            2, 2, 2, 3, 3, 2, 2, 2,     999, 0, 0, 0, 0, 0, 0, 0,
            2, 2, 2, 2, 2, 2, 2, 2,     999, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,     999, 0, 0, 0, 0, 0, 0, 0
        };

//        private readonly int[,] _queen =
//        {
//            {2, 3, 4, 3, 4, 3, 3, 2},
//            {2, 3, 4, 4, 4, 4, 3, 2},
//            {3, 4, 4, 4, 4, 4, 4, 3},
//            {3, 3, 4, 4, 4, 4, 3, 3},
//            {2, 3, 3, 4, 4, 3, 3, 2},
//            {2, 2, 2, 3, 3, 2, 2, 2},
//            {2, 2, 2, 2, 2, 2, 2, 2},
//            {0, 0, 0, 0, 0, 0, 0, 0}
//        };

        private readonly int[] _king =
        {
            0, 0, 0, 0, 0, 0, 0, 0,     999, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,     999, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,     999, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,     999, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,     999, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,     999, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,     999, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 15, 0,    999, 0, 0, 0, 0, 0, 0, 0,
        };

//        private readonly int[,] _king =
//        {
//            {0, 0, 0, 0, 0, 0, 0, 0},
//            {0, 0, 0, 0, 0, 0, 0, 0},
//            {0, 0, 0, 0, 0, 0, 0, 0},
//            {0, 0, 0, 0, 0, 0, 0, 0},
//            {0, 0, 0, 0, 0, 0, 0, 0},
//            {0, 0, 0, 0, 0, 0, 0, 0},
//            {0, 0, 0, 0, 0, 0, 0, 0},
//            {0, 0, 0, 0, 0, 0, 15, 0}
//        };

        private readonly int[] _endKing =
        {
            -3, -3, -3, -3, -3, -3, -3, -3, 999, 0, 0, 0, 0, 0, 0, 0,
            -3, -2, -2, -2, -2, -2, -2, -3, 999, 0, 0, 0, 0, 0, 0, 0,
            -3, -2, -1, -1, -1, -1, -2, -3, 999, 0, 0, 0, 0, 0, 0, 0,
            -3, -2, -1, 0, 0, -1, -2, -3,   999, 0, 0, 0, 0, 0, 0, 0,
            -3, -2, -1, 0, 0, -1, -2, -3,   999, 0, 0, 0, 0, 0, 0, 0,
            -3, -2, -1, -1, -1, -1, -2, -3, 999, 0, 0, 0, 0, 0, 0, 0,
            -3, -2, -2, -2, -2, -2, -2, -3, 999, 0, 0, 0, 0, 0, 0, 0,
            -3, -3, -3, -3, -3, -3, -3, -3, 999, 0, 0, 0, 0, 0, 0, 0,
        };

//        private readonly int[,] _endKing =
//        {
//            {-3, -3, -3, -3, -3, -3, -3, -3},
//            {-3, -2, -2, -2, -2, -2, -2, -3},
//            {-3, -2, -1, -1, -1, -1, -2, -3},
//            {-3, -2, -1, 0, 0, -1, -2, -3},
//            {-3, -2, -1, 0, 0, -1, -2, -3},
//            {-3, -2, -1, -1, -1, -1, -2, -3},
//            {-3, -2, -2, -2, -2, -2, -2, -3},
//            {-3, -3, -3, -3, -3, -3, -3, -3}
//        };

        public override int[] Pawn
        {
            get { return _pawn; }
        }

        public override int[] Rook
        {
            get { return _rook; }
        }

        public override int[] Knight
        {
            get { return _knight; }
        }

        public override int[] Bishop
        {
            get { return _bishop; }
        }

        public override int[] Queen
        {
            get { return _queen; }
        }

        public override int[] King
        {
            get
            {
                return Evaluation._endgame ? _endKing : _king;
            }
        }
    }

    internal class ScoreWhite : ScoreTable
    {
        private readonly int[] _pawn =
        {
            0, 0, 0, 0, 0, 0, 0, 0,         999, 0, 0, 0, 0, 0, 0, 0,
            -1, -1, 1, 5, 6, 1, -1, -1,     999, 0, 0, 0, 0, 0, 0, 0,
            -4, -4, 0, 4, 6, 0, -4, -4,     999, 0, 0, 0, 0, 0, 0, 0,
            -4, -4, 0, 6, 8, 0, -4, -4,     999, 0, 0, 0, 0, 0, 0, 0,
            -3, -3, 2, 9, 11, 2, -3, -3,    999, 0, 0, 0, 0, 0, 0, 0,
            -2, -2, 4, 12, 15, 4, -2, -2,   999, 0, 0, 0, 0, 0, 0, 0,
            7, 7, 13, 23, 26, 13, 7, 7,     999, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,         999, 0, 0, 0, 0, 0, 0, 0
        };

        private readonly int[] _rook =
        {
            0, 0, 0, 0, 0, 0, 0, 0,         999, 0, 0, 0, 0, 0, 0, 0,
            3, 4, 4, 6, 6, 4, 4, 3,         999, 0, 0, 0, 0, 0, 0, 0,
            4, 5, 5, 5, 5, 5, 5, 4,         999, 0, 0, 0, 0, 0, 0, 0,
            6, 6, 5, 6, 6, 5, 6, 6,         999, 0, 0, 0, 0, 0, 0, 0,
            8, 8, 8, 9, 9, 8, 8, 8,         999, 0, 0, 0, 0, 0, 0, 0,
            9, 10, 10, 11, 11, 10, 10, 9,   999, 0, 0, 0, 0, 0, 0, 0,
            4, 6, 7, 9, 9, 7, 6, 4,         999, 0, 0, 0, 0, 0, 0, 0,
            9, 9, 11, 10, 11, 9, 9, 9,      999, 0, 0, 0, 0, 0, 0, 0,
        };

        private readonly int[] _knight =
        {
            -7, -5, -4, -2, -2, -4, -5, -7, 999, 0, 0, 0, 0, 0, 0, 0,
            -5, -3, -1, 0, 0, -1, -3, -5,   999, 0, 0, 0, 0, 0, 0, 0,
            -3, 1, 3, 4, 4, 3, 1, -3,       999, 0, 0, 0, 0, 0, 0, 0,
            0, 5, 8, 9, 9, 8, 5, 0,         999, 0, 0, 0, 0, 0, 0, 0,
            3, 10, 14, 14, 14, 14, 10, 3,   999, 0, 0, 0, 0, 0, 0, 0,
            5, 11, 18, 19, 19, 18, 11, 5,   999, 0, 0, 0, 0, 0, 0, 0,
            1, 4, 12, 13, 13, 12, 4, 1,     999, 0, 0, 0, 0, 0, 0, 0,
            -2, 2, 7, 9, 9, 7, 2, -2,       999, 0, 0, 0, 0, 0, 0, 0
        };

        private readonly int[] _bishop =
        {
            0, 0, 0, 0, 0, 0, 0, 0,     999, 0, 0, 0, 0, 0, 0, 0,
            5, 5, 5, 3, 3, 5, 5, 5,     999, 0, 0, 0, 0, 0, 0, 0,
            4, 5, 5, -2, -2, 5, 5, 4,   999, 0, 0, 0, 0, 0, 0, 0,
            4, 5, 6, 8, 8, 6, 5, 4,     999, 0, 0, 0, 0, 0, 0, 0,
            3, 5, 7, 7, 7, 7, 5, 3,     999, 0, 0, 0, 0, 0, 0, 0,
            3, 5, 6, 6, 6, 6, 5, 3,     999, 0, 0, 0, 0, 0, 0, 0,
            4, 7, 7, 7, 7, 7, 7, 4,     999, 0, 0, 0, 0, 0, 0, 0,
            2, 3, 4, 4, 4, 4, 3, 2,     999, 0, 0, 0, 0, 0, 0, 0
        };

        private readonly int[] _queen =
        {
            0, 0, 0, 0, 0, 0, 0, 0,     999, 0, 0, 0, 0, 0, 0, 0,
            2, 2, 2, 2, 2, 2, 2, 2,     999, 0, 0, 0, 0, 0, 0, 0,
            2, 2, 2, 3, 3, 2, 2, 2,     999, 0, 0, 0, 0, 0, 0, 0,
            2, 3, 3, 4, 4, 3, 3, 2,     999, 0, 0, 0, 0, 0, 0, 0,
            3, 3, 4, 4, 4, 4, 3, 3,     999, 0, 0, 0, 0, 0, 0, 0,
            3, 4, 4, 4, 4, 4, 4, 3,     999, 0, 0, 0, 0, 0, 0, 0,
            2, 3, 4, 4, 4, 4, 3, 2,     999, 0, 0, 0, 0, 0, 0, 0,
            2, 3, 4, 3, 4, 3, 3, 2,     999, 0, 0, 0, 0, 0, 0, 0,
        };

        private readonly int[] _king =
        {
            0, 0, 0, 0, 0, 0, 0, 0,     999, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,     999, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,     999, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,     999, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,     999, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,     999, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,     999, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,     999, 0, 0, 0, 0, 0, 0, 0,
        };

        private readonly int[] _endKing =
        {
            -3, -3, -3, -3, -3, -3, -3, -3, 999, 0, 0, 0, 0, 0, 0, 0,
            -3, -2, -2, -2, -2, -2, -2, -3, 999, 0, 0, 0, 0, 0, 0, 0,
            -3, -2, -1, -1, -1, -1, -2, -3, 999, 0, 0, 0, 0, 0, 0, 0,
            -3, -2, -1, 0, 0, -1, -2, -3,   999, 0, 0, 0, 0, 0, 0, 0,
            -3, -2, -1, 0, 0, -1, -2, -3,   999, 0, 0, 0, 0, 0, 0, 0,
            -3, -2, -1, -1, -1, -1, -2, -3, 999, 0, 0, 0, 0, 0, 0, 0,
            -3, -2, -2, -2, -2, -2, -2, -3, 999, 0, 0, 0, 0, 0, 0, 0,
            -3, -3, -3, -3, -3, -3, -3, -3, 999, 0, 0, 0, 0, 0, 0, 0,
        };

        public override int[] Pawn
        {
            get { return _pawn; }
        }

        public override int[] Rook
        {
            get { return _rook; }
        }

        public override int[] Knight
        {
            get { return _knight; }
        }

        public override int[] Bishop
        {
            get { return _bishop; }
        }

        public override int[] Queen
        {
            get { return _queen; }
        }

        public override int[] King
        {
            get
            {
                return Evaluation._endgame ? _endKing : _king;
            }
        }
    }
}