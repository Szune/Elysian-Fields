using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Elysian_Fields
{
    class Spell
    {
        public int Damage;
        public bool[] Area;
        public Texture2D Sprite;
        public int ID;

        public Spell()
        {
            ID = -1;
        }

        public Spell(bool[] area, int damage, Texture2D sprite, int id = 0)
        {
            Area = area;
            Damage = damage;
            Sprite = sprite;
            ID = id;
        }
    }
}
