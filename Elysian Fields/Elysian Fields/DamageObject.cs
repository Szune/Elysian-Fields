﻿using System;
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