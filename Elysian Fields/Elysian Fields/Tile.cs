using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields
{
    class Tile : Entity

    {
        public bool Walkable;

        public Tile(string name) { Name = name; }
        public Tile(string name, int spriteID, Coordinates pos, int tileID, bool visible = true, bool walkable = true)
        {
            Name = name;
            ID = tileID;
            Position = pos;
            SpriteID = spriteID;
            Visible = visible;
            Walkable = walkable;
        }
        public Tile(string name, Coordinates pos, ConsoleColor color, int tileID = 0, bool visible = true)
        {
            Name = name;
            ID = tileID;
            Position = pos;
            Color = color;
            Visible = true;
            EntityType = Entity.TileEntity;
        }
    }
}
