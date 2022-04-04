using System;

namespace Serialize
{
    [Serializable]
    public class Output
    {
        public decimal SumResult { get; set; }
        public int MulResul { get; set; }
        public decimal[] SortedInputs { get; set; }
    }
}
