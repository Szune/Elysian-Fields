using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields
{
    class Loot : Item
    {
        public Decimal LootChance;

        public Loot(Item item)
        {
            this.SpriteID = item.SpriteID;
        }
        public Loot(Item item, Decimal _LootChance)
        {
            this.SpriteID = item.SpriteID;
            LootChance = _LootChance;
        }

        public static Loot Parse(Item item)
        {
            return new Loot(item);
        }
    }
}
