using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Window;
using SFML.Graphics;

namespace EvoForest
{
    class Leaf
    {
        Tree _tree;
        Vector2f _center;
        float _radius;
        CircleShape _circle;
        public Vector2f Center { get => _center; }
        public Tree GetTree { get => _tree; }
        public float Loss { get => _radius * Settings.LeafLoss; }
        public bool Intersect(Vector2f center, float radius)
        {
            float dsqr = (center.X - _center.X) * (center.X - _center.X) + (center.Y - _center.Y) * (center.Y - _center.Y);
            return dsqr < radius + _radius;
        }
        public Leaf(Branch branch, Vector2f center, float radius)
        {
            _tree = branch.GetTree;
            _radius = radius;
            _center = center;
            _circle = new CircleShape(_radius);
            _circle.Position = (_center - new Vector2f(_radius, _radius));
            _circle.FillColor = _tree.LeafColor;
            branch.AddMass(_radius * _radius * Settings.LeafMass, _center);
            World.AddLeaf(this);
            _tree.AddLeaf(this);
            branch.AddLeaf(this);
        }
        public float? RayIntersectionY(float x)
        {
            float dx = (float)Math.Abs(_center.X - x);
            if (dx > _radius) return null;
            float dy = (float)Math.Sqrt(_radius * _radius - dx * dx);
            return _center.Y - dy;
        }
        public void Draw(RenderWindow window)
            => window.Draw(_circle);
    }
}
