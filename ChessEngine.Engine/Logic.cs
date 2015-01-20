using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ChessEngine.Engine
{

    public class Logic
    {
        private IMove _bestRootMove;
        private readonly MoveGenerator _mg;
        //private int _score, _evals, _total;
        private int _depth, _perft;
        public string Turn;
        public Boolean EndGame;
        public static byte Player;
        public static int LastDepth;
        public byte LastMovedPiece;
        private IMove[] _principalVariations, _lastVariations;
        private Stack<IMove> _principalVar; 
        private readonly TimeSpan _timeAllowed = new TimeSpan(0,0,15);
        private readonly Stopwatch _time = new Stopwatch();
        private Boolean _running;

        public Logic()
        {
            _mg = new MoveGenerator();
            _lastVariations = new IMove[]{null};
            _principalVar = new Stack<IMove>();
            LastMovedPiece = 0xFF;
        }

//        public void SetDepth(int depth)
//        {
//            _depth = depth;
//        }
//
//        public int GetDepth() { return _depth; }

        public int RunPerft(int depth)
        {
            _perft = 0;
            RunPerftTest(Board.Game, depth, Board.Game.WhosTurn);
            return _perft;
        }

        public IMove GetBestMove()
        {
            if (Player != 0xFF)
            {
                //Console.WriteLine("performing best move");
                //_evals = _score = _total = 0;
                _time.Start();
                Stack<IMove> bestMove = null;
                _depth = _perft = 0;
                _running = true;
                while (_running)
                {
                //Console.WriteLine(DoAlphaBeta(Board.Game, _depth, Int32.MinValue, Int32.MaxValue, Player, /*0,*/ out bestMove));
                    _depth++;
                    _perft = 0;
                    _principalVariations = new IMove[_depth+1];
                    _principalVariations[_depth] = null;
                    DoAlphaBeta(Board.Game, _depth, Int32.MinValue, Int32.MaxValue, Player, /*0,*/ out bestMove);
                    _lastVariations = _principalVariations;
                    _principalVar = bestMove;
                    Console.WriteLine(_time.Elapsed + ": " + _depth + " " + _perft);
                }
                _time.Stop();
                Console.WriteLine("finished after " + _time.Elapsed + " : " + _depth + " moves: " + _perft);
                LastDepth = _depth;
                _time.Reset();

                return _bestRootMove;
            }
            return null;
        }

        public int DoAlphaBeta(Board lastBoard, int rDepth, int alpha, int beta, byte rPlayer, /*int bonus,*/ out Stack<IMove> prioMove)
        {
            //int newBonus;
            //Console.WriteLine("number of moves from last board: " + newMoves.Count + " at depth " + rDepth + " for player + " + rPlayer);
            prioMove = new Stack<IMove>();
            if (_time.Elapsed > _timeAllowed)
            {
                _running = false;
                return 0;
            }
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
                //_evals++;
                //Console.WriteLine("eval " + evals);
                return e /*+ bonus*/;
            }
            List<IMove> newMoves = new List<IMove>();
            if (LastMovedPiece != 0xFF)
            {
                _mg.SacrificeCheck(LastMovedPiece);
            }
            if (_principalVar.Count > 0)
            {
                newMoves.Add(_principalVar.Pop());
            }
            //if (_lastVariations[_lastVariations.Count()-rDepth] != null)
            //{
            //    newMoves.Add(_lastVariations[_lastVariations.Count() - rDepth]);
            //    _lastVariations[_lastVariations.Count() - rDepth] = null;
            //    _perft -= 1;
            //}
            newMoves.AddRange(_mg.GetAllMovesForPlayer(lastBoard, rPlayer));
            _perft += newMoves.Count;
            //Console.WriteLine(newMoves.Count);
            //_total += newMoves.Count;
            if (rPlayer == Player) //Maximizing
            {
                //Get all possible moves from current state
                foreach (IMove move in newMoves)
                {
//                    move.Execute();
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
                        Stack<IMove> moveChain = prioMove;
                        moveChain.Push(move);
                        prioMove = moveChain;
                        //_principalVariations[rDepth - 1] = move;
                        if (rDepth == _depth && _running)
                        {
                            Console.WriteLine("new best move " + v);
                            _bestRootMove = move;
                        }
                    }
                    if (alpha >= beta) //Stop if alpha is equal to or higher than beta, and prune the remainder
                    {
                        //move.Undo();
                        break;
                    }
                    //move.Undo();
                }
                return alpha;
            }
            else //Minimizing
            {
                foreach (IMove move in newMoves)
                {
//                    move.Execute();
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
                        Stack<IMove> moveChain = prioMove;
                        moveChain.Push(move);
                        prioMove = moveChain;
                        //_principalVariations[rDepth - 1] = move;
                    }
                    if (alpha >= beta) //Stop if alpha is equal to or higher than beta, and prune the remainder
                    {
                        //move.Undo();
                        break;
                    }
                    //move.Undo();
                }
                return beta;
            }
        }

        public void RunPerftTest(Board lastBoard, int depth, byte player)
        {
            if (depth == 0) return;
            List<IMove> newMoves = new List<IMove>();
            newMoves.AddRange(_mg.GetAllMovesForPlayer(lastBoard, player));
            _perft += newMoves.Count;
            byte newPlayer = player == WhitePlayer ? BlackPlayer : WhitePlayer;
            foreach (IMove move in newMoves)
            {
                Board newBoard = lastBoard.CloneBoard();
                move.ExecuteOnBoard(newBoard);

                RunPerftTest(newBoard, depth - 1, newPlayer); //Recursive call on possible methods
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


