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
using System.IO;

namespace Elysian_Fields
{
    class Utility
    {
        public static int MinZ = -7;
        public static int MaxZ = 7;
        public static int GroundZ = 0;

        public static int MaxX = 1024;
        public static int MaxY = 1024;

        public static int MaxZ_Order = 15;

        public static Coordinates CenterCoordinates = new Coordinates(Coordinates.Step * 12, Coordinates.Step * 8); // X: 12 Y: 8

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
