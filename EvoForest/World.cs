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
        static public void RemoveBranch(Branch b)
            => _branches[(int)b.Root.X].Remove(b);
        static public void AddLeaf(Leaf leaf)
            => _leaves[(int)leaf.Center.X].Add(leaf);
        static public void RemoveLeaf(Leaf leaf)
            => _leaves[(int)leaf.Center.X].Remove(leaf);
        static public void AddTree(Tree t)
            => _trees.Add(t);
        static public void RemoveTree(Tree t)
            => _trees.Remove(t);
        static public bool ValidateLeaf(Tree t, Vector2f center, float radius)
        {
            if ((center.X < 0) || (center.X >= Settings.MaxX) || (center.Y + radius > Settings.BottomY))
                return false;
            int ind = (int)center.X;
            for (int i = -1; i <= 1; i++)
                if ((ind + i >= 0) && (ind + i < Settings.MaxX))
                    foreach (Leaf leaf in _leaves[ind + i])
                        if ((t != leaf.GetTree) && leaf.Intersect(center, radius)) 
                            return false;
            return true;
        }
        static public bool ValidateBranch(Branch parent, Vector2f root, Vector2f end)
        {
            if ((end.X < 0) || (end.X >= Settings.MaxX) || (end.Y > Settings.BottomY))
                return false;
            int ind = (int)root.X;
            for (int i = -1; i <= 1; i++)
                if ((ind + i >= 0) && (ind + i < Settings.MaxX))
                    foreach (Branch b in _branches[ind + i])
                        if ((b != parent) && (b.Parent != parent) && b.Intersect(root, end))
                            return false;
            return true;
        }
        static void _SunRay(float x)
        {
            float firstY = Settings.BottomY + 10, secondY = Settings.BottomY + 10;
            Leaf firstLeaf = null, secondLeaf = null;
            int ind = (int)x;
            for (int i = -1; i <= 1; i++)
                if ((ind + i >= 0) && (ind + i < Settings.MaxX))
                    foreach (Leaf leaf in _leaves[ind + i])
                    {
                        float? y = leaf.RayIntersectionY(x);
                        if (y != null)
                        {
                            if ((firstLeaf == null) || (y < firstY))
                            {
                                (secondLeaf, secondY) = (firstLeaf, firstY);
                                (firstLeaf, firstY) = (leaf, (float)y);
                            }
                            else if ((secondLeaf == null) || (y < secondY))
                                (secondLeaf, secondY) = (leaf, (float)y);
                        }
                    }
            firstLeaf?.AddEnergy(Settings.Light * 2);
            secondLeaf?.AddEnergy(Settings.Light);
        }
        static void _CleanDead()
        {
            _trees.RemoveAll(t => t.IsDead);
            foreach (var sublist in _branches)
                for (int j = sublist.Count - 1; j >= 0; j--)
                    if (sublist[j].GetTree.IsDead)
                        sublist.RemoveAt(j);
            foreach (var sublist in _leaves)
                for (int j = sublist.Count - 1; j >= 0; j--)
                    if (sublist[j].GetTree.IsDead)
                        sublist.RemoveAt(j);
        }
        static public void Step()
        {
            for (int i = _trees.Count - 1; i >= 0; i--)
                _trees[i].Step();
            for (int i = 0; i < Settings.MaxX; i++)
                _SunRay((float)(rnd.NextDouble() + i));
            _CleanDead();
            int leafCount = 0;
            foreach (List<Leaf> sublist in _leaves)
                leafCount += sublist.Count;
        }
        static public void DrawAll()
        {
            if (Program.LeafFirst)
                foreach (List<Leaf> sublist in _leaves)
                    foreach (Leaf leaf in sublist) leaf.Draw(Program.window);
            foreach (List<Branch> sublist in _branches)
                foreach (Branch b in sublist) b.Draw(Program.window);
            if (!Program.LeafFirst)
                foreach (List<Leaf> sublist in _leaves)
                    foreach (Leaf leaf in sublist) leaf.Draw(Program.window);
        }
        static public void Init()
        {
            _trees = new List<Tree>();
            _branches = new List<Branch>[Settings.MaxX];
            _leaves = new List<Leaf>[Settings.MaxX];
            for (int i = 0; i < Settings.MaxX; i++)
            {
                _branches[i] = new List<Branch>();
                _leaves[i] = new List<Leaf>();
            }
            for (int i = 0; i < Settings.MaxX - 1; i++) 
                new Tree(new Dna(), 0.5f + i, Settings.InitTreeEnergy);
        }
    }
}
