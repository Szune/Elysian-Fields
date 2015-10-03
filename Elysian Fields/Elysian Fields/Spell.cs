using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Elysian_Fields
{
    class Spell
    {
        public int Damage;
        public bool[] Area;
        public Texture2D Sprite;
        public int ID;
        public int ManaCost;
        public const int ExhaustionTime = 500; // Milliseconds
        public bool HealSpell;
        public bool TargetSpell;

        public Spell()
        {
            ID = -1;
        }

        public Spell(bool[] area, int damage, Texture2D sprite, int manacost = 0, bool healspell = false, bool targetspell = false, int id = 0)
        {
            Area = area;
            Damage = damage;
            Sprite = sprite;
            ID = id;
            ManaCost = manacost;
            HealSpell = healspell;
            TargetSpell = targetspell;
        }
    }
}
