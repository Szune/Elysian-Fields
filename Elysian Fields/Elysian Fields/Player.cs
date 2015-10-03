using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields
{
    class Player : Creature
    {
        public Equipment EquippedItems;
        public int Exhaustion;

        public Player()
        {
            Name = "null";
            ID = -1;
        }
        public Player(string name, Coordinates coordinates, int health = 1, int mana = 1, int id = 0)
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
            MaxMana = mana;
            Mana = mana;
        }

        public int TotalStrength()
        {
            return EquippedItems.TotalStrength() + 1;
        }

        public int TotalDefense()
        {
            return EquippedItems.TotalDefense();
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

        public bool IsExhausted(int CurrentTime)
        {
            if (CurrentTime - Exhaustion > Spell.ExhaustionTime)
            {
                return false;
            }
            return true;
        }

        public bool CastSpell(Spell spell, int CurrentTime)
        {
            if (!IsExhausted(CurrentTime) && Mana >= spell.ManaCost)
            {
                Mana -= spell.ManaCost;
                Exhaustion = CurrentTime;
                return true;
            }
            return false;
        }
    }
}
