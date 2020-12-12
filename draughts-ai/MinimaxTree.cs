using System;
using System.Collections.Generic;
using System.Text;

namespace draughts_ai
{
    class MinimaxTree
    {

        public Board GameBoard { get; set; }
        private Stack<Node> NodesStack { get; set; }

        public MinimaxTree(Board board)
        {
            GameBoard = board;
            NodesStack = new Stack<Node>();
        }


        private void ConstructTree()
        {
            NodesStack.Push(new Node(0));

            while (!(NodesStack.Count == 1 && NodesStack.Peek().AllNextVisited()))
            {
                Node peek = NodesStack.Peek();
                if (!peek.Full)
                {
                    if (peek.NextNodes.Count == 0)  //means that not built yet
                    {
                        BuildNextNodes(peek);
                    }
                    else
                    {
                        Node next = peek.FindFirstEmpty();
                        if (next == null)  // just visited all next nodes
                        {
                            peek.Full = true;
                        }
                        else // built but not all is visited yet
                        {
                            NodesStack.Push(next);
                        }
                    }
                }
                else  // visited all next nodes -> go back
                {
                    if (peek.AgentIndex != 0)  // it is min agent
                    {
                        //peek.Benefits = FindMaximumBenefit(peek);   //TO DO: debug
                    }
                    else // now max agent must make his decision
                    {
                        //peek.Benefits = FindMinimumBenefit(peek);  //TO DO: debug
                    }
                    NodesStack.Pop();
                }
            }
            //TO DO: debug
           //NodesStack.Peek.Benefits = FindMinimumBenefit(Root);  //last action for root (root is always max agent)
        }



        private void BuildNextNodes(Node peek)
        {
            int nextAgent = peek.AgentIndex == 0 ? 1 : 0;

        }

    }
}
