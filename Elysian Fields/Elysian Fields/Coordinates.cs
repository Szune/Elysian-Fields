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
        public int Z { get; set; }

        public const int Step = 32;

        public Coordinates Parent { get; set; }

        public Coordinates() { X = 0; Y = 0; Z = 0; }
        public Coordinates(Elysian_Fields.Modules.AI.Node node)
        {
            X = node.X;
            Y = node.Y;
            Z = node.Z;
        }

        public Coordinates(int x, int y) { X = x; Y = y; }

        public Coordinates(int x, int y, int z) { X = x; Y = y; Z = z; }

        public override string ToString()
        {
            return "X: " + X.ToString() + " Y: " + Y.ToString() + " Z: " + Z.ToString();
        }

        public static Coordinates Parse(Elysian_Fields.Modules.AI.Node node)
        {
            return new Coordinates(node);
        }
    }
}
