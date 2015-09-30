using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields
{
    class Player : Creature
    {

        public Player()
        {

        }
        public Player(string name, Coordinates coordinates, ConsoleColor color = ConsoleColor.White, int health = 1, int id = 0)
        {
            /* Spelare */
            Name = name;
            Position = coordinates;
            SpawnPosition = coordinates;
            Color = color;
            MaxHealth = health;
            Health = health;
            ID = id;
            SuperPowerSteps = 0;
            Visible = true;
            EntityType = Entity.PlayerEntity;
        }
    }
}
