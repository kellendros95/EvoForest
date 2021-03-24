using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;
using SFML.Window;

namespace EvoForest
{
    static class World
    {
        static Random rnd = new Random();
        static List<Tree> _trees = new List<Tree>();
        static List<Branch>[] _branches = new List<Branch>[Settings.MaxX];
        static List<Leaf>[] _leaves = new List<Leaf>[Settings.MaxX];
        static public void AddBranch(Branch b)
            => _branches[(int)b.Root.X].Add(b);
        static public void AddLeaf(Leaf leaf)
            => _leaves[(int)leaf.Center.X].Add(leaf);
        static public void RemoveLeaf(Leaf leaf)
            => _leaves[(int)leaf.Center.X].Remove(leaf);
        static public void AddTree(Tree t)
            => _trees.Add(t);
    }
}
