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
    class Animation
    {
        public int ID;
        public int EndTime;
        public int StartTime;
        public int Steps;
        public int Duration;

        public int StepDuration { get { return Duration / Steps; } set { } }

        public int CurrentStep = 0;

        //public int OffsetX

        public Coordinates Position = new Coordinates();

        public string Text;

        public double StartOffsetY;

        //public int EndOffsetX;
        //public int EndOffsetY = 0;

        public Animation()
        {
            ID = -1;
        }

        public Animation(int _Duration, double _StartOffsetY, int _Steps, int _StartTime, string _Text = "", int id = 0, Coordinates pos = null)
        {
            ID = id;
            EndTime = _StartTime + _Duration;
            StartTime = _StartTime;
            Position = pos;
            Steps = _Steps;
            Duration = _Duration;
            StartOffsetY = _StartOffsetY;
            Text = _Text;
        }

        public double OffsetY(int CurrentTime)
        {
            double CurrentStepEndTime = StartTime + Duration - ((Steps - CurrentStep) * StepDuration);
            if (CurrentTime >= CurrentStepEndTime)
            {
                if (CurrentStep < Steps)
                    CurrentStep++;
            }
            return StartOffsetY - (CurrentStep * (StartOffsetY / Steps));
        }
    }
}
