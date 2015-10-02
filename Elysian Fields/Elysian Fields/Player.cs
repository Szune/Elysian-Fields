using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields
{
    class Player : Creature
    {
        public Equipment EquippedItems;

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
            EquippedItems = new Equipment();
        }

        public int TotalStrength()
        {
            return EquippedItems.TotalStrength() + Experience + 1;
        }

        public void EquipItem(Item item, string _ItemSlot)
        {
            if (_ItemSlot == ItemSlot.LeftHand)
                EquippedItems.LeftHand = item;
            else if (_ItemSlot == ItemSlot.RightHand)
                EquippedItems.RightHand = item;
        }

        public void UnequipItem(string _ItemSlot)
        {
            if (_ItemSlot == ItemSlot.LeftHand)
                EquippedItems.LeftHand = new Item();
            else if (_ItemSlot == ItemSlot.RightHand)
                EquippedItems.RightHand = new Item();
        }
    }
}
