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
using Microsoft.Xna.Framework.Graphics;

namespace Elysian_Fields
{
    class UI : SpriteObject
    {
        public Coordinates Position { get; set; }
        public string Name { get; set; }

        public int SpellID;

        public UI()
        {
            ID = -1;
        }
        public UI(Texture2D sprite, int id, int entitytype, Coordinates pos, string name = "null", int _SpellID = 0)
        {
            Sprite = sprite;
            ID = id;
            EntityType = entitytype;
            Position = pos;
            Name = name;
            SpellID = _SpellID;
        }
    }
}
