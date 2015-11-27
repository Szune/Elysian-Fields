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
    class SpriteObject
    {

        public Texture2D Sprite;
        public int ID;
        public int EntityType;
        public string SpriteName;
        public bool Walkable = true;
        public bool MovePlayer;
        public Coordinates RelativeMovePosition;

        public SpriteObject()
        {

        }
        public SpriteObject(Texture2D sprite, int id, int entitytype, string name, bool walkthrough = true, bool _MovePlayer = false, Coordinates _MovePos = null)
        {
            Sprite = sprite;
            ID = id;
            EntityType = entitytype;
            SpriteName = name;
            Walkable = walkthrough;
            MovePlayer = _MovePlayer;
            RelativeMovePosition = _MovePos;
        }
    }
}
