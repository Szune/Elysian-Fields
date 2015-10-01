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


        public Creature()
        {

        }
        public Creature(string name, int id = 0) { Name = name; MaxHealth = 1; Health = 1; ID = id; }

        public Creature(string name, Coordinates coordinates, int targetID, ConsoleColor color = ConsoleColor.White, int health = 1, int id = 0)
        {
            /* Spöken */
            Name = name;
            Position = coordinates;
            SpawnPosition = coordinates;
            Color = color;
            MaxHealth = health;
            Health = health;
            ID = id;
            TargetID = targetID;
            SuperPowerSteps = -1;
            Visible = true;
            EntityType = Entity.CreatureEntity;
        }

        public void Spawn()
        {
            Health = MaxHealth;
            Visible = true;
            Position = SpawnPosition;
            ResetPath();
        }

        public int ReceiveDamage(int strength)
        {
            Health -= strength;
            if(Health < 1)
            {
                Die();
            }
            return strength;
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
