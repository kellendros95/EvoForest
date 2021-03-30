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
        Color actionColor, activeGeneColor, startGeneColor;
        Vector2f _massCenter;
        float _mass;
        Gene _g;
        RectangleShape _drawRect;
        List<Branch> _branches = new List<Branch>();
        List<Leaf> _leaves = new List<Leaf>();
        public Branch Parent { get; private set; }
        public Tree ParentTree { get; private set; }
        public Dna DNA { get; private set; }
        public float Angle { get; private set; }
        public float Length { get; private set; }
        public float Thickness { get; private set; }
        public Vector2f End { get; private set; }
        public Vector2f Root { get; private set; }
        public bool Active { get => _g != null; }
        public float Loss { get; private set; }
        public bool ValidateMassAndMomentum(float newMass, Vector2f newMassCenter)
        {
            Vector2f sumCenter = (_massCenter * _mass + newMassCenter * newMass) / (_mass + newMass);
            if (_mass + newMass > Settings.MaxMass * Thickness)
                return false;
            if (Math.Abs((sumCenter.X - (Root.X + End.X) / 2) * (_mass + newMass)) > Settings.MaxMomentum * Thickness)
                return false;
            if (Parent == null) return true;
            return Parent.ValidateMassAndMomentum(newMass, newMassCenter);
        }
        public void Thicken(float param)
        {
            Thickness += param;
            AddMass(param * Length * Settings.BranchMass, (Root + End) / 2);
            _drawRect.Size = new Vector2f(Length, Thickness * Settings.ThicknessScale);
            _drawRect.Origin = new Vector2f(Length / 2, Thickness * Settings.ThicknessScale / 2);
            Loss = Length * Settings.BranchLoss * (Thickness / 2 + 0.5f);
        }
        public void AddMass(float newMass, Vector2f newMassCenter)
        {
            _massCenter = (_massCenter * _mass + newMassCenter * newMass) / (_mass + newMass);
            _mass += newMass;
            if (Parent != null) Parent.AddMass(newMass, newMassCenter);
        }
        void _DesignRectangle()
        {
            _drawRect = new RectangleShape(new Vector2f(Length, Thickness * Settings.ThicknessScale));
            _drawRect.Origin = new Vector2f(Length / 2, Thickness * Settings.ThicknessScale / 2);
            _drawRect.Position = (Root + End) / 2;
            _drawRect.Rotation = (float)(180.0 / Math.PI) * Angle;
        }
        public Branch(Branch parent, Vector2f root, Vector2f end, float angle, float length, int geneNumber)
        {
            Parent = parent;
            ParentTree = parent.ParentTree;
            DNA = Parent.DNA.Child();
            _g = DNA[geneNumber];
            Root = root;
            End = end;
            Angle = angle;
            Length = length;
            Thickness = 1.0f;
            Loss = Length * Settings.BranchLoss;
            _mass = Length * Settings.BranchMass;
            _massCenter = (Root + End) / 2;
            parent.AddMass(_mass, _massCenter);
            activeGeneColor = Settings.GeneColor(geneNumber);
            startGeneColor = Settings.GeneColor(geneNumber);
            actionColor = Color.White;
            _DesignRectangle();
            World.AddBranch(this);
            ParentTree.AddBranch(this);
            Parent.AddBranch(this);
        }
        public Branch (Tree tree, float x)
        {
            Parent = null;
            ParentTree = tree;
            DNA = tree.DNA;
            _g = DNA[0];
            Root = new Vector2f(x, Settings.BottomY);
            Length = (Settings.MinBranchLength + Settings.MaxBranchLength) / 2;
            Angle = -(float)Math.PI / 2;
            Thickness = 1.0f;
            Loss = Length * Settings.BranchLoss;
            End = new Vector2f(x, Settings.BottomY - Length);
            _mass = Length * Settings.BranchMass;
            _massCenter = (Root + End) / 2;
            activeGeneColor = Settings.GeneColor(0);
            startGeneColor = Settings.GeneColor(0);
            _DesignRectangle();
            World.AddBranch(this);
            ParentTree.AddBranch(this);
        }
        public void AddBranch(Branch b)
            => _branches.Add(b);
        public void AddLeaf(Leaf leaf)
            => _leaves.Add(leaf);
        public bool Intersect(Vector2f newRoot, Vector2f newEnd)
        {
            Vector2f v0 = End - Root, v1 = newRoot - Root, v2 = newEnd - Root;
            float z01 = (v0.X * v1.Y - v0.Y * v1.X), z02 = (v0.X * v2.Y - v0.Y * v2.X);
            if (z01 * z02 > 0) return false;
            v0 = newEnd - newRoot; v1 = Root - newRoot; v2 = End - newRoot;
            z01 = (v0.X * v1.Y - v0.Y * v1.X); z02 = (v0.X * v2.Y - v0.Y * v2.X);
            if (z01 * z02 > 0) return false;
            return true;
        }
        public void DropLeaves()
        {
            foreach (Leaf leaf in _leaves)
            {
                ParentTree.RemoveLeaf(leaf);
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
                _g = ParentTree.DNA[(int)grow.nextGene];
                activeGeneColor = Settings.GeneColor((int)grow.nextGene);
            }
            (Color, BranchAction) action = grow.growOption switch
            {
                GrowOption.Drop => (Color.Red, new DropLeaves(this)),
                GrowOption.Leaf => (Color.Green, new GrowLeaf(this, grow.param1, grow.param2)),
                GrowOption.Branch => (Color.Blue, new GrowBranch(this, grow.param1, grow.param2, grow.childGene)),
                GrowOption.Seed => (Color.Yellow, new GrowSeed(this, grow.param1, grow.param2)),
                GrowOption.Thicken => (Color.Magenta, new ThickenBranch(this, grow.param1)),
                _ => (Color.White, null)
            };
            actionColor = action.Item1;
            return action.Item2;
        }
        public void Draw(RenderWindow window)
        {
            _drawRect.FillColor = Program.BranchCM switch
            {
                BranchColorMode.Action => actionColor,
                BranchColorMode.ActiveGene => activeGeneColor,
                BranchColorMode.StartGene => startGeneColor,
                BranchColorMode.Thickness => new Color((byte)(Thickness * 25), 25, (byte)(Thickness * 25)),
                BranchColorMode.Mass => new Color((byte)(255 * _mass / (Settings.MaxMass * Thickness)), 50, 50)
            };
            window.Draw(_drawRect);
        }
    }
}
