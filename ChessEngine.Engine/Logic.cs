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
        //private IMove[] _principalVariations, _lastVariations;
        private Stack<IMove> _principalVar; 
        private readonly TimeSpan _timeAllowed = new TimeSpan(0,0,15);
        private readonly Stopwatch _time = new Stopwatch();
        private Boolean _running;

        public Logic()
        {
            _mg = new MoveGenerator();
            _principalVar = new Stack<IMove>();
//            _lastVariations = new IMove[]{null};
//            List<int> test2;
//            TestStack(out test2);
//            foreach (int t in test2)
//            {
//                Console.WriteLine(t);
//            }
//            Stack<int> test;
//            TestStack(Int32.MinValue, 2, out test);
//            while (test.Any())
//            {
//                Console.WriteLine(test.Pop());
//            }
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
                _depth = _perft = 0;
                _running = true;
                _principalVar = new Stack<IMove>();
                while (_running)
                {
                //Console.WriteLine(DoAlphaBeta(Board.Game, _depth, Int32.MinValue, Int32.MaxValue, Player, /*0,*/ out bestMove));
                    _depth++;
                    _perft = 0;
//                    _principalVariations = new IMove[_lastVariations.Count()+1];
//                    IMove principalMove;
                    Stack<IMove> principalStack;
                    DoAlphaBeta(Board.Game, _depth, Int32.MinValue, Int32.MaxValue, Player, 0, out principalStack);
//                    _lastVariations = _principalVariations;
                    _principalVar = principalStack;
//                    while (_principalVar.Any())
//                    {
//                        Move next = (Move)_principalVar.Pop();
//                        Console.WriteLine(next.Origin + " " + next.Target);
//                    }
                    Console.WriteLine(_time.Elapsed + ": " + _depth + " " + _perft);
                }
                _time.Stop();
                Console.WriteLine("finished after " + _time.Elapsed + " : " + (_depth-1));
                LastDepth = _depth;
                _time.Reset();

                return _bestRootMove;
            }
            return null;
        }

        public int DoAlphaBeta(Board lastBoard, int rDepth, int alpha, int beta, byte rPlayer, int bonus, out Stack<IMove> prioMove)
        {
            //Console.WriteLine("number of moves from last board: " + newMoves.Count + " at depth " + rDepth + " for player + " + rPlayer);
            int newBonus = 0;
            prioMove = null;
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
                return e + bonus;
            }
            List<IMove> newMoves = new List<IMove>();
            if (lastBoard.LastMovedPiece != 0xFF)
            {
                newMoves.AddRange(_mg.SacrificeCheck(lastBoard));
            }
            if (newMoves.Count == 0)
            {
                if(_principalVar.Any())
                {
                newMoves.Add(_principalVar.Pop());
                }
                newMoves.AddRange(_mg.GetAllMovesForPlayer(lastBoard, rPlayer));
            } 
//            if (_lastVariations[_lastVariations.Count()-rDepth] != null)
//            {
//                newMoves.Add(_lastVariations[_lastVariations.Count() - rDepth]);
//                _lastVariations[_lastVariations.Count() - rDepth] = null;
//                _perft -= 1;
//            }
            if (_depth >= 5 && rDepth == 1)
            {
                //Console.WriteLine("depth test");
            }
            _perft += newMoves.Count;
            //Console.WriteLine(newMoves.Count);
            //_total += newMoves.Count;
            if (rPlayer == Player) //Maximizing
            {
                //Get all possible moves from current state
                foreach (IMove move in newMoves)
                {
//                    move.Execute();
                    if (move is Move && rDepth == 1 && _depth == 5)
                    {
                        Move testMove = (Move)move;
                        if (testMove.Kill != 0)
                        {
                            //Console.WriteLine("check eval");
                        }
                    }
                    Board newBoard = lastBoard.CloneBoard();
                    move.ExecuteOnBoard(newBoard);
                    Stack<IMove> nextMove;
//                    newBonus = bonus + Evaluation.EvaluateBonus(move, rDepth);
                    
                    int v = DoAlphaBeta(newBoard, rDepth - 1, alpha, beta, GetEnemy(), newBonus, out nextMove); //Recursive call on possible methods
                    Stack<IMove> moveChain = nextMove != null ? new Stack<IMove>(new Stack<IMove>(nextMove)) : new Stack<IMove>();
                    moveChain.Push(move);
                    if (v > alpha)
                    {
                        alpha = v;
                        prioMove = moveChain;
//                        _lastVariations[_depth-1] = prioMove;
//                        if (rDepth > 3)
//                        {
//                            Console.WriteLine("test");
//                        }
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
                    Stack<IMove> nextMove;
//                    newBonus = bonus - Evaluation.EvaluateBonus(move, rDepth);
                    int v = DoAlphaBeta(newBoard, rDepth - 1, alpha, beta, Player, newBonus, out nextMove); //Recursive call on possible method
                    Stack<IMove> moveChain = nextMove != null ? new Stack<IMove>(new Stack<IMove>(nextMove)) : new Stack<IMove>(); moveChain.Push(move);
                    if (v < beta)
                    {
                        beta = v;
                        prioMove = moveChain;
//                        _lastVariations[_depth-1] = prioMove;
//                        if (rDepth > 3)
//                        {
//                            Console.WriteLine("test");
//                        }
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

        private int test;

        public void TestStack(/*int highest, int depth,*/ out List<int> stack)
        {
            stack = null;
            if (test < 5) return;
            
            TestStack(out stack);
            stack.Add(test++);
//            stack = null;
//            if (depth == 0)
//            {
//                return 99;
//            }
//            int[] root = {1, 2, 3, 4};
//            int[] evals = {6, 7, 4, 5, 8, 3, 2, 1};
//            int[] moves = depth == 1 ? root : evals;
//            foreach (int move in moves)
//            {
//                int v = TestStack(move, depth - 1, out stack);
//                if (v > highest)
//                {
//                    Stack<int> chain = stack;
//                    chain.Push(move);
//                    stack = chain;
//                }
//                return move;
//            }
//            return 0;
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


