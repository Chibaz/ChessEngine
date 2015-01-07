using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ChessEngine.Engine
{

    public class Logic
    {
        private IMove _next;
        private int score, _evals, _total;
        private readonly MoveGenerator _mg;
        private int Depth = 3;
        public Boolean EndGame;

        public Logic()
        {
            _mg = new MoveGenerator();
        }

        public void setDepth(int Depth)
        {
            this.Depth = Depth;
        }

        public int getDepth() { return Depth; }

        public IMove GetBestMove()
        {
            Console.WriteLine("performing best move");
            _evals = score = _total = 0;
            var time = new Stopwatch();
            time.Start();
            Move bestMove = null;
            /*while (time.Elapsed < timeAllowed)
            {*/
            DoAlphaBeta(Board.Game, Depth, Int32.MinValue, Int32.MaxValue, Board.aiColor, bestMove, 0);
            /*depth++;
        }*/
            time.Stop();
            Console.WriteLine(time.Elapsed + ": " + _evals + " evaluations after " + _total + " boards");
            return _next;
        }

        public int DoAlphaBeta(Board lastBoard, int rDepth, int alpha, int beta, int rPlayer, Move prioMove, int bonus)
        {
            List<IMove> newMoves = _mg.GetAllMovesForPlayer(lastBoard, rPlayer);
            _total += newMoves.Count;
            int newBonus;
            //Console.WriteLine("number of moves from last board: " + newMoves.Count + " at depth " + rDepth + " for player + " + rPlayer);
            if (newMoves.Count == 0 || rDepth == 0)
            {
                var e = Evaluation.Evaluate(lastBoard);
                //Console.WriteLine("eval: " + e + " bonus is " + bonus);
                //e += Evaluation.evaluateBonus(new Move(new int[] { 0, 0 }, 0), rDepth);//, rDepth
                /*if (bonus != 0)
                {
                    Console.WriteLine("added " + bonus + " to evaluation");
                }
                Console.WriteLine("total is " + (e + bonus));*/
                _evals++;
                //Console.WriteLine("eval " + evals);
                return e + bonus;
            }
            if (rPlayer == Board.aiColor) //Maximizing
            {
                //Get all possible moves from current state
                foreach (var move in newMoves)
                {
                    var newBoard = lastBoard.CloneBoard();
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
                    newBonus = bonus + Evaluation.EvaluateBonus(move, rDepth);
                    var v = DoAlphaBeta(newBoard, rDepth - 1, alpha, beta, rPlayer * -1, null, newBonus); //Recursive call on possible methods
                    if (v > alpha)
                    {
                        alpha = v;
                        if (rDepth == Depth)
                        {
                            //Console.WriteLine("new best move " + v);
                            _next = move;
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
                foreach (var move in newMoves)
                {
                    var newBoard = lastBoard.CloneBoard();
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
                    newBonus = bonus - Evaluation.EvaluateBonus(move, rDepth);
                    var v = DoAlphaBeta(newBoard, rDepth - 1, alpha, beta, rPlayer * -1, null, newBonus); //Recursive call on possible method                    
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
    }
}


