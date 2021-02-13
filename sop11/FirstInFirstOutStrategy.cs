using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sop11
{
    public class FirstInFirstOutStrategy
    {
        private readonly List<int> _inputList;
        private readonly int _memorySize;
        private readonly int _maxTimestep;
        public readonly int[][] SimulationTable;
        public readonly bool[] MemFaultTable;

        public FirstInFirstOutStrategy(List<int> inputList, int memorySize)
        {
            _inputList = inputList;
            _memorySize = memorySize;
            _maxTimestep = _inputList.Count();
            SimulationTable = new int[_maxTimestep][];
            MemFaultTable = new bool[_maxTimestep];
            for (int timeStepIndex = 0; timeStepIndex < _inputList.Count; timeStepIndex++)
            {
                SimulationTable[timeStepIndex] = new int[_memorySize];
            }

            Simulate();
        }

        public void Simulate()
        {
            Queue<int> pagesQueue = new Queue<int>(_memorySize);
            for (int timeStepIndex = 0; timeStepIndex < _inputList.Count; timeStepIndex++)
            {
                if (timeStepIndex > 0)
                {
                    //copy last page to current timestep
                    SimulationTable[timeStepIndex - 1].CopyTo(SimulationTable[timeStepIndex], 0);
                }

                int memoryRef = _inputList[timeStepIndex];

                if (!IsMemoryReferenceAvailable(SimulationTable[timeStepIndex], memoryRef))
                {
                    MemFaultTable[timeStepIndex] = true;
                    bool shouldChooseVictim = TryAllocateMemory(memoryRef, ref SimulationTable[timeStepIndex])
                                              == false;
                    if (shouldChooseVictim)
                    {
                        int victimPage = ChooseVictim(pagesQueue);
                        ReplaceVictim(ref SimulationTable[timeStepIndex], victimPage, memoryRef);
                    }

                    pagesQueue.Enqueue(memoryRef);
                }
            }
        }

        private int ChooseVictim(Queue<int> pages)
        {
            return pages.Dequeue();
        }

        private void ReplaceVictim(ref int[] memSnapshot, int victimPage, int replaceWithPage)
        {
            for (int i = 0; i < _memorySize; i++)
            {
                int currentPage = memSnapshot[i];
                if (currentPage == victimPage)
                {
                    memSnapshot[i] = replaceWithPage;
                    return;
                }
            }
        }

        private bool IsMemoryReferenceAvailable(int[] memSnapshot, int requestedPage)
        {
            for (int i = 0; i < _memorySize; i++)
            {
                int currentPage = memSnapshot[i];
                if (currentPage == requestedPage)
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryAllocateMemory(int memRef, ref int[] memSnapshot)
        {
            for (int memorySizeIndex = 0; memorySizeIndex < _memorySize; memorySizeIndex++)
            {
                if (memSnapshot[memorySizeIndex] == default(int))
                {
                    memSnapshot[memorySizeIndex] = memRef;
                    return true;
                }
            }

            return false;
        }
    }
}
