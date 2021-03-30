using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoForest
{
    class Gene
    {
        static Random rnd = new Random();
        float[] _p = new float[Settings.GeneSize];
        int _dnaInd;
        public Gene(int dnaInd)
        {
            _dnaInd = dnaInd;
            for (int i = 0; i < Settings.GeneSize; i++)
                _p[i] = (float)rnd.NextDouble();
        }
        public Gene(int dnaInd, Gene par)
        {
            _dnaInd = dnaInd;
            _p = (float[])par._p.Clone();
            _p[rnd.Next(Settings.GeneSize)] = (float)rnd.NextDouble();
        }
        public GrowInfo Grow()
        {
            GrowInfo grow = new GrowInfo();
            grow.param1 = _p[0];
            grow.param2 = _p[1];
            float variant = _p[2 + rnd.Next(Settings.GrowVariants)];
            grow.growOption = ((int)(variant / 0.12f)) switch
            {
                0 => GrowOption.Leaf,
                1 => GrowOption.Seed,
                2 => GrowOption.Drop,
                3 => GrowOption.Thicken,
                4 => GrowOption.None,
                _ => GrowOption.Branch
            };
            grow.childGene = (int)((variant - 0.6f) / (0.4f / Settings.DnaLen));
            float next = _p[2 + Settings.GrowVariants + rnd.Next(Settings.NextGeneVariants)];
            if ((_dnaInd == Settings.DnaLen - 1) || (next < 0.2f))
                grow.nextGene = null;
            else 
                grow.nextGene = _dnaInd + 1 + (int)((next - 0.2f) / (0.8f / (Settings.DnaLen - 1 - _dnaInd)));
            /*if (next < 0.5f) grow.nextGene = null;
            else grow.nextGene = (int)((next - 0.5f) / (0.5f / (Settings.DnaLen)));*/
            return grow;
        }
    }
}
