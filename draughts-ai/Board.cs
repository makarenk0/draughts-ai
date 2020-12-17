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
        public byte[,] Matrix { get; set; }

        public static byte[,] Test = new byte[,]
        {
            { 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 2, 0, 2, 0, 0},
            { 0, 0, 3, 0, 0, 0, 0, 0},
            { 0, 0, 0, 2, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 2, 0, 2, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0}
        };

        public static byte[,] DefaultTest = new byte[,]
        {
            { 0, 1, 0, 0, 0, 2, 0, 2},
            { 1, 0, 1, 0, 0, 0, 2, 0},
            { 0, 1, 0, 0, 0, 2, 0, 2},
            { 1, 0, 1, 0, 0, 0, 2, 0},
            { 0, 1, 0, 0, 0, 2, 0, 2},
            { 1, 0, 1, 0, 0, 0, 2, 0},
            { 0, 1, 0, 0, 0, 2, 0, 2},
            { 1, 0, 1, 0, 0, 0, 2, 0}
        };

        public Board()
        {
            Matrix = new byte[8, 8];

            Matrix = DefaultTest;
        }

        public Board(Board copy)
        {
            Matrix = (byte[,])copy.Matrix.Clone();
        }
        
        //agent index 0 - Red
        //agent index 1 - Black
        // KeyValuePair 1)steps 2)amount of beaten men
        public List<KeyValuePair<byte[], byte>> GetPossibleSteps(byte agentIndex)
        {
            List<KeyValuePair<byte[], byte>> result = new List<KeyValuePair<byte[], byte>>();
            result.AddRange(BeatingSteps(agentIndex));
            result.AddRange(SimpleSteps(agentIndex));
            return result;
        }

        private List<KeyValuePair<byte[], byte>> SimpleSteps(int agentIndex)
        {
            List<KeyValuePair<byte[], byte>> steps = new List<KeyValuePair<byte[], byte>>();
            int manId = agentIndex == 0 ? 1 : 2;
            int kingId = agentIndex == 0 ? 3 : 4;

            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    if(Matrix[i, j] == manId)
                    {
                        steps.Add(agentIndex == 0 && EmptyAndExist(j + 1, i - 1) ? new KeyValuePair<byte[], byte>(new byte[] { ConvertToPDN(j, i), ConvertToPDN(j + 1, i - 1) }, 0) : new KeyValuePair<byte[], byte>(new byte[0], 0));
                        steps.Add(agentIndex == 0 && EmptyAndExist(j + 1, i + 1) ? new KeyValuePair<byte[], byte>(new byte[] { ConvertToPDN(j, i), ConvertToPDN(j + 1, i + 1) }, 0) : new KeyValuePair<byte[], byte>(new byte[0], 0));
                        steps.Add(agentIndex == 1 && EmptyAndExist(j - 1, i - 1) ? new KeyValuePair<byte[], byte>(new byte[] { ConvertToPDN(j, i), ConvertToPDN(j - 1, i - 1) }, 0) : new KeyValuePair<byte[], byte>(new byte[0], 0));
                        steps.Add(agentIndex == 1 && EmptyAndExist(j - 1, i + 1) ? new KeyValuePair<byte[], byte>(new byte[] { ConvertToPDN(j, i), ConvertToPDN(j - 1, i + 1) }, 0) : new KeyValuePair<byte[], byte>(new byte[0], 0));
                    }
                    else if(Matrix[i, j] == kingId)
                    {
                        steps.AddRange(KingSimpleMove(j, i));
                    }
                }
                
            }
            return steps.Where(x => x.Key.Length != 0).ToList();
        }

        private List<KeyValuePair<byte[], byte>> BeatingSteps(byte agentIndex)
        {
            int manId = agentIndex == 0 ? 1 : 2;
            int kingId = agentIndex == 0 ? 3 : 4;
            List<KeyValuePair<byte[], byte>> result = new List<KeyValuePair<byte[], byte>>();
            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    if(Matrix[i, j] == manId)
                    {
                        result.AddRange(BeatFromCell(agentIndex, j, i, 0, new byte[1] { ConvertToPDN(j, i)}));
                    }
                    else if(Matrix[i, j] == kingId)
                    {
                        result.AddRange(BeatFromCell(agentIndex, j, i, 0, new byte[1] { ConvertToPDN(j, i)}, true));
                    }
                }
            }
            return result;
        }

        private List<KeyValuePair<byte[], byte>> BeatFromCell(byte agentIndex, int x, int y, byte alreadyBeaten, byte[] path, bool isKing = false)
        {
            byte[] toBeat = new byte[] { agentIndex == 1 ? (byte)1 : (byte)2, agentIndex == 1 ? (byte)3 : (byte)4 };
            byte[] tryFuther = new byte[path.Length + 1];
            Array.Copy(path, 0, tryFuther, 0, path.Length);
            List<KeyValuePair<byte[], byte>> steps = new List<KeyValuePair<byte[], byte>>();
            if (isKing)
            {
                int[] arr = new int[] { -1, 1 };
                for (int i = 0; i < 4; i++)
                {
                    int directionX = arr[i % 2];
                    int directionY = arr[i / 2];
                    
                    //preventing jumping "forward and backward" and looping
                    if (path.Where(el => el == ConvertToPDN(x + (2*directionX), y + (2*directionY))).Count() != 0) continue;

                    if (EmptyAndExist(x + (directionX * 2), y + (directionY * 2)) && IsPresent(x + directionX, y + directionY, toBeat))
                    {
                        tryFuther[path.Length] = ConvertToPDN(x + (directionX * 2), y + (directionY * 2));
                        steps.Add(new KeyValuePair<byte[], byte>((byte[])tryFuther.Clone(), (byte)(alreadyBeaten + 1)));
                        steps.AddRange(BeatFromCell(agentIndex, x + (directionX * 2), y + (directionY * 2), (byte)(alreadyBeaten + 1), tryFuther, isKing));
                    }

                }
            }
            else
            {
                int directionX = agentIndex == 0 ? 1 : -1;
                for (int i = 0; i < 2; i++)
                {
                    int directionY = (i == 0) ? 1 : -1;

                    //Don't need <preventing jumping "forward and backward" and looping> because men can only beat forward
                

                    if (EmptyAndExist(x + (directionX * 2), y + (directionY * 2)) && IsPresent(x + directionX, y + directionY, toBeat))
                    {
                        tryFuther[path.Length] = ConvertToPDN(x + (directionX * 2), y + (directionY * 2));
                        steps.Add(new KeyValuePair<byte[], byte>((byte[])tryFuther.Clone(), (byte)(alreadyBeaten + 1)));
                        steps.AddRange(BeatFromCell(agentIndex, x + (directionX * 2), y + (directionY * 2), (byte)(alreadyBeaten + 1), tryFuther));
                    }

                }
            }
            return steps;
        }

        private List<KeyValuePair<byte[], byte>> KingSimpleMove(int x, int y)
        {
            List<KeyValuePair<byte[], byte>> moves = new List<KeyValuePair<byte[], byte>>();
            int[] arr = new int[] { -1, 1 };
            for (int i = 0; i < 4; i++) 
            {
                //init
                int startX = x + arr[i % 2], startY = y + arr[i / 2];
                if (EmptyAndExist(startX, startY)) {
                    moves.Add(new KeyValuePair<byte[], byte>(new byte[] { ConvertToPDN(x, y), ConvertToPDN(startX, startY) }, 0));

                    ////first step  TO DO: reduce dublicating code   // if you want standart draughts with king moving along digonal unlimitly uncomment this code
                    //startX += arr[i % 2];
                    //startY += arr[i / 2];

                    ////steps till cant go
                    //while (EmptyAndExist(startX, startY))
                    //{
                    //    //int sourceLength = moves.Last().Key.Length;
                    //    int[] newSteps = new int[2];
                    //    newSteps[0] = moves.Last().Key[0];
                    //    //Array.Copy(moves.Last().Key, 0, newSteps, 0, sourceLength);
                    //    newSteps[1] = ConvertToPDN(startX, startY);
                    //    moves.Add(new KeyValuePair<int[], int>(newSteps, 0));

                    //    startX += arr[i % 2];
                    //    startY += arr[i / 2];
                    //}
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

        private bool IsPresent(int x, int y, byte[] ids)
        {
            return ids.Where(el => el == Matrix[y, x]).ToArray().Length != 0;
        }

        public KeyValuePair<int, int> ConvertFromPDN(int n)
        {
            return new KeyValuePair<int, int>((n - 1) / 4, (((n - 1) * 2) % 8) + ((((n - 1) / 4) + 1) % 2));
        }

        public byte ConvertToPDN(int x, int y)
        {
            return (byte) (x * 4 + y / 2 + 1);
        }


        public void ApplyStep(byte[] steps)  // doesn't validate checkers rules, apply only valid steps!!!
        {
            List<KeyValuePair<int, int>> coords = new List<KeyValuePair<int, int>>();
            for(int i = 0; i< steps.Length; i++)
            {
                coords.Add(ConvertFromPDN(steps[i]));
            }

            for(int i = 1; i < coords.Count; i++)
            {
                byte elementIndex = Matrix[coords.ElementAt(i - 1).Value, coords.ElementAt(i - 1).Key];
                Matrix[coords.ElementAt(i - 1).Value, coords.ElementAt(i - 1).Key] = 0;
                Matrix[coords.ElementAt(i).Value, coords.ElementAt(i).Key] = elementIndex;

                //in case of destroying enemy
                int deltaX = coords.ElementAt(i).Key - coords.ElementAt(i - 1).Key;
                int deltaY = coords.ElementAt(i).Value - coords.ElementAt(i - 1).Value;
                if(Math.Abs(deltaX) == 2)
                {
                    KeyValuePair<int, int> destroyCoord = new KeyValuePair<int, int>(coords.ElementAt(i-1).Key + deltaX/2, coords.ElementAt(i - 1).Value + deltaY / 2);
                    Matrix[destroyCoord.Value, destroyCoord.Key] = 0;
                }
            }
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


        public sbyte SuperiorityForAgent(byte agentIndex)
        {
            if(agentIndex == 0)
            {
                return (sbyte)(PlayerScore(0) - PlayerScore(1));
            }
            return (sbyte)(PlayerScore(1) - PlayerScore(0));
        }

        public sbyte PlayerScore(byte agentIndex)  //man: +1 , king: +2
        {
            sbyte score = 0;
            int manId = agentIndex == 0 ? 1 : 2;
            int kingId = agentIndex == 0 ? 3 : 4;
            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    if (Matrix[i, j] == manId) 
                        score += 1;
                    if (Matrix[i, j] == kingId) 
                        score += 2;
                }
            }
            return score;
        }
    }
}
