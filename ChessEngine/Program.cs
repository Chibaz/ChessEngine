using System;
using System.IO;
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
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\LogFile.txt";
            Logger = new StreamWriter(path, false);
            Logger.WriteLine("ChessEngine v{0}.{1} by Kasper Wind, Denmark",
                   typeof (Program).Assembly.GetName().Version.Major, typeof (Program).Assembly.GetName().Version.Minor);
            Logger.WriteLine((0x88 & 146) == 0);

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
                Logger.WriteLine(ex.Message);
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
                default:
                    Winboard.ProcessCmd(input);
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
