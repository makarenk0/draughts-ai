using System;
using System.Collections.Generic;
using System.Linq;

namespace draughts_ai
{
    class Board
    {

        // 0 -empty cell
        // 1 - red man
        // 2 - black man
        // 3 - red king
        // 4 - black king
        public int[,] Matrix { get; set; }

        public int[,] Test = new int[,]
        {
            { 0, 2, 0, 0, 0, 0, 0, 2},
            { 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 3, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 4, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 4},
            { 0, 0, 0, 0, 0, 0, 0, 0}
        };

        public Board()
        {
            Matrix = new int[8, 8];

            Matrix = Test;
        }

        //agent index 0 - Red
        //agent index 1 - Black
        // KeyValuePair 1)steps 2)amount of beaten men
        public List<KeyValuePair<int[], int>> GetPossibleSteps(int agentIndex)
        {

            return SimpleSteps(agentIndex);
        }

        private List<KeyValuePair<int[], int>> SimpleSteps(int agentIndex)
        {
            List<KeyValuePair<int[], int>> steps = new List<KeyValuePair<int[], int>>();
            int manId = agentIndex == 0 ? 1 : 2;
            int kingId = agentIndex == 0 ? 3 : 4;

            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    if(Matrix[i, j] == manId)
                    {
                        steps.Add(agentIndex == 0 && EmptyAndExist(j + 1, i - 1) ? new KeyValuePair<int[], int>(new int[] { ConvertToPDN(j, i), ConvertToPDN(j + 1, i - 1) }, 0) : new KeyValuePair<int[], int>(new int[0], 0));
                        steps.Add(agentIndex == 0 && EmptyAndExist(j + 1, i + 1) ? new KeyValuePair<int[], int>(new int[] { ConvertToPDN(j, i), ConvertToPDN(j + 1, i + 1) }, 0) : new KeyValuePair<int[], int>(new int[0], 0));
                        steps.Add(agentIndex == 1 && EmptyAndExist(j - 1, i - 1) ? new KeyValuePair<int[], int>(new int[] { ConvertToPDN(j, i), ConvertToPDN(j - 1, i - 1) }, 0) : new KeyValuePair<int[], int>(new int[0], 0));
                        steps.Add(agentIndex == 1 && EmptyAndExist(j - 1, i + 1) ? new KeyValuePair<int[], int>(new int[] { ConvertToPDN(j, i), ConvertToPDN(j - 1, i + 1) }, 0) : new KeyValuePair<int[], int>(new int[0], 0));
                    }
                    else if(Matrix[i, j] == kingId)
                    {
                        steps.AddRange(KingSimpleMove(j, i));
                    }
                }
                
            }
            return steps.Where(x => x.Key.Length != 0).ToList();
        }

        private List<KeyValuePair<int[], int>> KingSimpleMove(int x, int y)
        {
            List<KeyValuePair<int[], int>> moves = new List<KeyValuePair<int[], int>>();
            int[] arr = new int[] { -1, 1 };
            for (int i = 0; i < 4; i++) 
            {
                //init
                int startX = x + arr[i % 2], startY = y + arr[i / 2];
                moves.Add(EmptyAndExist(startX, startY) ? new KeyValuePair<int[], int>(new int[] { ConvertToPDN(x, y), ConvertToPDN(startX, startY) }, 0) : new KeyValuePair<int[], int>(new int[0], 0));
                
                //first step  TO DO: reduce dublicating code
                startX += arr[i % 2];
                startY += arr[i / 2];

                //steps till cant go
                while (EmptyAndExist(startX, startY))
                {
                    int sourceLength = moves.Last().Key.Length;
                    int[] newSteps = new int[sourceLength + 1];
                    Array.Copy(moves.Last().Key, 0, newSteps, 0, sourceLength);
                    newSteps[sourceLength] = ConvertToPDN(startX, startY);
                    moves.Add(new KeyValuePair<int[], int>(newSteps, 0));

                    startX += arr[i % 2];
                    startY += arr[i / 2];
                }
            }

            return moves;

        }


        private bool EmptyAndExist(int x, int y)
        {
            if (x >= 0 && x < 8 && y >= 0 && y < 8 && Matrix[y, x] == 0)
            {
                return true;
            }
            return false;
        }

        public void DefaultBoardSet()
        {

        }

        public KeyValuePair<int, int> ConvertFromPDN(int n)
        {
            return new KeyValuePair<int, int>((n - 1) / 4, (((n - 1) * 2) % 8) + ((((n - 1) / 4) + 1) % 2));
        }

        public int ConvertToPDN(int x, int y)
        {
            return x * 4 + y / 2 + 1;
        }

        public void PrintBoard()
        {
            for(int i = 0; i < Matrix.GetLength(0); i++)
            {
                for(int j = 0; j < Matrix.GetLength(1); j++)
                {
                    Console.Write(String.Concat(Matrix[i, j], " "));
                }
                Console.WriteLine();
            }
        }
    }
}
