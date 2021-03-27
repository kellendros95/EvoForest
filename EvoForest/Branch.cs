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
        Color actionColor, activeGeneColor, startGeneColor;
        Vector2f _root, _end, _massCenter;
        float _angle, _length, _mass;
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
        public bool ValidateMomentum(float newMass, Vector2f newMassCenter)
        {
            Vector2f sumCenter = (_massCenter * _mass + newMassCenter * newMass) / (_mass + newMass);
            if (Math.Abs(sumCenter.X - (_root.X + _end.X) / 2) * (_mass + newMass) > Settings.MaxMomentum)
                return false;
            if (_parent == null) return true;
            return _parent.ValidateMomentum(newMass, newMassCenter);
        }
        public void AddMass(float newMass, Vector2f newMassCenter)
        {
            _massCenter = (_massCenter * _mass + newMassCenter * newMass) / (_mass + newMass);
            _mass += newMass;
            if (_parent != null) _parent.AddMass(newMass, newMassCenter);
        }
        public Branch(Branch parent, Vector2f root, Vector2f end, float angle, float length, int geneNumber)
        {
            _parent = parent;
            _tree = parent.GetTree;
            _root = root;
            _end = end;
            _angle = angle;
            _length = length;
            _mass = _length * Settings.BranchMass;
            _massCenter = (_root + _end) / 2;
            parent.AddMass(_mass, _massCenter);
            _g = _tree.GetDna[geneNumber];
            activeGeneColor = Settings.GeneColor(geneNumber);
            startGeneColor = Settings.GeneColor(geneNumber);
            actionColor = Color.White;
            _drawLine = new Vertex[] { new Vertex(_root, actionColor), new Vertex(_end, actionColor) };
            World.AddBranch(this);
            _tree.AddBranch(this);
            _parent.AddBranch(this);
        }
        public Branch (Tree tree, float x)
        {
            _parent = null;
            _tree = tree;
            _root = new Vector2f(x, Settings.BottomY);
            _length = (Settings.MinBranchLength + Settings.MaxBranchLength) / 2;
            _angle = -(float)Math.PI / 2;
            _end = new Vector2f(x, Settings.BottomY - _length);
            _mass = _length * Settings.BranchMass;
            _massCenter = (_root + _end) / 2;
            _g = tree.GetDna[0];
            activeGeneColor = Settings.GeneColor(0);
            startGeneColor = Settings.GeneColor(0);
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
            if (grow.nextGene == null)
            {
                _g = null;
                activeGeneColor = Color.White;
            }
            else
            {
                _g = _tree.GetDna[(int)grow.nextGene];
                activeGeneColor = Settings.GeneColor((int)grow.nextGene);
            }
            switch (grow.growOption)
            {
                case GrowOption.Drop: actionColor = Color.Red; return new DropLeaves(this);
                case GrowOption.Leaf: actionColor = Color.Green; return new GrowLeaf(this, grow.param1, grow.param2);
                case GrowOption.Branch: actionColor = Color.Blue; return new GrowBranch(this, grow.param1, grow.param2, grow.childGene);
                case GrowOption.Seed: actionColor = Color.Yellow; return new GrowSeed(this, grow.param1, grow.param2);
                default: actionColor = Color.White; return null;
            };
        }
        void _SetColor(Color c)
        {
            _drawLine[0].Color = c;
            _drawLine[1].Color = c;
        }
        public void Draw(RenderWindow window)
        {
            _SetColor(Program.BCM switch
            {
                BranchColorMode.Action => actionColor,
                BranchColorMode.ActiveGene => activeGeneColor,
                BranchColorMode.StartGene => startGeneColor
            });
            window.Draw(_drawLine, PrimitiveType.Lines);
        }
    }
}
