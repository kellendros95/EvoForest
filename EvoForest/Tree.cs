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
        static List<Branch> _branches = new List<Branch>();
        static List<Leaf> _leaves = new List<Leaf>();
        public Color LeafColor { get => _dna.GetColor; }
        public void AddBranch(Branch b)
            => _branches.Add(b);
        public void AddLeaf(Leaf leaf)
            => _leaves.Add(leaf);
    }
}
