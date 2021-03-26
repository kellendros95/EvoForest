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
        static public bool ValidateLeaf(Tree t, Vector2f center, float radius)
        {
            int ind = (int)center.X;
            for (int i = -1; i <= 1; i++)
                if ((ind + i >= 0) && (ind + i < Settings.MaxX))
                    foreach (Leaf leaf in _leaves[ind + i])
                        if ((t != leaf.GetTree) && leaf.Intersect(center, radius)) 
                            return false;
            return true;
        }
        static public bool ValidateBranch(Vector2f root, Vector2f end)
        {
            int ind = (int)root.X;
            for (int i = -1; i <= 1; i++)
                if ((ind + i >= 0) && (ind + i < Settings.MaxX))
                    foreach (Branch b in _branches[ind + i])
                        if (b.Intersect(root, end))
                            return false;
            return true;
        }
    }
}
