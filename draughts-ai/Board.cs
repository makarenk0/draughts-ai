using System;
using System.Collections.Generic;
using System.Text;

namespace draughts_ai
{
    class Board
    {

        // 0 -empty cell
        // 1 - my man
        // 2 - enemy man
        // 3 - my king
        // 4 - enemy king
        private int[,] Matrix { get; set; }
        
        public Board()
        {
            Matrix = new int[8, 8];

        }
    }
}
