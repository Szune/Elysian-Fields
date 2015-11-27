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

namespace Elysian_Fields_Map_Editor
{
    class Tile : Entity

    {
        public bool Walkable;
        public int Z_order = 0; // TODO: Add Z-order
        public Coordinates drawPosition;

        public Tile(string name) { Name = name; ID = -1; }
        public Tile(string name, int spriteID, Coordinates pos, Coordinates drawPos, int tileID, bool visible = true, bool walkable = true, int zorder = 0)
        {
            Name = name;
            ID = tileID;
            Position = pos;
            drawPosition = drawPos;
            SpriteID = spriteID;
            Visible = visible;
            Walkable = walkable;
            EntityType = Entity.TileEntity;
            Z_order = zorder;
        }
    }
}
