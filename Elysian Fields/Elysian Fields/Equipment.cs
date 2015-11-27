using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Elysian_Fields
{
    class Equipment
    {
        public Item LeftHand = new Item();
        public Item RightHand = new Item();
        public Item Armor = new Item();
        public Item Helmet = new Item();
        public Item Legs = new Item();
        public Item Bag = new Item();


        public Equipment()
        {

        }

        public int TotalStrength()
        {
            return LeftHand.Strength + RightHand.Strength + Armor.Strength + Helmet.Strength + Legs.Strength;
        }

        public int TotalDefense()
        {
            return LeftHand.Defense + RightHand.Defense + Armor.Defense + Helmet.Defense + Legs.Defense;
        }

        public bool IsItemSlotEmpty(string _ItemSlot)
        {
            if (_ItemSlot == ItemSlot.LeftHand)
            {
                if (LeftHand.ID == -1)
                    return true;
            }
            else if (_ItemSlot == ItemSlot.RightHand)
            {
                if (RightHand.ID == -1)
                    return true;
            }
            else if (_ItemSlot == ItemSlot.Armor)
            {
                if (Armor.ID == -1)
                    return true;
            }
            else if (_ItemSlot == ItemSlot.Helmet)
            {
                if (Helmet.ID == -1)
                    return true;
            }
            else if (_ItemSlot == ItemSlot.Legs)
            {
                if (Legs.ID == -1)
                    return true;
            }
            else if (_ItemSlot == ItemSlot.Bag)
            {
                if (Bag.ID == -1)
                    return true;
            }
            return false;
        }

        public void EquipItem(Item item, string _ItemSlot)
        {
            /*ItemSlot hej = new ItemSlot();
            foreach (var prop in typeof(ItemSlot).GetProperties())
            {
                if (prop.CanRead)
                {
                    var value = prop.GetValue(hej, null) as string;
                }
            }*/ // The code above will loop through all the constants of the ItemSlot class, might be useful if we make changes to the Equipment class

            if (_ItemSlot == ItemSlot.LeftHand)
            {
                LeftHand = item;
            }
            else if(_ItemSlot == ItemSlot.RightHand)
            {
                RightHand = item;
            }
            else if(_ItemSlot == ItemSlot.Armor)
            {
                Armor = item;
            }
            else if (_ItemSlot == ItemSlot.Helmet)
            {
                Helmet = item;
            }
            else if (_ItemSlot == ItemSlot.Legs)
            {
                Legs = item;
            }
            else if (_ItemSlot == ItemSlot.Bag)
            {
                Bag = item;
            }
        }

        public void UnequipItem(string _ItemSlot)
        {
            if (_ItemSlot == ItemSlot.LeftHand)
            {
                LeftHand = new Item();
            }
            else if (_ItemSlot == ItemSlot.RightHand)
            {
                RightHand = new Item();
            }
            else if (_ItemSlot == ItemSlot.Armor)
            {
                Armor = new Item();
            }
            else if (_ItemSlot == ItemSlot.Helmet)
            {
                Helmet = new Item();
            }
            else if (_ItemSlot == ItemSlot.Legs)
            {
                Legs = new Item();
            }
            else if (_ItemSlot == ItemSlot.Bag)
            {
                Bag = new Item();
            }
        }

        public override string ToString()
        {
            bool first = true;
            string items = "Items:";
            string bags = "Bags:";
            if (Bag.Container != null)
            {
                List<Item> AllItems = new List<Item>();
                AllItems = Bag.Container.GetAllItemsFromNestedBags(Bag);
                bags += Bag.SpriteID + ";" + Bag.ID + ";-1";

                for (int i = 0; i < AllItems.Count; i++)
                {
                    if (AllItems[i].WearSlot == ItemSlot.Bag)
                    {
                        bags += "+" + AllItems[i].SpriteID + ";" + AllItems[i].ID + ";" + AllItems[i].ParentID;
                    }
                }

                for (int i = 0; i < AllItems.Count; i++)
                {
                    if (AllItems[i].Container == null)
                    {
                        if(!first)
                        {
                            items += "+" +AllItems[i].SpriteID + ";" + AllItems[i].ParentID;
                        }
                        else
                        {
                            items += AllItems[i].SpriteID + ";" + AllItems[i].ParentID;
                            first = false;
                        }                       
                    }
                }
            }
            bags += ".";

            if (bags != "Bags:.")
            {
                return LeftHand.SpriteID + "," + RightHand.SpriteID + "," + Helmet.SpriteID + "," + Armor.SpriteID + "," + Legs.SpriteID + "," + bags + items;
            }
            else
            {
                if (Bag.ID != -1)
                {
                    return LeftHand.SpriteID + "," + RightHand.SpriteID + "," + Helmet.SpriteID + "," + Armor.SpriteID + "," + Legs.SpriteID + ",Bags:" + Bag.SpriteID + ";" + Bag.ID + ".Items:-1";
                }
                else
                {
                    return LeftHand.SpriteID + "," + RightHand.SpriteID + "," + Helmet.SpriteID + "," + Armor.SpriteID + "," + Legs.SpriteID + ",Bags:-1.Items:-1";
                }
            }
        }

    }
}
