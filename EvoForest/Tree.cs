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
        Dna _dna;
        List<Branch> _branches = new List<Branch>();
        List<Leaf> _leaves = new List<Leaf>();
        Queue<Branch> growQueue = new Queue<Branch>();
        BranchAction activeAction;
        int _mirrored, _age = 0;
        float _energy, _loss = Settings.BaseTreeLoss;
        bool _dead = false;
        public Dna GetDna { get => _dna; }
        public Color LeafColor { get => _dna.GetColor; }
        /*public Color LeafColor { 
            get
            {
                byte c = (byte)Math.Min(_energy, 255);
                return new Color(c, c, c);
            }
        }*/
        public int Mirrored { get => _mirrored; }
        public bool IsDead { get => _dead; }
        public Tree(Dna dna, float x, float energy)
        {
            _dna = dna;
            World.AddTree(this);
            _energy = energy;
            _mirrored = rnd.Next(2) * 2 - 1;
            new Branch(this, x, _dna[0]);
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
            _age++;
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
            if ((_energy < 0) || (_age > Settings.MaxAge) || ((activeAction == null) && (growQueue.Count == 0))) _dead = true;
        }
    }
}
