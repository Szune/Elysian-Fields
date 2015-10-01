using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields
{
    class DamageObject
    {
        public Creature creature;
        public int ID;
        public int damageDealt;

        public DamageObject()
        {
            ID = -1;
        }

        public DamageObject(Creature monster, int Damage, int id = 0)
        {
            creature = monster;
            damageDealt = Damage;
            ID = id;
        }
    }
}
