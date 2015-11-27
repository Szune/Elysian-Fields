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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Elysian_Fields_Map_Editor
{
    class Entity
    {
        public string Name { get; set; }
        public ConsoleColor Color { get; set; }

        public Coordinates Position { get; set; }

        public int SpriteID { get; set; }

        public int EntityType { get; set; }

        public const int CreatureEntity = 1;
        public const int PlayerEntity = 2;
        public const int TileEntity = 3;
        public const int ItemEntity = 4;
        public const int UnknownEntity = 5;
        public const int TenByTenButton = 6;
        public const int ThirtyTwoByThirtyTwoButton = 7;






        public int ID { get; set; }

        public bool Visible { get; set; }

        public int Experience { get; set; }

        public Entity() { ID = 0; }

        public Entity(string name, int id = 0, bool visible = true) { Name = name; ID = id; Visible = visible; }

        public Entity(string name, Coordinates pos) { Name = name; Position = pos; }

        public Entity(string name, Coordinates pos, bool visible = true)
        {
            Name = name; Position = pos; Visible = visible;
        }

        public bool SamePosition(Coordinates Source, Coordinates Destination)
        {
            return (Source.X == Destination.X && Source.Y == Destination.Y);
        }
    }
}
