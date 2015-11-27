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
using System.Reflection;
using Elysian_Fields.Modules.Controls;

namespace Elysian_Fields
{
    class Backpack
    {
        private List<Item> ContainedItems = new List<Item>();
        public Coordinates Position;
        public string Name = "";
        public bool Open = false;
        public int ID = -1;
        public int ItemID;
        public Coordinates ClosePosition;
        public Scrollbar Scroll = new Scrollbar();
        public int Height = 80;
        public int MaxHeight = 200;

        public bool AddItem(Item item)
        {
            if (ContainedItems.Count < 20)
            {
                item.Parent = this;
                item.ParentID = this.ID;
                item.ParentItemID = this.ItemID;
                item.Slot = ItemSlot.InsideBag;
                ContainedItems.Add(item);

                for(int i = 0; i < ContainedItems.Count; i++)
                {
                    ContainedItems[i].BagSlot = i;
                }
                return true;
            }
            return false;
        }

        public void RemoveItem(Item item)
        {
            item.BagSlot = -1;
            item.Parent = new Backpack();
            item.ParentID = -1;
            item.ParentItemID = -1;
            ContainedItems.Remove(item);
        }


        public List<Item> GetItems()
        {
            return ContainedItems;
        }

        public void Sort()
        {
            ContainedItems.Sort((a, b) => a.BagSlot.CompareTo(b.BagSlot));
        }

        public List<Item> GetItemsSafe()
        {
            List<Item> newList = new List<Item>();
            newList.AddRange(ContainedItems);
            return newList;
        }

        public Item GetItemBySlot(int Slot)
        {
            if (Slot < ContainedItems.Count - 1)
                return ContainedItems[Slot];
            else
                return new Item();
        }

        public bool IsEmpty()
        {
            if(ContainedItems.Count > 0)
            {
                return false;
            }
            return true;
        }

        public List<Item> GetAllItemsFromNestedBags(Item Bag)
        {
            List<Item> AllItems = new List<Item>();
            bool loopDone = false;
            if (this.ContainedItems.Count > 0)
            {
                List<Item> bagItems = new List<Item>();
                bagItems = Bag.Container.GetItemsSafe() ;
                List<Backpack> moreBags = new List<Backpack>();


                while (!loopDone)
                {

                    for (int i = 0; i < bagItems.Count; i++)
                    {
                        if (bagItems[i].Container != null)
                        {
                            if (!bagItems[i].Container.IsEmpty())
                            {
                                AllItems.Add(bagItems[i]);
                                moreBags.Add(bagItems[i].Container);
                            }
                            else
                            {
                                AllItems.Add(bagItems[i]);
                            }
                        }
                        else
                        {
                            AllItems.Add(bagItems[i]);
                        }
                    }
                    bagItems.Clear();
                    if (moreBags.Count > 0)
                    {
                        bagItems = moreBags[0].GetItemsSafe();
                        moreBags.RemoveAt(0);
                    }
                    else
                    {
                        loopDone = true;
                    }
                }
            }
            List<Item> newList = new List<Item>();
            newList.AddRange(AllItems);
            return newList;
        }
    }
}
