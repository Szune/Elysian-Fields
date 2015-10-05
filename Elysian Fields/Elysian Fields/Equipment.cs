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


        public Equipment()
        {

        }

        public int TotalStrength()
        {
            return LeftHand.Strength + RightHand.Strength + Armor.Strength + Helmet.Strength;
        }

        public int TotalDefense()
        {
            return LeftHand.Defense + RightHand.Defense + Armor.Defense + Helmet.Defense;
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
        }

        public override string ToString()
        {
            return LeftHand.ID + "," + RightHand.ID + "," + Helmet.ID + "," + Armor.ID;
        }

    }
}
