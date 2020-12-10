using System;
using System.Collections.Generic;
using System.Text;

namespace draughts_ai
{
    class MainHandler
    {
        public String Filename { get; set; }
        public RequestsExecutor ReqExecutor { get; set; }

        public MainHandler(string filename_URLs)
        {
            Filename = filename_URLs;
            ReqExecutor = new RequestsExecutor(Filename);
        }
    }
}
