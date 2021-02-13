using System;
using System.Collections.Generic;
using System.Linq;

namespace sop11
{
    public class OptimalStrategy
    {
        private readonly List<int> _inputList;
        private readonly int _memorySize;
        private readonly int _maxTimestep;
        public readonly int[][] SimulationTable;
        public readonly bool[] MemFaultTable;

        public OptimalStrategy(List<int> inputList, int memorySize)
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
                        int victimPage = ChooseVictim(timeStepIndex, SimulationTable[timeStepIndex], _inputList);
                        ReplaceVictim(ref SimulationTable[timeStepIndex], victimPage, memoryRef);
                    }
                }
            }
        }

        private int ChooseVictim(int currentTimeStep, int[] memSnapshot, List<int> inputList)
        {
            Dictionary<int, int> pageToLastUsageIndex = new Dictionary<int, int>();

            for (int memoryRefIndex = 0; memoryRefIndex < memSnapshot.Length; memoryRefIndex++)
            {
                int currentMemoryRef = memSnapshot[memoryRefIndex];
                
                //initialize dictionary
                pageToLastUsageIndex.Add(currentMemoryRef, Int32.MaxValue);

                for (int timeStep = currentTimeStep + 1;
                    timeStep < inputList.Count;
                    timeStep++)
                {
                    int futurePage = inputList[timeStep];
                    if (futurePage == currentMemoryRef)
                    {
                        pageToLastUsageIndex[currentMemoryRef] = timeStep;
                    }
                }
            }

            return pageToLastUsageIndex
                .OrderByDescending(y => y.Value)
                .First().Key;
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