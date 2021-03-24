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
        static List<Branch> _branches = new List<Branch>();
        static List<Leaf> _leaves = new List<Leaf>();
        public Branch Parent { get => _parent; }
        public Branch(Branch parent, Tree tree, Vector2f root, float angle, float length, Gene g)
        {
            _tree = tree;
            _root = root;
            _angle = angle;
            _length = length;
            _parent = parent;
            _g = g;
            _end = new Vector2f(
                _root.X + (float)Math.Cos(_angle) * _length,
                _root.Y + (float)Math.Sin(_angle) * _length);
            _g = g;
            _SetColor(Color.White);
            World.AddBranch(this);
            _tree.AddBranch(this);
            if (_parent != null) _parent.AddBranch(this);
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
        public bool Grow() // Возвращает, надо ли ветку вернуть в очередь роста
        {
            GrowInfo grow = _g.Grow();
            // ОСТАНОВИЛСЯ ТУТ
        }
        void _SetColor(Color c)
        {
            _drawLine[0].Color = c;
            _drawLine[1].Color = c;
        }
    }
}
