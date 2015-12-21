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
using Microsoft.Xna.Framework.Graphics;

namespace Elysian_Fields
{
    class Spell
    {
        public int Damage;
        public double[] Area;
        public Texture2D Sprite;
        public int ID;
        public int ManaCost;
        public string Name;
        public const int ExhaustionTime = 500; // Milliseconds
        public bool HealSpell;
        public bool TargetSpell;
        public bool AreaSpell;
        public int Cooldown;

        dynamic lua = new DynamicLua.DynamicLua();
        string PathToLua;

        public Spell()
        {
            ID = -1;
        }

        public Spell(string path)
        {
            LoadScript(path);
        }

        public void LoadScript(string path)
        {
            Storage storage = Storage.Instance;
            PathToLua = path;
            lua.DoFile(path);
            Name = (string)lua.Name;
            if(lua.Spell_Area == true)
            {
                AreaSpell = true;
                DynamicLua.DynamicLuaTable tab = lua.Area;

                int i = 0;
                foreach(KeyValuePair<object, object> kvp in tab)
                {
                    i++;
                }

                Area = new double[i];

                i = 0;
                foreach (KeyValuePair<object, object> kvp in tab)
                {
                    Area[i] = (double)kvp.Value;
                    i++;
                }
            }
            Damage = (int)lua.Damage;
            Sprite = storage.GetSpriteByName((string)lua.SpriteName);
            ManaCost = (int)lua.Mana_Cost;
            HealSpell = lua.Spell_Heal;
            TargetSpell = lua.Spell_RequireTarget;
            ID = (int)lua.ID;
            Cooldown = (int)lua.Cooldown;
        }

        public Spell(double[] area, int damage, Texture2D sprite, int manacost = 0, bool healspell = false, bool targetspell = false, int id = 0)
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
