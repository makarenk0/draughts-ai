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
        private Timer timer;

        public MainHandler(string filename_URLs)
        {
            Filename = filename_URLs;
            ReqExecutor = new RequestsExecutor(Filename);
            Thread.Sleep(1000);
            Running = true;
            

            TimerCallback timeCB = new TimerCallback(timer_Tick);
            timer = new Timer(timeCB, null, 0, 1000);


            while (Running)
            {
                Thread.Sleep(1000);
            }

        }

        private void timer_Tick(object state)
        {
            KeyValuePair<bool, int[,]> tryRequest = ReqExecutor.GetBoardState();
            if (tryRequest.Key)
            {
                Board freshData = new Board();
                freshData.Matrix = tryRequest.Value;
                MinimaxTree tree = new MinimaxTree(freshData, ReqExecutor.MyColor);
                String move = String.Join(", ", tree.Answer);
                ReqExecutor.MakeMove(String.Concat("[", move, "]"));
            }
        }

        //private void EventLoop()
        //{
        //    Board b = new Board();
        //    //b.Matrix = ReqExecutor.GetBoardState();
        //    b.PrintBoard();
           
        //    MinimaxTree tree = new MinimaxTree(b, "RED");
        //    //Console.WriteLine(b.PlayerScore(0));
        //    //List<KeyValuePair<int[], int>> steps = b.GetPossibleSteps(0);
        //    while (Running)
        //    {

        //    }

        //}

    }
}
