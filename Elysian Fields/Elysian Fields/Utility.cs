using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Elysian_Fields
{
    class Utility
    {

        public static int ExperienceNeededForLevel(double level)
        {
            double neededExp = ((Math.Pow(level, 2) * 450) + 450 - (350 * (level / 2) * 4) - ((Math.Pow(level, 2) - 1) * 200) + (Math.Pow(level, 2) * 50) - ((Math.Pow(level, 2) + 2) * 50) );
            return (int)neededExp;
        }
        public static int ManaSpentNeededForMagicStrength(double level)
        {
            double neededExp = level * 100 + (Math.Pow(level, 2) * 50);
            return (int)neededExp;
        }
    }
}
