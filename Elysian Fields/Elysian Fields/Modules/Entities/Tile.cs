using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields
{
    class Tile : Entity

    {
        public bool Walkable;
        public int Z_order = 0; // TODO: Add Z-order
        public Coordinates drawPosition;
        public bool MovePlayer;
        public Coordinates RelativeMovePosition;

        public Tile(string name) { Name = name; }
        public Tile(string name, int spriteID, Coordinates pos, Coordinates drawPos, int tileID, bool visible = true, bool walkable = true, int zorder = 0, bool movePlayer = false, Coordinates movePos = null)
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
            MovePlayer = movePlayer;
            RelativeMovePosition = movePos;
        }
    }
}
