using System;
using System.Collections.Generic;
using System.Text;

namespace draughts_ai
{
    class Node
    {
        public int AgentIndex { get; set; }  //0 player, 1 - enemy
        public List<Node> NextNodes { get; set; }
        public List<int[]> NextSteps { get; set; }
        public int Benefit { get; set; }
        public bool Full { get; set; }

        public Board State { get; set; }

        public Node(int agentIndex)
        {
            AgentIndex = agentIndex;
            NextNodes = new List<Node>();
        }


        public bool AllNextVisited()
        {
            if (NextNodes.Count == 0)
            {
                return false;
            }
            foreach (var node in NextNodes)
            {
                if (!node.Full)
                {
                    return false;
                }
            }
            return true;
        }

        public Node FindFirstEmpty()
        {
            for (int i = 0; i < NextNodes.Count; i++)
            {
                if (!NextNodes[i].Full)
                {
                    return NextNodes[i];
                }
            }
            return null;
        }
    }
}
