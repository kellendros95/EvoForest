using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace EvoForest
{
    class Tree
    {
        Dna _dna;
        List<Branch> _branches = new List<Branch>();
        int _mirrored;
        float _energy;
        public Dna GetDna { get => _dna; }
        public Color LeafColor { get => _dna.GetColor; }
        public int Mirrored { get => _mirrored; }
        public Tree(Dna dna, float x, float energy)
        {
            _dna = dna;
            new Branch(this, x, _dna[0]);
            World.AddTree(this);
            _energy = energy;
        }
        public void AddBranch(Branch b)
            => _branches.Add(b); //TODO
    }
}
