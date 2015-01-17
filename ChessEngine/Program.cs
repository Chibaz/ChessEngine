using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using ChessEngine.Engine;

namespace ChessEngine.CommandLine
{
    class Program
    {
        private static readonly BackgroundWorker ReadInput = new BackgroundWorker();
        private static readonly Winboard Winboard = new Winboard();
        private static Boolean _running = true;
        public static TextWriter Logger;

        static void Main(string[] args)
        {
            Console.WriteLine("ChessEngine v{0}.{1} by Kasper Wind, Denmark", typeof(Program).Assembly.GetName().Version.Major, typeof(Program).Assembly.GetName().Version.Minor);

            //Make logger
            string path = @"C:\Users\" + Environment.UserName + @"\Source\ChessEngine\LogFile.txt";
            Logger = new StreamWriter(path, false);
            Logger.WriteLine("ChessEngine v{0}.{1} by Kasper Wind, Denmark",
                   typeof (Program).Assembly.GetName().Version.Major, typeof (Program).Assembly.GetName().Version.Minor);
//            try
//            {
//                if (File.Exists(path))
//                {
//                    File.Delete(path);
//                }
//                File.Create(path);
//                Logger = new StreamWriter(path, true);
//                Logger.WriteLine("ChessEngine v{0}.{1} by Kasper Wind, Denmark",
//                    typeof (Program).Assembly.GetName().Version.Major, typeof (Program).Assembly.GetName().Version.Minor);
//            }
//            catch (IOException e)
//            {
//                Console.WriteLine(e.ToString());
//            }

            ReadInput.DoWork += _bwReadInput_DoWork;
            ReadInput.RunWorkerCompleted += _bwReadInput_CompletedWork;
            ReadInput.RunWorkerAsync();

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            while (_running)
            {
                System.Threading.Thread.Sleep(500);
            }
            Logger.WriteLine("Runtime finished");
        }

        private static void _bwReadInput_DoWork(object sender, DoWorkEventArgs e)
        {
            try{
                while(true){
                    string input = Console.ReadLine();
                    Boolean playing = ProcessInput(input);
                    Logger.WriteLine("input: " + input);
                    if(!playing){
                        break;
                    }
                }
            }catch(Exception ex){
                Console.WriteLine(ex.StackTrace);
                Logger.WriteLine(ex.StackTrace);
            }
        }

        private static void _bwReadInput_CompletedWork(object sender, RunWorkerCompletedEventArgs e)
        {
            _running = false;
        }

        public static bool ProcessInput(string input)
        {
            string[] split = input.Split(' ');
            string primaryCommand = split[0].ToLowerInvariant();

            switch (primaryCommand)
            {
                case "quit":
                    _running = false;
                    return false;

//                case "logtest":
//                    logtest(split[1]);
//                    break;
//                case "perft":
//                    Perft.PerftSuite(int.Parse(split[1]), false, false);
//                    break;
//                case "evalperft":
//                    Perft.PerftSuite(int.Parse(split[1]), true, false);
//                    break;
//                case "evalsortperft":
//                    Perft.PerftSuite(int.Parse(split[1]), true, true);
//                    break;
//                case "sortperft":
//                    Perft.PerftSuite(int.Parse(split[1]), false, true);
//                    break;
//                case "nodestodepth":
//                    Perft.NodesToDepth(int.Parse(split[1]));
//                    PrintSearchCutoffStats();
//                    ConsoleWriteline(string.Format(" nodes:{0,10}\n evals:{1,10}\n pawns:{2,10}\n mater:{3,10}",
//                        NoraGrace.Engine.Search.CountTotalAINodes,
//                        NoraGrace.Engine.Evaluation.Evaluator.TotalEvalCount,
//                        NoraGrace.Engine.Evaluation.PawnEvaluator.TotalEvalPawnCount,
//                        NoraGrace.Engine.Evaluation.MaterialEvaluator.TotalEvalMaterialCount));
//                    break;
//                case "annotateeval":
//                    Perft.AnnotatePGNEval(split[1], split[2]);
//                    break;
//                case "counts":
//                    ConsoleWriteline(string.Format(" nodes:{0,10}\n evals:{1,10}\n pawns:{2,10}\n mater:{3,10}",
//                        NoraGrace.Engine.Search.CountTotalAINodes,
//                        NoraGrace.Engine.Evaluation.Evaluator.TotalEvalCount,
//                        NoraGrace.Engine.Evaluation.PawnEvaluator.TotalEvalPawnCount,
//                        NoraGrace.Engine.Evaluation.MaterialEvaluator.TotalEvalMaterialCount));
//                    break;
//                case "genmagic":
//                    NoraGrace.Engine.Attacks.Generation.FindMagics();
//                    ConsoleWriteline("done");
//                    break;
//                case "sts":
//                    using (var reader = new System.IO.StreamReader("STSAll.epd"))
//                    {
//                        int totalCorrect = 0;
//                        int totalScore = 0;
//                        TimeSpan totalTime = TimeSpan.FromSeconds(0);
//
//                        int possibleCorrect = 0;
//                        int possibleScore = 0;
//                        TimeSpan possibleTime = TimeSpan.FromSeconds(0);
//
//
//
//                        var epds = NoraGrace.Engine.EPD.ParseMultiple(reader).ToArray();
//                        NoraGrace.Engine.TranspositionTable transTable = new Engine.TranspositionTable();
//                        foreach (var epd in epds.Take(1500))
//                        {
//                            possibleCorrect++;
//                            possibleScore += 10;
//                            possibleTime += TimeSpan.FromSeconds(1);
//
//                            bool correct;
//                            int score;
//                            TimeSpan time;
//                            correct = epd.RunTest(TimeSpan.FromSeconds(1), transTable, out score, out time);
//                            ConsoleWriteline(string.Format("{4}/{5} {0} {1} {2} {3}", epd.ID, correct, score, time, possibleCorrect, epds.Length));
//
//                            totalCorrect += correct ? 1 : 0;
//                            totalScore += score;
//                            totalTime += time;
//
//
//                        }
//                        ConsoleWriteline(string.Format("TotalCorrect:{0}/{1}", totalCorrect, possibleCorrect));
//                        ConsoleWriteline(string.Format("TotalScore:{0}/{1}", totalScore, possibleScore));
//                        ConsoleWriteline(string.Format("TotalTime:{0}/{1}", totalTime, possibleTime));
//                    }
//                    break;
                default:
                    Winboard.ProcessCmd(input);
                    /*        
            break;
            }*/
                    return true;
            }
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            Logger.WriteLine(Board.Game.PrintBoard());
            Logger.Close();
        }
    }
}
