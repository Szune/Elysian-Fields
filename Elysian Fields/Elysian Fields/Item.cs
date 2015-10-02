using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields
{
    class Item : Entity
    {

        public int Strength;
        public int Defense;

        public Item()
        {
            ID = -1;
            Strength = 0;
            Defense = 0;
        }
        public Item(string name, Coordinates pos, int spriteid, int tileID = 0, int strength = 0, int defense = 0, bool visible = true)
        {
            Name = name;
            ID = tileID;
            Position = pos;
            Visible = visible;
            EntityType = Entity.ItemEntity;
            SpriteID = spriteid;
            Strength = strength;
            Defense = defense;
        }
    }
}
