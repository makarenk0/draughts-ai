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
            ReqExecutor = new RequestsExecutor(Filename);
            Running = true;
            SearchingAnswer = false;

            //EventLoop();

            TimerCallback timeCB = new TimerCallback(timer_Tick);
            timer = new Timer(timeCB, null, 0, 1000);


            while (Running)
            {
                Thread.Sleep(1000);
            }
            timer.Dispose();
        }

        private void timer_Tick(object state)
        {
            KeyValuePair<int, byte[,]> tryRequest = ReqExecutor.GetBoardState();
            if (tryRequest.Key == 1 && !SearchingAnswer)
            {
                SearchingAnswer = true;

                Board freshData = new Board();
                freshData.Matrix = tryRequest.Value;
                MinimaxTree tree = new MinimaxTree(freshData, ReqExecutor.MyColor, ReqExecutor.AvailableTime);
                String move = String.Join(", ", tree.Answer);
                ReqExecutor.MakeMove(String.Concat("[", move, "]"));

                SearchingAnswer = false;
            }
            else if(tryRequest.Key == 2)
            {
                Running = false;
            }
        }

        private void EventLoop()
        {
            Board b = new Board();
            //b.Matrix = ReqExecutor.GetBoardState();
            b.PrintBoard();

            MinimaxTree tree = new MinimaxTree(b, "RED");
            //Console.WriteLine(b.PlayerScore(0));
            //List<KeyValuePair<int[], int>> steps = b.GetPossibleSteps(0);
            while (Running)
            {

            }

        }

    }
}
