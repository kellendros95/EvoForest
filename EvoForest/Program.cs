using System;
using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace EvoForest
{
    class Program
    {
        const float MinFrameTime = 1.0f / 60.0f;
        static Clock _frameClock = new Clock();
        static float _frameTime, _resX = 1000, _resY = 600, _centerX = 8, _centerY = 5, _zoom = 50.0f;
        public static RenderWindow window = new RenderWindow(new VideoMode((uint)_resX, (uint)_resY), "Forest");
        public static bool Pause { get; private set; }
        static public LeafColorMode LCM { get; private set; }
        static public BranchColorMode BCM { get; private set; }
        static public bool LeafFirst { get; private set; }
        static void Main(string[] args)
        {
            Settings.InitColors();
            _frameTime = _frameClock.Restart().AsSeconds();
            window.SetVerticalSyncEnabled(true);
            window.KeyPressed += Controls;
            window.SetView(new View(new Vector2f(_centerX, _centerY), new Vector2f(_resX / _zoom, _resY / _zoom)));
            World.Init();
            LCM = LeafColorMode.Species;
            BCM = BranchColorMode.Action;
            LeafFirst = false;
            Pause = false;
            Vertex[] leftLine = new Vertex[] { new Vertex(new Vector2f(0, -500), Color.White), new Vertex(new Vector2f(0, 500), Color.White) };
            Vertex[] rightLine = new Vertex[] { new Vertex(new Vector2f(Settings.MaxX, -500), Color.White), new Vertex(new Vector2f(Settings.MaxX, 500), Color.White) };
            Vertex[] bottomLine = new Vertex[] { new Vertex(new Vector2f(0, Settings.BottomY), Color.White), new Vertex(new Vector2f(Settings.MaxX, Settings.BottomY), Color.White) };
            while (window.IsOpen)
            {
                window.Clear();
                if (!Pause)
                    while (_frameClock.ElapsedTime.AsSeconds() < MinFrameTime)
                        World.Step();
                World.DrawAll();
                window.Draw(leftLine, PrimitiveType.Lines);
                window.Draw(rightLine, PrimitiveType.Lines);
                window.Draw(bottomLine, PrimitiveType.Lines);
                window.Display();
                window.DispatchEvents();
                _frameTime = _frameClock.Restart().AsSeconds();
                Console.WriteLine("FPS = {0}", 1.0f / _frameTime);
            }
        }
        static void Controls(Object sender, EventArgs e)
        {
            const float speed = 1000;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape)) window.Close();
            if (Keyboard.IsKeyPressed(Keyboard.Key.Left)) _centerX -= _frameTime * speed / _zoom;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Right)) _centerX += _frameTime * speed / _zoom;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Up)) _centerY -= _frameTime * speed / _zoom;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Down)) _centerY += _frameTime * speed / _zoom;
            if (Keyboard.IsKeyPressed(Keyboard.Key.PageDown)) _zoom *= 9.0f / 10.0f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.PageUp)) _zoom *= 10.0f / 9.0f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Enter)) World.Init();
            if (Keyboard.IsKeyPressed(Keyboard.Key.P)) Pause = !Pause;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Num1)) LCM = LeafColorMode.Species;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Num2)) LCM = LeafColorMode.Energy;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Num3)) LCM = LeafColorMode.Photosythesis;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Q)) BCM = BranchColorMode.Action;
            if (Keyboard.IsKeyPressed(Keyboard.Key.W)) BCM = BranchColorMode.ActiveGene;
            if (Keyboard.IsKeyPressed(Keyboard.Key.E)) BCM = BranchColorMode.StartGene;
            if (Keyboard.IsKeyPressed(Keyboard.Key.L)) LeafFirst = false;
            if (Keyboard.IsKeyPressed(Keyboard.Key.B)) LeafFirst = true;
            window.SetView(new View(new Vector2f(_centerX, _centerY), new Vector2f(_resX / _zoom, _resY / _zoom)));
        }
    }
}
