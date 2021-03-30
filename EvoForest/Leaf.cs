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
        CircleShape _circle;
        int timeAfterRay = 0;
        public Vector2f Center { get; private set; }
        public float Radius { get; private set; }
        public Tree ParentTree { get; private set; }
        public float Loss { get => Radius * Settings.LeafLoss; }
        Color PhotoColor
        {
            get
            {
                byte g = (byte)Math.Max(0, 255 - 50 * timeAfterRay);
                byte r = (byte)(64 - g / 4);
                return new Color(r, g, r);
            }
        }
        public bool Intersect(Vector2f center, float radius)
        {
            float dsqr = (center.X - Center.X) * (center.X - Center.X) + (center.Y - Center.Y) * (center.Y - Center.Y);
            return dsqr < radius + Radius;
        }
        void _DesignCircle()
        {
            _circle = new CircleShape(Radius);
            _circle.Origin = new Vector2f(Radius, Radius);
            _circle.Position = Center;
        }
        public Leaf(Branch branch, Vector2f center, float radius)
        {
            ParentTree = branch.ParentTree;
            Radius = radius;
            Center = center;
            _DesignCircle();
            branch.AddMass(Radius * Radius * Settings.LeafMass, Center);
            World.AddLeaf(this);
            ParentTree.AddLeaf(this);
            branch.AddLeaf(this);
        }
        public float? RayIntersectionY(float x)
        {
            float dx = (float)Math.Abs(Center.X - x);
            if (dx > Radius) return null;
            float dy = (float)Math.Sqrt(Radius * Radius - dx * dx);
            return Center.Y - dy;
        }
        public void AddEnergy(float e)
        {
            ParentTree.AddEnergy(e);
            timeAfterRay = 0;
        }
        public void Draw(RenderWindow window)
        {
            _circle.FillColor = Program.LeafCM switch
            {
                LeafColorMode.Species => ParentTree.SpeciesLeafColor,
                LeafColorMode.Energy => ParentTree.EnergyLeafColor,
                LeafColorMode.Photosythesis => PhotoColor
            };
            window.Draw(_circle);
            timeAfterRay += Program.StepsTillDraw;
        }
    }
}
