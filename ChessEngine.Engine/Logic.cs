using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ChessEngine.Engine
{

    public class Logic
    {
        private IMove _bestRootMove;
        private readonly MoveGenerator _mg;
        private int _score, _evals, _total;
        private int _depth = 2;
        public string Turn;
        public Boolean EndGame;
        public static byte Player;

        public Logic()
        {
            _mg = new MoveGenerator();
        }

        public void SetDepth(int depth)
        {
            _depth = depth;
        }

        public int getDepth() { return _depth; }

        public IMove GetBestMove()
        {
            if (Player != 0xFF)
            {
                Console.WriteLine("performing best move");
                _evals = _score = _total = 0;
                var time = new Stopwatch();
                time.Start();
                IMove bestMove = null;
                /*while (time.Elapsed < timeAllowed)
            {*/
                DoAlphaBeta(Board.Game, _depth, Int32.MinValue, Int32.MaxValue, Player, /*0,*/ out bestMove);
                /*depth++;
        }*/
                time.Stop();
                //Console.WriteLine(time.Elapsed + ": " + _evals + " evaluations after " + _total + " boards");

                return _bestRootMove;
            }
            return null;
        }

        public int DoAlphaBeta(Board lastBoard, int rDepth, int alpha, int beta, byte rPlayer, /*int bonus,*/ out IMove prioMove)
        {
            //int newBonus;
            //Console.WriteLine("number of moves from last board: " + newMoves.Count + " at depth " + rDepth + " for player + " + rPlayer);
            prioMove = null;
            if (/*newMoves.Count == 0 ||*/ rDepth == 0)
            {
                int e = Evaluation.Evaluate(lastBoard);
                //Console.WriteLine("eval: " + e + " bonus is " + bonus);
                //e += Evaluation.evaluateBonus(new Move(new int[] { 0, 0 }, 0), rDepth);//, rDepth
                /*if (bonus != 0)
                {
                    Console.WriteLine("added " + bonus + " to evaluation");
                }
                Console.WriteLine("total is " + (e + bonus));*/
                _evals++;
                //Console.WriteLine("eval " + evals);
                return e /*+ bonus*/;
            }
            List<IMove> newMoves = _mg.GetAllMovesForPlayer(lastBoard, rPlayer);
            _total += newMoves.Count;
            if (rPlayer == Player) //Maximizing
            {
                //Get all possible moves from current state
                foreach (IMove move in newMoves)
                {
                    Board newBoard = lastBoard.CloneBoard();
                    move.ExecuteOnBoard(newBoard);
                    /*
                    if (rDepth == 1)
                    {
                        Console.WriteLine("lastBoard was");
                        foreach (var entry in lastBoard.tiles)
                        {
                            Console.Write(entry + " ");
                        }
                        Console.WriteLine("newBoard is");
                        foreach (var entry in newBoard.tiles)
                        {
                            Console.Write(entry + " ");
                        }
                        Console.WriteLine();
                    }*/
                    //newBonus = bonus + Evaluation.EvaluateBonus(move, rDepth);
                    int v = DoAlphaBeta(newBoard, rDepth - 1, alpha, beta, GetEnemy(), /*newBonus,*/ out prioMove); //Recursive call on possible methods
                    if (v > alpha)
                    {
                        alpha = v;
                        if (rDepth == _depth)
                        {
                            //Console.WriteLine("new best move " + v);
                            prioMove = move;
                            _bestRootMove = move;
                        }
                    }
                    if (alpha >= beta) //Stop if alpha is equal to or higher than beta, and prune the remainder
                    {
                        break;
                    }
                }
                return alpha;
            }
            else //Minimizing
            {
                foreach (IMove move in newMoves)
                {
                    Board newBoard = lastBoard.CloneBoard();
                    move.ExecuteOnBoard(newBoard);
                    /*
                    if (rDepth == 1)
                    {
                        Console.WriteLine("lastBoard was");
                        foreach (var entry in lastBoard.tiles)
                        {
                            Console.Write(entry + " ");
                        }
                        Console.WriteLine("newBoard is");
                        foreach (var entry in newBoard.tiles)
                        {
                            Console.Write(entry + " ");
                        }
                        Console.WriteLine();
                    }
                    if (move is Move && ((Move)move).Killing.Position != null)
                    {
                        newBonus -= 100 * rDepth;
                        Console.WriteLine("kill penalty");
                        //Console.WriteLine("bonus of " + newBonus + " applied");
                    }*/
                    /*if (Evaluation.evaluateBonus(move, rDepth) != 0)
                    {
                        Console.WriteLine("increased bonus from " + bonus + " to " + newBonus);
                    }*/
                    //newBonus = bonus - Evaluation.EvaluateBonus(move, rDepth);
                    int v = DoAlphaBeta(newBoard, rDepth - 1, alpha, beta, Player, /*newBonus,*/ out prioMove); //Recursive call on possible method                    
                    if (v < beta)
                    {
                        beta = v;
                    }
                    if (alpha >= beta) //Stop if alpha is equal to or higher than beta, and prune the remainder
                    {
                        break;
                    }
                }
                return beta;
            }
        }

        public static byte GetEnemy()
        {
            return Player.Equals(BlackPlayer) ? WhitePlayer : BlackPlayer;
        }

        public static byte WhitePlayer = 0x00;
        public static byte BlackPlayer = 0x08;
    }


}


