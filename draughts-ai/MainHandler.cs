using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace draughts_ai
{
    class MainHandler
    {
        public String Filename { get; set; }
        public RequestsExecutor ReqExecutor { get; set; }
        public bool Running { get; set; }
        private bool SearchingAnswer { get; set; }
        private Timer timer;

        public MainHandler(string filename_URLs)
        {
            Filename = filename_URLs;
            try
            {
                ReqExecutor = new RequestsExecutor(Filename);
                Console.WriteLine(String.Concat("My color: ", ReqExecutor.MyColor, "\n"));
                Running = true;
                SearchingAnswer = false;


                TimerCallback timeCB = new TimerCallback(timer_Tick);
                timer = new Timer(timeCB, null, 0, 200);


                while (Running)
                {
                    Thread.Sleep(1000);
                }
                timer.Dispose();
                Console.WriteLine("End");
            }
            catch (Exception)
            {
                Console.WriteLine("Closing...");
            }
           
        }

        private void timer_Tick(object state)
        {
            KeyValuePair<int, byte[,]> tryRequest = ReqExecutor.GetBoardState();
            if (tryRequest.Key == 1 && !SearchingAnswer)
            {
                SearchingAnswer = true;

                Board freshData = new Board();
                freshData.Matrix = tryRequest.Value;


                MinimaxTree tree = null;
                Console.WriteLine(ReqExecutor.AvailableTime);
           
                    Task.Delay((int)((ReqExecutor.AvailableTime * 1000) - 210)).ContinueWith(t => { //makes move when time is up, -150 just to be sure
                        tree.Computing = false;
                        for (int i = 1; i < tree.Answer.Length; i++)
                        {
                            String move = String.Concat("[", tree.Answer[i - 1], ", ", tree.Answer[i], "]");
                            ReqExecutor.MakeMove(move);
                            Thread.Sleep(50);
                        }
                        Console.WriteLine('\n');
                        SearchingAnswer = false;
                    });

                    tree = new MinimaxTree(freshData, ReqExecutor.MyColor);
                    while (SearchingAnswer)// if there is enough time add depth to tree
                    {
                        if (!tree.MustBeat)
                        {
                            tree.TreeDepth += 1;
                            tree.ConstructTree();
                        }
                    }
                    tree = null;
                    GC.Collect();  //Garbage collector forcing 
            }
            else if(tryRequest.Key == 2)
            {
                Running = false;
            }
        }

        private void LocalTest()
        {
            Board b = new Board();
            //b.Matrix = ReqExecutor.GetBoardState();
            b.PrintBoard();

            MinimaxTree tree = new MinimaxTree(b, "RED");
            String move = String.Join(", ", tree.Answer);
            //Console.WriteLine(b.PlayerScore(0));
            //List<KeyValuePair<int[], int>> steps = b.GetPossibleSteps(0);
            while (Running)
            {
                Thread.Sleep(1000);
            }

        }

    }
}
