using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace draughts_ai
{
    class MinimaxTree
    {

        public Board GameBoard { get; set; }
        private Stack<Node> NodesStack { get; set; }
        private String MyColor { get; set; }
        private const int _treeDepth = 1;   //make time dependence
        private Node Root { get; set; }

        public MinimaxTree(Board board, String color)
        {
            GameBoard = board;
            MyColor = color;
            NodesStack = new Stack<Node>();
            ConstructTree();
        }


        private void ConstructTree()
        {
            Root = new Node(0, GameBoard);
            NodesStack.Push(Root);

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
                    //Console.WriteLine();

                    if(peek.NextNodes.Count != 0)
                    {
                        peek.Benefit = peek.AgentIndex == 0 ? FindMaximumBenefit(peek) : FindMinimumBenefit(peek);
                    }
                    NodesStack.Pop();
                }
            }
            //TO DO: debug
            NodesStack.Peek().Benefit = FindMaximumBenefit(Root);  //last action for root (root is always max agent)
            Root.PrintPretty("", true, "");


            Node step = Root.NextNodes.Find(x => x.Key.Benefit == Root.Benefit).Key;
            step.State.PrintBoard();
        }

        private int FindMinimumBenefit(Node peek)
        {
            return peek.NextNodes.Min(x => x.Key.Benefit);
        }

        private int FindMaximumBenefit(Node peek)
        {
            return peek.NextNodes.Max(x => x.Key.Benefit);
        }

        private void BuildNextNodes(Node peek)
        {
            int nextAgent = peek.AgentIndex == 0 ? 1 : 0;
            List<KeyValuePair<int[], int>> nextSteps = peek.State.GetPossibleSteps(nextAgent == 0 ? (MyColor == "RED" ? 1 : 0) : (MyColor == "RED" ? 0 : 1));
         
            if(nextSteps.Count == 0)  // if there are no options to go
            {
                ComputeBenefitAndFinalize(peek);
            }
            else
            {
                foreach (var step in nextSteps)
                {
                    Board afterStep = new Board(peek.State);
                    afterStep.ApplyStep(step.Key);
                    Node next = new Node(nextAgent, afterStep);

                    if (NodesStack.Count == (_treeDepth * 2) + 1)
                    {
                        ComputeBenefitAndFinalize(next);
                    }

                    peek.NextNodes.Add(new KeyValuePair<Node, int[]>(next, step.Key));
                }
            }
            
        }

        public void ComputeBenefitAndFinalize(Node node)
        {
            node.Full = true;
            node.Benefit = MyColor == "RED" ? node.State.SuperiorityForAgent(0) : node.State.SuperiorityForAgent(1);
        }

    }
}
