using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields
{
    class Equipment
    {
        public Item LeftHand = new Item();
        public Item RightHand = new Item();


        public Equipment()
        {

        }

        public int TotalStrength()
        {
            return LeftHand.Strength + RightHand.Strength;
        }
    }
}
