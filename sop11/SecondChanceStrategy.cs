using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sop11
{
    public class SecondChanceStrategy
    {
        private readonly List<int> _inputList;
        private readonly int _memorySize;
        private readonly int _maxTimestep;
        public readonly int[][] SimulationTable;
        public readonly bool[] MemFaultTable;
        private readonly bool[] _secondChangeTable;
        public List<bool[]> SecondChangeTableSnapshot;

        public SecondChanceStrategy(List<int> inputList, int memorySize)
        {
            _inputList = inputList;
            _memorySize = memorySize;
            _maxTimestep = _inputList.Count();
            SimulationTable = new int[_maxTimestep][];
            MemFaultTable = new bool[_maxTimestep];
            _secondChangeTable = new bool[_memorySize];
            SecondChangeTableSnapshot = new List<bool[]>();
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
                        int victimPage = ChooseVictim(ref SimulationTable[timeStepIndex]);
                        ReplaceVictim(ref SimulationTable[timeStepIndex], victimPage, memoryRef);
                    }
                }
                SecondChangeTableSnapshot.Add((bool[])_secondChangeTable.Clone());
            }
        }

        private int ChooseVictim(ref int[] memSnapshot)
        {
            for (int i = 0; i < memSnapshot.Length; i++)
            {
                if (_secondChangeTable[i] == true)
                {
                    _secondChangeTable[i] = false;
                }
                else
                {
                    return memSnapshot[i];
                }
            }

            return memSnapshot.Length;
        }

        private void ReplaceVictim(ref int[] memSnapshot, int victimPage, int replaceWithPage)
        {
            for (int i = 0; i < _memorySize; i++)
            {
                int currentPage = memSnapshot[i];
                if (currentPage == victimPage)
                {
                    memSnapshot[i] = replaceWithPage;
                    _secondChangeTable[i] = true;
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
                    _secondChangeTable[i] = true;
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
                    _secondChangeTable[memorySizeIndex] = true;
                    return true;
                }
            }

            return false;
        }
    }
}
