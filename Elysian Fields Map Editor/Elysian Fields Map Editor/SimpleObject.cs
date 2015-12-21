using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields_Map_Editor
{
    class SimpleObject
    {
        public int SpriteID;
        public int RealID;
        public Coordinates Position { get; set; }
        public int Z_order;

        public SimpleObject()
        {
            SpriteID = -1;
        }

        public SimpleObject(int _SpriteID, Coordinates spawnPos, int realid = 0, int zorder = 0)
        {
            SpriteID = _SpriteID;
            Position = spawnPos;
            RealID = realid;
            Z_order = zorder;
        }
    }
}
