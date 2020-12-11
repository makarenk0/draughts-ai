﻿using System;
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
            { 0, 0, 0, 2, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 2, 0, 2, 0, 0, 0, 0},
            { 0, 0, 3, 0, 0, 0, 0, 0},
            { 0, 2, 0, 2, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 2, 0, 0, 0, 0, 0, 0},
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
            List<KeyValuePair<int[], int>> result = new List<KeyValuePair<int[], int>>();
            result.AddRange(BeatingSteps(agentIndex));
            result.AddRange(SimpleSteps(agentIndex));
            return result;
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

        private List<KeyValuePair<int[], int>> BeatingSteps(int agentIndex)
        {
            int manId = agentIndex == 0 ? 1 : 2;
            int kingId = agentIndex == 0 ? 3 : 4;
            List<KeyValuePair<int[], int>> result = new List<KeyValuePair<int[], int>>();
            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    if(Matrix[i, j] == manId)
                    {
                        result.AddRange(BeatFromCell(agentIndex, j, i, 0, new int[1] { ConvertToPDN(j, i)}));
                    }
                    else if(Matrix[i, j] == kingId)
                    {
                        result.AddRange(BeatFromCell(agentIndex, j, i, 0, new int[1] { ConvertToPDN(j, i)}, true));
                    }
                }
            }
            return result;
        }

        private List<KeyValuePair<int[], int>> BeatFromCell(int agentIndex, int x, int y, int alreadyBeaten, int[] path, bool isKing = false)
        {
            int[] toBeat = new int[] { agentIndex == 1 ? 1 : 2, agentIndex == 1 ? 3 : 4 };
            int[] tryFuther = new int[path.Length + 1];
            Array.Copy(path, 0, tryFuther, 0, path.Length);
            List<KeyValuePair<int[], int>> steps = new List<KeyValuePair<int[], int>>();
            if (isKing)
            {
                int[] arr = new int[] { -1, 1 };
                for (int i = 0; i < 4; i++)
                {
                    int directionX = arr[i % 2];
                    int directionY = arr[i / 2];
                    
                    //preventing jumping "forward and backward"
                    KeyValuePair<int, int> prevPoint = path.Length > 1 ? ConvertFromPDN(path[path.Length - 2]) : new KeyValuePair<int, int>(-1, -1);
                    if (x + (2*directionX) == prevPoint.Key && y + (2*directionY) == prevPoint.Value) continue;

                    if (EmptyAndExist(x + (directionX * 2), y + (directionY * 2)) && IsPresent(x + directionX, y + directionY, toBeat))
                    {
                        tryFuther[path.Length] = ConvertToPDN(x + (directionX * 2), y + (directionY * 2));
                        steps.Add(new KeyValuePair<int[], int>((int[])tryFuther.Clone(), alreadyBeaten + 1));
                        steps.AddRange(BeatFromCell(agentIndex, x + (directionX * 2), y + (directionY * 2), alreadyBeaten + 1, tryFuther, isKing));
                    }

                }
            }
            else
            {
                int directionX = agentIndex == 0 ? 1 : -1;
                for (int i = 0; i < 2; i++)
                {
                    int directionY = (i == 0) ? 1 : -1;
                    if (EmptyAndExist(x + (directionX * 2), y + (directionY * 2)) && IsPresent(x + directionX, y + directionY, toBeat))
                    {
                        tryFuther[path.Length] = ConvertToPDN(x + (directionX * 2), y + (directionY * 2));
                        steps.Add(new KeyValuePair<int[], int>((int[])tryFuther.Clone(), alreadyBeaten + 1));
                        steps.AddRange(BeatFromCell(agentIndex, x + (directionX * 2), y + (directionY * 2), alreadyBeaten + 1, tryFuther));
                    }

                }
            }
            return steps;
        }

        private List<KeyValuePair<int[], int>> KingSimpleMove(int x, int y)
        {
            List<KeyValuePair<int[], int>> moves = new List<KeyValuePair<int[], int>>();
            int[] arr = new int[] { -1, 1 };
            for (int i = 0; i < 4; i++) 
            {
                //init
                int startX = x + arr[i % 2], startY = y + arr[i / 2];
                if (EmptyAndExist(startX, startY)) {
                    moves.Add(new KeyValuePair<int[], int>(new int[] { ConvertToPDN(x, y), ConvertToPDN(startX, startY) }, 0));

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

        private bool IsPresent(int x, int y, int[] ids)
        {
            return ids.Where(el => el == Matrix[y, x]).ToArray().Length != 0;
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