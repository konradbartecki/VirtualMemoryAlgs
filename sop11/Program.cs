using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sop11
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter numbers:");

            string input = "1, 2, 3, 4, 2, 1, 5, 6, 2, 1, 2, 3, 7, 6, 3, 2, 1, 2, 3, 6";
            List<int> memoryReference = input
                .Split(',')
                .Select(x => x.Trim())
                .Select(x => Convert.ToInt32(x))
                .ToList();

            if (memoryReference.Any(x => x == 0))
                throw new ArgumentOutOfRangeException("Numbers should not have 0");

            var lru4 = new LastRecentlyUsedStrategy(memoryReference, 4);
            var lru6 = new LastRecentlyUsedStrategy(memoryReference, 6);
            var opt4 = new OptimalStrategy(memoryReference, 4);
            var opt6 = new OptimalStrategy(memoryReference, 6);
            var fifo4 = new FirstInFirstOutStrategy(memoryReference, 4);
            var fifo6 = new FirstInFirstOutStrategy(memoryReference, 6);
            var sc4 = new SecondChanceStrategy(memoryReference, 4);
            var sc6 = new SecondChanceStrategy(memoryReference, 6);


            var memRef = memoryReference.ToArray();

            var lru4csv = ToCsv(lru4.SimulationTable, memRef, lru4.MemFaultTable);
            var lru6csv = ToCsv(lru6.SimulationTable, memRef, lru6.MemFaultTable);
            var opt4csv = ToCsv(opt4.SimulationTable, memRef, opt4.MemFaultTable);
            var opt6csv = ToCsv(opt6.SimulationTable, memRef, opt6.MemFaultTable);
            var fifo4csv = ToCsv(fifo4.SimulationTable, memRef, fifo4.MemFaultTable);
            var fifo6csv = ToCsv(fifo6.SimulationTable, memRef, fifo6.MemFaultTable);
            var sc4csv = ToCsv(sc4.SimulationTable, memRef, sc4.MemFaultTable, sc4.SecondChangeTableSnapshot);
            var sc6csv = ToCsv(sc6.SimulationTable, memRef, sc6.MemFaultTable, sc6.SecondChangeTableSnapshot);
        }

        private static string ToCsv(int[][] sourceTable, int[] memoryReference, bool[] memFaultTable, List<bool[]> secondChangeTable = null)
        {
            var stringBuilder = new StringBuilder();
            string separator = ",";
            int memorySize = sourceTable[0].Length;
            for (int y = 0; y < memorySize + 3; y++)
            {
                for (int x = 0; x < sourceTable.GetUpperBound(0) + 1; x++)
                {
                    if (y == 0)
                    {
                        //timestep
                        stringBuilder.Append(x + 1);
                    }
                    else if (y == 1)
                    {
                        //memory reference
                        stringBuilder.Append(memoryReference[x]);
                    }
                    else if (y == memorySize + 2)
                    {
                        //last column mem fault
                        if (memFaultTable[x])
                        {
                            stringBuilder.Append("x");
                        }
                    }
                    else
                    {
                        int numToPrint = sourceTable[x][y - 2];
                        if (numToPrint != 0)
                        {
                            stringBuilder.Append(numToPrint);
                            if (secondChangeTable != null)
                            {
                                int sc = Convert.ToInt32(secondChangeTable[x][y - 2]);
                                stringBuilder.Append($" ({sc})");
                            }
                        }
                    }
                    stringBuilder.Append(separator);
                }

                stringBuilder.Append(Environment.NewLine);
            }

            return stringBuilder.ToString();
        }

        
    }
}
