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
