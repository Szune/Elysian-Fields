using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields_Map_Editor
{
    class Tile : Entity

    {
        public bool Walkable;

        public Tile(string name) { Name = name; ID = -1; }
        public Tile(string name, int spriteID, Coordinates pos, int tileID, bool visible = true, bool walkable = true)
        {
            Name = name;
            ID = tileID;
            Position = pos;
            SpriteID = spriteID;
            Visible = visible;
            Walkable = walkable;
        }
    }
}
