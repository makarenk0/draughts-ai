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
        private const int _minTreeDepth = 2;   //min tree depth
        private Node Root { get; set; }
        public byte[] Answer { get; set; }

        private bool FirstPossibleVariants { get; set; }
        public bool MustBeat { get; set; }
        public int TreeDepth { get; set; }

        public bool Computing { get; set; }

        //public Dictionary<int, List<sbyte>> _benefitsOnLevel;

        public MinimaxTree(Board board, String color)
        {
            TreeDepth = _minTreeDepth;
            Computing = true;
            GameBoard = board;
            MyColor = color;
            NodesStack = new Stack<Node>();
            FirstPossibleVariants = true;
            MustBeat = false;

            //_benefitsOnLevel = new Dictionary<int, List<sbyte>>();

            Console.WriteLine("New step computing start:");

            Root = new Node(0, GameBoard);
            NodesStack.Push(Root);

            ConstructTree();  //default answer


        }


        public void ConstructTree()
        {

            while (!(NodesStack.Count == 1 && NodesStack.Peek().AllNextVisited()) && Computing)
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
                    NodesStack.Pop();
                    peek.ResetNextNodes();
                }
            }
            Root.ResetNextNodes();



            if (Computing)
            {
                if (MustBeat)
                {
                    Console.Write("Must beat step (depth 1)");
                    NodesStack.Peek().Benefit = FindMaximumBenefit(Root);
                }
                else
                {
                    ComputeAnswer();
                }

                KeyValuePair<Node, byte[]> pair = Root.NextNodes.Find(x => x.Key.Benefit == Root.Benefit);
                Answer = pair.Value;
            }
            //step.State.PrintBoard();
            //Root.PrintPretty("", true, "");
        }

        private void ComputeAnswer()//getting the tree result comapring nodes
        {

            while (!(NodesStack.Count == 1 && NodesStack.Peek().AllNextVisited()))
            {
                Node peek = NodesStack.Peek();
                if (peek.AllNextVisited())
                {
                    peek.Benefit = peek.AgentIndex == 0 ? FindMaximumBenefit(peek) : FindMinimumBenefit(peek);

                    //_benefitsOnLevel.Add(NodesStack.Count, new List<sbyte>());//attempt to do alpha beta for minimax tree
                    //_benefitsOnLevel[NodesStack.Count].Add(peek.Benefit);
                    //if ((NodesStack.Count % 2 == 1 && _benefitsOnLevel[NodesStack.Count].Max() > peek.Benefit) || (NodesStack.Count % 2 == 0 && _benefitsOnLevel[NodesStack.Count].Min() < peek.Benefit))
                    //{
                    //    NodesStack.Pop();
                    //}
                    peek.Full = true;
                    NodesStack.Pop();
                    peek.ResetNextNodes();

                }
                else
                {
                    if (peek.NextNodes.Count == 0)
                    {
                        NodesStack.Pop();
                        peek.Full = true;
                    }
                    else
                    {
                        Node next = peek.FindFirstEmpty();
                        NodesStack.Push(next);
                    }
                }
            }
            NodesStack.Peek().Benefit = FindMaximumBenefit(Root);
            Root.ResetNextNodes();
            //logging tree depth
            Console.Write(String.Concat("Try depth: ", TreeDepth * 2 + 1, ", "));
        }


        private sbyte FindMinimumBenefit(Node peek)
        {
            return peek.NextNodes.Min(x => x.Key.Benefit);
        }

        private sbyte FindMaximumBenefit(Node peek)
        {
            return peek.NextNodes.Max(x => x.Key.Benefit);
        }

        private void BuildNextNodes(Node peek)// build possible variants of the board and put them in nodes
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

                    if (NodesStack.Count == (TreeDepth * 2) + 1 || MustBeat)
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