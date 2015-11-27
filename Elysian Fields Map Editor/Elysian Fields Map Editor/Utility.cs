using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Elysian_Fields_Map_Editor
{
    class Utility
    {
        public static int MinZ = -7;
        public static int MaxZ = 7;
        public static int GroundZ = 0;

        public static int MaxX = 1024;
        public static int MaxY = 1024;

        public static int MinZ_Order = 0;
        public static int MaxZ_Order = 15;

        public static Coordinates CenterScreenCoordinates = new Coordinates(Coordinates.UI_Step * 12, Coordinates.UI_Step * 8); // X: 12 Y: 8

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
