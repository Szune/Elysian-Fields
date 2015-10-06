using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Elysian_Fields
{
    class Backpack
    {
        private List<Item> ContainedItems = new List<Item>();
        public Coordinates Position;
        public string Name = "";
        public bool Open = false;
        public int ID;
        public int ItemID;

        public List<Item> GetItems()
        {
            return ContainedItems;
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
            // TODO: Send all items in bag and in their bags and in their bags as well (Bag.Container.GetItemsSafe() until all bags are accounted for)
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

        public bool AddItem(Item item)
        {
            if (ContainedItems.Count < 20)
            {
                item.Parent = this;
                item.ParentID = this.ID;
                item.ParentItemID = this.ItemID;
                ContainedItems.Add(item);
                return true;
            }
            return false;
        }

        public void RemoveItem(Item item)
        {
            item.Parent = new Backpack();
            item.ParentID = -1;
            item.ParentItemID = -1;
            ContainedItems.Remove(item);
        }
    }
}
