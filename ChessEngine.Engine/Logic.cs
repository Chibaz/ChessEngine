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
        private int _depth, _perft;
        private Stack<IMove> _principalVar;
        private readonly TimeSpan _timeAllowed = new TimeSpan(0, 0, 10);
        private readonly Stopwatch _time = new Stopwatch();

        public static byte Player;
        public Boolean Thinking;

        public Logic()
        {
            _mg = new MoveGenerator();
            _principalVar = new Stack<IMove>();
        }

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
                _time.Start();
                _depth = _perft = 0;
                Thinking = true;
                _principalVar = new Stack<IMove>();
                while (Thinking)
                {
                    _depth++;
                    _perft = 0;
                    Stack<IMove> principalStack;
                    DoAlphaBeta(Board.Game, _depth, Int32.MinValue, Int32.MaxValue, Player, out principalStack);
                    _principalVar = principalStack;
                    Console.WriteLine(_time.Elapsed + ": " + _depth + " " + _perft);
                }
                _time.Stop();
                Console.WriteLine("finished after " + _time.Elapsed + " : " + (_depth-1));
                _time.Reset();

                return _bestRootMove;
            }
            return null;
        }

        public int DoAlphaBeta(Board lastBoard, int rDepth, int alpha, int beta, byte rPlayer, out Stack<IMove> prioMove)
        {
            prioMove = null;
            if (_time.Elapsed > _timeAllowed || Thinking == false)
            {
                Thinking = false;
                return 0;
            }
            if (/*newMoves.Count == 0 ||*/ rDepth == 0)
            {
                int e = Evaluation.Evaluate(lastBoard);
                _perft++;
                return e;
            }
            List<IMove> newMoves = new List<IMove>();
            List<IMove> captureSacrifice = new List<IMove>();;
            if (_principalVar.Any())
            {
                newMoves.Add(_principalVar.Pop());
            }
            if (lastBoard.LastMovedPiece != 0xFF)
            {
                captureSacrifice = _mg.SacrificeCheck(lastBoard);
            }
            newMoves.AddRange(captureSacrifice.Count == 0
                ? _mg.GetAllMovesForPlayer(lastBoard, rPlayer)
                : captureSacrifice);
            if (rPlayer == Player) //Maximizing
            {
                //Get all possible moves from current state
                foreach (IMove move in newMoves)
                {
//                    move.Execute();
                    Board newBoard = lastBoard.CloneBoard();
                    move.ExecuteOnBoard(newBoard);

                    Stack<IMove> nextMove;                   
                    int v = DoAlphaBeta(newBoard, rDepth - 1, alpha, beta, GetEnemy(), out nextMove); //Recursive call on possible methods
                    Stack<IMove> moveChain = nextMove != null ? new Stack<IMove>(new Stack<IMove>(nextMove)) : new Stack<IMove>();
                    moveChain.Push(move);
                    if (v > alpha)
                    {
                        alpha = v;
                        prioMove = moveChain;
                        if (rDepth == _depth && Thinking)
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
                    int v = DoAlphaBeta(newBoard, rDepth - 1, alpha, beta, Player, out nextMove); //Recursive call on possible method
                    Stack<IMove> moveChain = nextMove != null ? new Stack<IMove>(new Stack<IMove>(nextMove)) : new Stack<IMove>(); moveChain.Push(move);
                    if (v < beta)
                    {
                        beta = v;
                        prioMove = moveChain;
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


