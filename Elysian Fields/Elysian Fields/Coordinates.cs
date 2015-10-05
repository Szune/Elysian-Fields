using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields
{
    class Coordinates
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int H { get; set; } // heuristic used in the AI class
        public int G { get; set; } // keep track of movement cost
        public int F { get; set; } // H + G (total movement cost)

        public const int Step = 32;

        public Coordinates Parent { get; set; }

        public Coordinates() { X = 0; Y = 0; }

        public Coordinates(int x, int y) { X = x; Y = y; G = 0; H = 0; F = 0; }

        public Coordinates(int x, int y, int h) { X = x; Y = y; G = 0; H = h; F = 0 + h; }

        public Coordinates(int x, int y, int h, int g) { X = x; Y = y; H = h; G = g; F = h + g; }

        public Coordinates(int x, int y, int h, int g, Coordinates parent) { X = x; Y = y; H = h; G = g; F = h + g; Parent = parent; }

        public bool hasParent()
        {
            return !object.Equals(Parent, default(Coordinates));
        }

        public override string ToString()
        {
            return "X: " + X.ToString() + " Y:" + Y.ToString();
        }
    }
}
