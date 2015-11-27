using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields.Modules.Controls
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
