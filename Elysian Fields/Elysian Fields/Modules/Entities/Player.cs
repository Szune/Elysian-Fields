/*
    Elysian Fields is a 2D game programmed in C# with the framework MonoGame
    Copyright (C) 2015 Erik Iwarson

    If you have any questions, don't hesitate to send me an e-mail at erikiwarson@gmail.com

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

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
