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
    class DamageObject
    {
        public Creature creature;
        public int ID;
        public int damageDealt;
        public int EndTime;
        public int StartTime;
        public const int Steps = 30;
        public const int DamageDuration = 1000;
        
        public int StepDuration { get { return DamageDuration / Steps; } set { } }

        public int CurrentStep = 0;

        public bool Healing;

        //public int OffsetX

        public Coordinates Position = new Coordinates();

        //public int StartOffsetX;
        public double StartOffsetY = 15;

        //public int EndOffsetX;
        //public int EndOffsetY = 0;

        public DamageObject()
        {
            ID = -1;
        }

        public DamageObject(Creature monster, int Damage, bool healing, int _StartTime, int _EndTime, int id = 0, Coordinates pos = null)
        {
            creature = monster;
            damageDealt = Damage;
            ID = id;
            EndTime = _EndTime;
            StartTime = _StartTime;
            Position = pos;
            Healing = healing;
        }

        public double OffsetY(int CurrentTime)
        {
            double CurrentStepEndTime = StartTime + DamageDuration - ((Steps - CurrentStep) * StepDuration);
            if (CurrentTime >= CurrentStepEndTime)
            {
                if(CurrentStep < Steps)
                CurrentStep++;
            }
            return StartOffsetY - (CurrentStep * (StartOffsetY / Steps));
        }
    }
}
