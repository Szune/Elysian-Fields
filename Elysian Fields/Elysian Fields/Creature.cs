using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields
{
    class Creature : Entity
    {
        public Coordinates SpawnPosition { get; set; }

        public List<Coordinates> Path = new List<Coordinates>();

        public int SuperPowerSteps { get; set; }
        public Coordinates Destination { get; set; }

        public int TargetID { get; set; }

        public int MaxHealth { get; set; }
        public int Health { get; set; }

        public int Strength { get; set; }
        public int Defense { get; set; }

        public int TimeOfLastAttack;


        public Creature()
        {
            ID = -1;
        }
        public Creature(string name, int id = -1) { Name = name; MaxHealth = 1; Health = 1; ID = id; }

        public Creature(string name, Coordinates coordinates, int targetID, int strength = 1, int health = 1, int id = 0, int defense = 0)
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
            Health -= DamageDone;
            if(Health < 1)
            {
                Die();
            }
            return DamageDone;
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
                Step = Path[Path.Count - 1];
                Path.RemoveAt(Path.Count - 1);
            }
            return Step;
        }

        public void Die()
        {
            Visible = false;
        }
    }
}
