using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace EvoForest
{
    abstract class BranchAction
    {
        protected static Random rnd = new Random();
        protected Branch _branch;
        public abstract float Cost { get; }
        public abstract bool Valid { get; }
        public abstract void Execute();
    }
    class DropLeaves : BranchAction
    {
        public DropLeaves(Branch branch)
        {
            _branch = branch;
        }
        public override float Cost { get => 0.0f; }
        public override bool Valid { get => true; }
        public override void Execute()
            => _branch.DropLeaves();
    }
    class GrowLeaf : BranchAction
    {
        float _angle, _radius;
        Vector2f _center;
        public GrowLeaf(Branch branch, float param1, float param2)
        {
            _branch = branch;
            _angle = branch.Angle + (param1 - 0.5f) * (float)Math.PI * branch.GetTree.Mirrored;
            _radius = Settings.MinLeafRadius + (Settings.MaxLeafRadius - Settings.MinLeafRadius) * param2;
            _center = branch.End + new Vector2f((float)Math.Cos(_angle) * _radius, (float)Math.Sin(_angle) * _radius);
        }
        public override float Cost { get => Settings.LeafCost * _radius; }
        public override bool Valid 
        {
            get => World.ValidateLeaf(_branch.GetTree, _center, _radius)
                && _branch.ValidateMomentum(_radius * _radius * Settings.LeafMass, _center);
        }
        public override void Execute()
            => new Leaf(_branch, _center, _radius);
    }
    class GrowBranch : BranchAction
    {
        float _angle, _length;
        Vector2f _root, _end;
        int _geneNumber;
        public GrowBranch(Branch branch, float param1, float param2, int geneNumber)
        {
            _branch = branch;
            _angle = branch.Angle + (param1 - 0.5f) * (float)Math.PI * branch.GetTree.Mirrored;
            _length = Settings.MinBranchLength + (Settings.MaxBranchLength - Settings.MinBranchLength) * param2;
            _root = branch.End;
            _end = _root + new Vector2f((float)Math.Cos(_angle) * _length, (float)Math.Sin(_angle) * _length);
            _geneNumber = geneNumber;
        }
        public override float Cost { get => Settings.BranchCost * _length; }
        public override bool Valid 
        {
            get => World.ValidateBranch(_branch, _root, _end)
                && _branch.ValidateMomentum(_length * Settings.BranchMass, (_root + _end) / 2);
        }
        public override void Execute()
            => new Branch(_branch, _root, _end, _angle, _length, _geneNumber);
    }
    class GrowSeed : BranchAction
    {
        float _x, _energy, _cost;
        Dna _dna;
        public GrowSeed(Branch branch, float param1, float param2)
        {
            float _scatter = Settings.MaxSeedScatter * (Settings.BottomY - branch.End.Y) * param1;
            _x = branch.End.X + (float)(rnd.NextDouble() - 0.5) * _scatter;
            while ((_x < 0) || (_x >= Settings.MaxX))
                _x = branch.End.X + (float)(rnd.NextDouble() - 0.5) * _scatter;
            _energy = Settings.MaxSeedEnergy * param2;
            _dna = branch.GetTree.GetDna.Child();
            _cost = _energy / Settings.SeedEnergyEfficiency + param1 * Settings.SeedScatterCost;
        }
        public override float Cost { get => _cost; }
        public override bool Valid { get => true; }
        public override void Execute()
            => new Tree(_dna, _x, _energy);
    }
}
