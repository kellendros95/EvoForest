using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoForest
{
    struct GrowInfo
    {
        public GrowOption growOption;
        public float param1;
        public float param2;
        public int childGene;
        public int? nextGene;
    }
}
