using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    class Program
    {
        private static readonly BackgroundWorker _bwReadInput = new BackgroundWorker();
        private static readonly Winboard _winboard = new Winboard();

        static void Main(string[] args)
        {
        }

        public static bool ProcessInput(string input)
        {
            string[] split = input.Split(' ');
            string primaryCommand = split[0].ToLowerInvariant();
            switch (primaryCommand)
            {
                case "quit":
                    return false;
                case "logtest":
                    logtest(split[1]);
                    break;
                case "perft":
                    Perft.PerftSuite(int.Parse(split[1]), false, false);
                    break;
                case "evalperft":
                    Perft.PerftSuite(int.Parse(split[1]), true, false);
                    break;
                case "evalsortperft":
                    Perft.PerftSuite(int.Parse(split[1]), true, true);
                    break;
                case "sortperft":
                    Perft.PerftSuite(int.Parse(split[1]), false, true);
                    break;
                case "nodestodepth":
                    Perft.NodesToDepth(int.Parse(split[1]));
                    PrintSearchCutoffStats();
                    ConsoleWriteline(string.Format(" nodes:{0,10}\n evals:{1,10}\n pawns:{2,10}\n mater:{3,10}",
                        NoraGrace.Engine.Search.CountTotalAINodes,
                        NoraGrace.Engine.Evaluation.Evaluator.TotalEvalCount,
                        NoraGrace.Engine.Evaluation.PawnEvaluator.TotalEvalPawnCount,
                        NoraGrace.Engine.Evaluation.MaterialEvaluator.TotalEvalMaterialCount));
                    break;
                case "annotateeval":
                    Perft.AnnotatePGNEval(split[1], split[2]);
                    break;
                case "counts":
                    ConsoleWriteline(string.Format(" nodes:{0,10}\n evals:{1,10}\n pawns:{2,10}\n mater:{3,10}",
                        NoraGrace.Engine.Search.CountTotalAINodes,
                        NoraGrace.Engine.Evaluation.Evaluator.TotalEvalCount,
                        NoraGrace.Engine.Evaluation.PawnEvaluator.TotalEvalPawnCount,
                        NoraGrace.Engine.Evaluation.MaterialEvaluator.TotalEvalMaterialCount));
                    break;
                case "genmagic":
                    NoraGrace.Engine.Attacks.Generation.FindMagics();
                    ConsoleWriteline("done");
                    break;
                case "sts":
                    using (var reader = new System.IO.StreamReader("STSAll.epd"))
                    {
                        int totalCorrect = 0;
                        int totalScore = 0;
                        TimeSpan totalTime = TimeSpan.FromSeconds(0);

                        int possibleCorrect = 0;
                        int possibleScore = 0;
                        TimeSpan possibleTime = TimeSpan.FromSeconds(0);



                        var epds = NoraGrace.Engine.EPD.ParseMultiple(reader).ToArray();
                        NoraGrace.Engine.TranspositionTable transTable = new Engine.TranspositionTable();
                        foreach (var epd in epds.Take(1500))
                        {
                            possibleCorrect++;
                            possibleScore += 10;
                            possibleTime += TimeSpan.FromSeconds(1);

                            bool correct;
                            int score;
                            TimeSpan time;
                            correct = epd.RunTest(TimeSpan.FromSeconds(1), transTable, out score, out time);
                            ConsoleWriteline(string.Format("{4}/{5} {0} {1} {2} {3}", epd.ID, correct, score, time, possibleCorrect, epds.Length));

                            totalCorrect += correct ? 1 : 0;
                            totalScore += score;
                            totalTime += time;


                        }
                        ConsoleWriteline(string.Format("TotalCorrect:{0}/{1}", totalCorrect, possibleCorrect));
                        ConsoleWriteline(string.Format("TotalScore:{0}/{1}", totalScore, possibleScore));
                        ConsoleWriteline(string.Format("TotalTime:{0}/{1}", totalTime, possibleTime));
                    }
                    break;
                default:
                    _winboard.ProcessCmd(input);
                    break;
            }
            return true;
        }
    }
}
