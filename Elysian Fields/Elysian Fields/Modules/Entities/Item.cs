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
    class Item : Entity
    {

        public int Strength;
        public int Defense;
        public string Slot = null;
        public string WearSlot;
        public int WearingPlayerID;
        public Backpack Container;
        public Backpack Parent;
        public int ParentID = -1;
        public int ParentItemID = -1;
        public bool isEmpty = true;
        public int BagSlot = -1;
        public int Z_order = 0;
        public int RealID = 0;

        public Item()
        {
            ID = -1;
            Strength = 0;
            Defense = 0;
        }
        public Item(string name, int realid, string wearslot, Coordinates pos, int spriteid, int id = 0, int strength = 0, int defense = 0, bool visible = true, int wearingplayerid = 0, int zorder = 0)
        {
            RealID = realid;
            Name = name;
            ID = id;
            Position = pos;
            Visible = visible;
            EntityType = Entity.ItemEntity;
            SpriteID = spriteid;
            Strength = strength;
            Defense = defense;
            WearSlot = wearslot;
            WearingPlayerID = wearingplayerid;
            if (wearslot == ItemSlot.Bag)
            {
                Container = new Backpack();
                Container.ID = this.ID;
                Container.ItemID = spriteid;
            }
            Z_order = zorder;
            Parent = new Backpack();
        }
    }
}
