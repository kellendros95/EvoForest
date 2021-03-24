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
        public float Mass
        {
            get => _radius * _radius * Settings.LeafMass;
        }
        public Vector2f Center
        {
            get => _center;
        }
        public Leaf(Tree tree, Branch branch, Vector2f growPoint, float angle, float radius)
        {
            _tree = tree;
            _radius = radius;
            _center = growPoint + new Vector2f((float)Math.Cos(angle), (float)Math.Sin(angle));
            _circle = new CircleShape(_radius);
            _circle.Position = (_center - new Vector2f(_radius, _radius));
            _circle.FillColor = _tree.LeafColor;
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
