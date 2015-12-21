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
    public sealed class Storage
    {
        private static readonly Storage instance = new Storage();

        internal List<SpriteObject> spriteList = new List<SpriteObject>();

        private Storage()
        {

        }

        public static Storage Instance
        {
            get
            {
                return instance;
            }
        }

        internal Texture2D GetSpriteByID(int ID)
        {
            for (int i = 0; i < spriteList.Count; i++)
            {
                if (spriteList[i].ID == ID)
                {
                    return spriteList[i].Sprite;
                }

            }
            return spriteList[0].Sprite;
        }

        internal Texture2D GetSpriteByName(string Name)
        {
            for (int i = 0; i < spriteList.Count; i++)
            {
                if (spriteList[i].SpriteName == Name)
                {
                    return spriteList[i].Sprite;
                }

            }
            return spriteList[0].Sprite;
        }

        internal int GetSpriteIDByName(string Name)
        {
            for (int i = 0; i < spriteList.Count; i++)
            {
                if (spriteList[i].SpriteName == Name)
                {
                    return spriteList[i].ID;
                }

            }
            return spriteList[0].ID;
        }

        internal SpriteObject GetSpriteObjectByID(int id)
        {
            for (int i = 0; i < spriteList.Count; i++)
            {
                if (spriteList[i].ID == id)
                {
                    return spriteList[i];
                }

            }
            return spriteList[0];
        }

        // Store sprites and stuff like that here
    }
}
