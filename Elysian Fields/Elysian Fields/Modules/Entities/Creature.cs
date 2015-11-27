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
using Elysian_Fields.Modules.AI;

namespace Elysian_Fields
{
    class Creature : Entity
    {
        public Coordinates SpawnPosition { get; set; }

        public List<Node> Path = new List<Node>();

        public int SuperPowerSteps { get; set; }
        public Coordinates Destination { get; set; }

        public int TargetID { get; set; }

        public List<Loot> LootList = new List<Loot>();

        public int Level { get; set; }

        public int MaxHealth { get; set; }
        public int Health { get; set; }

        public int MagicStrength { get; set; }


        public int MaxMana { get; set; }
        public int Mana { get; set; }

        public int Strength { get; set; }
        public int Defense { get; set; }

        public int TimeOfLastAttack;


        public Creature()
        {
            ID = -1;
        }
        public Creature(string name, int id = -1) { Name = name; MaxHealth = 1; Health = 1; ID = id; }

        public Creature(string name, Coordinates coordinates, int targetID, int strength = 1, int health = 1, int id = 0, int defense = 0, int experience = 1, List<Loot> loot = null, int spriteid = 0)
        {
            /* Spöken */
            Name = name;
            Position = coordinates;
            SpawnPosition = coordinates;
            MaxHealth = health;
            Health = health;
            ID = id;
            TargetID = targetID;
            SuperPowerSteps = -1;
            Visible = true;
            EntityType = Entity.CreatureEntity;
            Strength = strength;
            TimeOfLastAttack = 0;
            Defense = defense;
            Experience = experience;
            if(loot != null)
            {
                LootList.AddRange(loot);
            }
            SpriteID = spriteid;
        }

        public void Spawn()
        {
            Health = MaxHealth;
            Visible = true;
            Position = SpawnPosition;
            ResetPath();
        }

        public int ReceiveDamage(int strength, int defense)
        {
            Random generator = new Random();
            int DamageDone = generator.Next(0, strength) - generator.Next(0, defense);
            if (DamageDone < 0) DamageDone = 0;
            if (Health - DamageDone < 0)
            {
                DamageDone = DamageDone - (DamageDone - Health);
                Health = 0;
            }
            else
            {
                Health -= DamageDone;
            }
            if(Health < 1)
            {
                Die();
            }
            return DamageDone;
        }

        public void ReceiveExperience(int experience)
        {
            Experience += experience;
            while(IsExperienceEnoughToLevelUp())
            {
                LevelUp();
            }
        }

        public bool IsExperienceEnoughToLevelUp()
        {
            if(Experience >= Utility.ExperienceNeededForLevel(Level)) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void LevelUp()
        {
            Level += 1;
            MaxHealth += 15;
            Health = MaxHealth;
            MaxMana += 15;
            Mana = MaxMana;
        }

        public bool hasPath()
        {
            if (Path.Count > 0)
            {
                return SamePosition(new Coordinates(Path[0].X, Path[0].Y), Destination);
            }
            else
            {
                return false;
            }

        }

        public bool hasTarget()
        {
            if(TargetID != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ResetPath()
        {
            Path.Clear();
        }

        public Coordinates NextStep()
        {
            Coordinates Step = null;
            if (Path.Count > 0)
            {
                Step = Coordinates.Parse(Path[Path.Count - 1]);
                Path.RemoveAt(Path.Count - 1);
            }
            return Step;
        }

        public void Die()
        {
            Health = 0;
            Visible = false;
        }
    }
}
