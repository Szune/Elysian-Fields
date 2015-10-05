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
        public string Slot = null;
        public string WearSlot;

        public Item()
        {
            ID = -1;
            Strength = 0;
            Defense = 0;
        }
        public Item(string name, string wearslot, Coordinates pos, int spriteid, int id = 0, int strength = 0, int defense = 0, bool visible = true)
        {
            Name = name;
            ID = id;
            Position = pos;
            Visible = visible;
            EntityType = Entity.ItemEntity;
            SpriteID = spriteid;
            Strength = strength;
            Defense = defense;
            WearSlot = wearslot;
        }
    }
}
