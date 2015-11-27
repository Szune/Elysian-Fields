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

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields_Map_Editor.Modules.Controls
{
    class Textbox : Controls
    {
        public string Text;
        public int MaxChars;
        public int Start_SpriteID;
        public int Middle_SpriteID;
        public int End_SpriteID;

        public Textbox()
        {
            Position = null;
            ID = -1;
        }
        public Textbox(string name)
        {
            Position = null;
            ID = -1;
            Name = name;
        }
        public Textbox(int id, string name, int boxStart_SpriteID, int boxMiddle_SpriteID, int boxEnd_SpriteID, string text = "", int size = 30, Coordinates pos = null, int maxChars = 7)
        {
            ID = id;
            Name = name;
            Text = text;
            Width = size;
            Position = pos;
            Start_SpriteID = boxStart_SpriteID;
            Middle_SpriteID = boxMiddle_SpriteID;
            End_SpriteID = boxEnd_SpriteID;
            MaxChars = maxChars;
        }
    }
}
