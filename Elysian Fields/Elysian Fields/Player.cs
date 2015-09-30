using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields
{
    class Player : Creature
    {
        public Item LeftHand = new Item();
        public Player()
        {

        }
        public Player(string name, Coordinates coordinates, int health = 1, int id = 0)
        {
            /* Spelare */
            Name = name;
            Position = coordinates;
            SpawnPosition = coordinates;
            MaxHealth = health;
            Health = health;
            ID = id;
            SuperPowerSteps = 0;
            Visible = true;
            EntityType = Entity.PlayerEntity;
            TargetID = -1;
        }
    }
}
