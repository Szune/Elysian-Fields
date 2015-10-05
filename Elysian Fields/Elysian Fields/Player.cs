using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields
{
    class Player : Creature
    {
        public Equipment EquippedItems;
        public int Exhaustion { get; set; }
        public int ManaSpent { get; set; }

        public Player()
        {
            Name = "null";
            ID = -1;
        }
        public Player(string name, Coordinates coordinates, int health = 1, int mana = 1, int level = 1, int id = 0)
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
            Level = level;
            MagicStrength = 0;
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
            EquippedItems.EquipItem(item, _ItemSlot);
        }

        public void UnequipItem(string _ItemSlot)
        {
            EquippedItems.UnequipItem(_ItemSlot);
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
                SpendMana(spell.ManaCost);
                Exhaustion = CurrentTime;
                return true;
            }
            return false;
        }

        public void SpendMana(int mana)
        {
            Mana -= mana;
            ManaSpent += mana;
            if(ManaSpent > Utility.ManaSpentNeededForMagicStrength(MagicStrength)) { MagicStrength += 1; }
        }
    }
}
