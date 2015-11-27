using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Elysian_Fields_Map_Editor
{
    class SpriteObject
    {

        public Texture2D Sprite;
        public int ID;
        public int EntityType;
        public string Name;
        public Coordinates Position { get; set; }

        public SpriteObject()
        {
            ID = -1;
        }
        public SpriteObject(string name)
        {
            ID = -1;
        }
        public SpriteObject(Texture2D sprite, int id, int entitytype, string name, Coordinates ButtonPosition = null)
        {
            Sprite = sprite;
            ID = id;
            EntityType = entitytype;
            Position = ButtonPosition;
            Name = name;
        }
    }
}
