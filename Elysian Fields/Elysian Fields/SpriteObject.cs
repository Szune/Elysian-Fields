﻿using System;
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

        public SpriteObject()
        {

        }
        public SpriteObject(Texture2D sprite, int id, int entitytype, string name)
        {
            Sprite = sprite;
            ID = id;
            EntityType = entitytype;
            SpriteName = name;
        }
    }
}
