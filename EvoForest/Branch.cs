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
    class Branch
    {
        static Random rnd = new Random();
        Tree _tree;
        Branch _parent;
        Vector2f _root, _end;
        float _angle, _length;
        Gene _g;
        Vertex[] _drawLine;
        List<Branch> _branches = new List<Branch>();
        List<Leaf> _leaves = new List<Leaf>();
        public Branch Parent { get => _parent; }
        public Tree GetTree { get => _tree; }
        public float Angle { get => _angle; }
        public Vector2f End { get => _end; }
        public Vector2f Root { get => _root; }
        public bool Active { get => _g != null; }
        public float Loss { get => _length * Settings.BranchLoss; }
        public Branch(Branch parent, Vector2f root, Vector2f end, float angle, float length, Gene g)
        {
            _parent = parent;
            _tree = parent.GetTree;
            _root = root;
            _end = end;
            _angle = angle;
            _length = length;
            _g = g;
            _drawLine = new Vertex[] { new Vertex(_root, Color.White), new Vertex(_end, Color.White) };
            World.AddBranch(this);
            _tree.AddBranch(this);
            _parent.AddBranch(this);
        }
        public Branch (Tree tree, float x, Gene g)
        {
            _parent = null;
            _tree = tree;
            _root = new Vector2f(x, Settings.BottomY);
            _length = (Settings.MinBranchLength + Settings.MaxBranchLength) / 2;
            _angle = -(float)Math.PI / 2;
            _end = new Vector2f(x, Settings.BottomY - _length);
            _g = g;
            _drawLine = new Vertex[] { new Vertex(_root, Color.White), new Vertex(_end, Color.White) };
            World.AddBranch(this);
            _tree.AddBranch(this);
        }
        public void AddBranch(Branch b)
            => _branches.Add(b);
        public void AddLeaf(Leaf leaf)
            => _leaves.Add(leaf);
        public bool Intersect(Vector2f newRoot, Vector2f newEnd)
        {
            Vector2f v0 = _end - _root, v1 = newRoot - _root, v2 = newEnd - _root;
            float z01 = (v0.X * v1.Y - v0.Y * v1.X), z02 = (v0.X * v2.Y - v0.Y * v2.X);
            if (z01 * z02 > 0) return false;
            v0 = newEnd - newRoot; v1 = _root - newRoot; v2 = _end - newRoot;
            z01 = (v0.X * v1.Y - v0.Y * v1.X); z02 = (v0.X * v2.Y - v0.Y * v2.X);
            if (z01 * z02 > 0) return false;
            return true;
        }
        public void DropLeaves()
        {
            foreach (Leaf leaf in _leaves)
            {
                _tree.RemoveLeaf(leaf);
                World.RemoveLeaf(leaf);
            }
            _leaves = new List<Leaf>();
        }
        public BranchAction Grow()
        {
            GrowInfo grow = _g.Grow();
            _g = (grow.nextGene != null) ? _tree.GetDna[(int)grow.nextGene] : null;
            switch (grow.growOption)
            {
                case GrowOption.Drop: _SetColor(Color.Red); return new DropLeaves(this);
                case GrowOption.Leaf: _SetColor(Color.Green); return new GrowLeaf(this, grow.param1, grow.param2);
                case GrowOption.Branch: _SetColor(Color.Blue); return new GrowBranch(this, grow.param1, grow.param2, _tree.GetDna[grow.childGene]);
                case GrowOption.Seed: _SetColor(Color.Yellow); return new GrowSeed(this, grow.param1, grow.param2);
                default: throw new KeyNotFoundException();
            };
        }
        void _SetColor(Color c)
        {
            _drawLine[0].Color = c;
            _drawLine[1].Color = c;
        }
        public void Draw(RenderWindow window)
            => window.Draw(_drawLine, PrimitiveType.Lines);
    }
}
