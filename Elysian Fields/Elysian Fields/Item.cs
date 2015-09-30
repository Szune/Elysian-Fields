using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields
{
    class Item : Entity
    {
        public Item(string name, Coordinates pos, ConsoleColor color, int tileID = 0, bool visible = true)
        {
            Name = name;
            ID = tileID;
            Position = pos;
            Color = color;
            Visible = visible;
            EntityType = Entity.ItemEntity;
        }
    }
}
