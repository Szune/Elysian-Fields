using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Elysian_Fields
{
    class Entity
    {
        public string Name { get; set; }
        public ConsoleColor Color { get; set; }

        public Coordinates Position { get; set; }

        public int SpriteID { get; set; }

        public int EntityType { get; set; }

        public const int CreatureEntity = 1;
        public const int PlayerEntity = 2;
        public const int TileEntity = 3;
        public const int ItemEntity = 4;
        public const int UnknownEntity = 5;
        public const int SpellEntity = 6;

        




        public int ID { get; set; }

        public bool Visible { get; set; }

        public int Experience { get; set; }

        public Entity() { ID = 0; }

        public Entity(string name, int id = 0, bool visible = true) { Name = name; ID = id; Visible = visible; }

        public Entity(string name, Coordinates pos) { Name = name; Position = pos; }

        public Entity(string name, Coordinates pos, bool visible = true)
        {
            Name = name; Position = pos; Visible = visible;
        }

        public bool SamePosition(Coordinates Source, Coordinates Destination)
        {
            return (Source.X == Destination.X && Source.Y == Destination.Y);
        }
    }
}
