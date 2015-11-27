using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields_Map_Editor
{
    class Tile : Entity

    {
        public bool Walkable;
        public int Z_order = 0; // TODO: Add Z-order
        public Coordinates drawPosition;

        public Tile(string name) { Name = name; ID = -1; }
        public Tile(string name, int spriteID, Coordinates pos, Coordinates drawPos, int tileID, bool visible = true, bool walkable = true, int zorder = 0)
        {
            Name = name;
            ID = tileID;
            Position = pos;
            drawPosition = drawPos;
            SpriteID = spriteID;
            Visible = visible;
            Walkable = walkable;
            EntityType = Entity.TileEntity;
            Z_order = zorder;
        }
    }
}
