using System;
using System.Collections.Generic;
using System.Text;

namespace draughts_ai
{
    class Node
    {
        public int AgentIndex { get; set; }  //0 player, 1 - enemy
        public List<KeyValuePair<Node, int[]>> NextNodes { get; set; }
        public int Benefit { get; set; }
        public bool Full { get; set; }

        public Board State { get; set; }

        public Node(int agentIndex, Board state)
        {
            AgentIndex = agentIndex;
            NextNodes = new List<KeyValuePair<Node, int[]>>();
            State = new Board(state);
        }


        public bool AllNextVisited()
        {
            if (NextNodes.Count == 0)
            {
                return false;
            }
            foreach (var node in NextNodes)
            {
                if (!node.Key.Full)
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
                if (!NextNodes[i].Key.Full)
                {
                    return NextNodes[i].Key;
                }
            }
            return null;
        }


        public void PrintPretty(string indent, bool last, string step)
        {
            Console.Write(indent);
            if (last)
            {
                Console.Write("\\-");
                indent += "  ";
            }
            else
            {
                Console.Write("|-");
                indent += "| ";
            }

            Console.WriteLine(String.Concat("Ag: ", AgentIndex, " Steps:[", step, "]", " Benefit(", Benefit, ")"));

            for (int i = 0; i < NextNodes.Count; i++)
                NextNodes[i].Key.PrintPretty(indent, i == NextNodes.Count - 1, String.Join(',', NextNodes[i].Value));
        }

    }
}
