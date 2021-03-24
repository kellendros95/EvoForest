using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace EvoForest
{
    class Dna
    {
        static Random rnd = new Random();
        Gene[] _g = new Gene[Settings.DnaLen];
        Color _color;
        public Color GetColor { get => _color; }
        public Dna()
        {
            for (int i = 0; i < Settings.DnaLen; i++)
                _g[i] = new Gene(i);
            _color = new Color((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256));
        }
        private Dna(Dna p)
        {
            int a = rnd.Next(Settings.DnaLen);
            for (int i = 0; i < Settings.DnaLen; i++)
                if (i != a) _g[i] = p._g[i];
                else _g[i] = new Gene(i);
            _color = new Color((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256));
        }
        public Dna Child()
        {
            if (rnd.NextDouble() > Settings.MutationChance)
                return this;
            return new Dna(this);
        }
        public Gene this[int i] { get => _g[i]; }
    }
}
