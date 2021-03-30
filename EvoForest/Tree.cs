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
        static Random rnd = new Random();
        List<Branch> _branches = new List<Branch>();
        List<Leaf> _leaves = new List<Leaf>();
        Queue<Branch> growQueue = new Queue<Branch>();
        BranchAction activeAction;
        float _energy, _loss = Settings.BaseTreeLoss;
        public Dna DNA { get; private set; }
        public Color SpeciesLeafColor { get => DNA.GetColor; }
        public Color EnergyLeafColor 
        {
            get
            {
                byte r = (byte)Math.Min(_energy / _loss, 255);
                byte b = (byte)(64 - r / 4);
                return new Color(r, r, b);
            }
        }
        public int Mirrored { get; private set; }
        public bool IsDead { get; private set; }
        public Tree(Dna dna, float x, float energy)
        {
            DNA = dna;
            World.AddTree(this);
            _energy = energy;
            Mirrored = rnd.Next(2) * 2 - 1;
            new Branch(this, x);
        }
        public void AddBranch(Branch b)
        {
            _branches.Add(b);
            _loss += b.Loss;
            growQueue.Enqueue(b);
        }
        public void RemoveBranch(Branch b)
        {
            _branches.Remove(b);
            _loss -= b.Loss;
        }
        public void AddLeaf(Leaf leaf)
        {
            _leaves.Add(leaf);
            _loss += leaf.Loss;
        }
        public void RemoveLeaf(Leaf leaf)
        {
            _leaves.Remove(leaf);
            _loss -= leaf.Loss;
        }
        public void AddEnergy(float e)
            => _energy += e;
        public void Step()
        {
            _energy -= _loss;
            //_age++;
            if (activeAction == null)
            {
                Branch activeBranch = growQueue.Dequeue();
                activeAction = activeBranch.Grow();
                if (activeBranch.Active) growQueue.Enqueue(activeBranch);
            }
            if (activeAction != null)
            {
                if (!activeAction.Valid) activeAction = null;
                else if (_energy > activeAction.Cost * 2)
                {
                    _energy -= activeAction.Cost;
                    activeAction.Execute();
                    activeAction = null;
                }
            }
            if ((_energy < 0)/* || (_age > Settings.MaxAge)*/ || ((activeAction == null) && (growQueue.Count == 0)))
                IsDead = true;
        }
    }
}
