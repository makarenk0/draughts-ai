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
        private const int _treeDepth = 2;   //make time dependence
        private Node Root { get; set; }
        public byte[] Answer { get; set; }

        private bool FirstPossibleVariants { get; set; }
        private bool MustBeat { get; set; }


        public MinimaxTree(Board board, String color, double availableTime)
        {
            GameBoard = board;
            MyColor = color;
            NodesStack = new Stack<Node>();
            FirstPossibleVariants = true;
            MustBeat = false;
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
                        if (MustBeat) break;
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
            


            KeyValuePair<Node, byte[]> pair = Root.NextNodes.Find(x => x.Key.Benefit == Root.Benefit);
            Answer = pair.Value;
            //step.State.PrintBoard();

            //Root.PrintPretty("", true, "");
        }

        private sbyte FindMinimumBenefit(Node peek)
        {
            return peek.NextNodes.Min(x => x.Key.Benefit);
        }

        private sbyte FindMaximumBenefit(Node peek)
        {
            return peek.NextNodes.Max(x => x.Key.Benefit);
        }

        private void BuildNextNodes(Node peek)
        {
            byte nextAgent = (byte)(peek.AgentIndex == 0 ? 1 : 0);
            List<KeyValuePair<byte[], byte>> nextSteps = peek.State.GetPossibleSteps(nextAgent == 0 ? (MyColor == "RED" ? (byte)1 : (byte)0) : (MyColor == "RED" ? (byte)0 : (byte)1));


            if (FirstPossibleVariants && nextSteps.Exists(x => x.Value != 0))
            {
                MustBeat = true;
            }
            FirstPossibleVariants = false;


            if (nextSteps.Count == 0)  // if there are no options to go
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

                    peek.NextNodes.Add(new KeyValuePair<Node, byte[]>(next, step.Key));
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
