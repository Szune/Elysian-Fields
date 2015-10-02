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
