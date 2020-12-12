﻿using System;
using System.Collections.Generic;
using System.Text;

namespace draughts_ai
{
    class MainHandler
    {
        public String Filename { get; set; }
        public RequestsExecutor ReqExecutor { get; set; }
        public bool Running { get; set; }

        public MainHandler(string filename_URLs)
        {
            Filename = filename_URLs;
            //ReqExecutor = new RequestsExecutor(Filename);
            Running = true;
            EventLoop();
        }

        private void EventLoop()
        {
            Board b = new Board();
            //b.Matrix = ReqExecutor.GetBoardState();
            b.PrintBoard();
            b.ApplyStep(new int[] { 32, 27});
            Console.WriteLine();
            b.PrintBoard();
            //List<KeyValuePair<int[], int>> steps = b.GetPossibleSteps(0);
            while (Running)
            {

            }

        }

    }
}
