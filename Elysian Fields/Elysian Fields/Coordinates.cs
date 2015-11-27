/*
    Elysian Fields is a 2D game programmed in C# with the framework MonoGame
    Copyright (C) 2015 Erik Iwarson

    If you have any questions, don't hesitate to send me an e-mail at erikiwarson@gmail.com

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

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
