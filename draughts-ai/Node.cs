using System;
using System.Collections.Generic;
using System.Text;

namespace draughts_ai
{
    class Node
    {
        public int AgentIndex { get; set; }
        public List<Node> NextNodes { get; set; }
        public int[] Step { get; set; }
        public bool Full { get; set; }

        public Node()
        {
            NextNodes = new List<Node>();
        }
    }
}
