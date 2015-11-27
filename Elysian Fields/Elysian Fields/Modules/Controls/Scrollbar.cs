using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elysian_Fields.Modules.Controls
{
    class Scrollbar
    {
        public Coordinates Position;
        public Coordinates UpArrow;
        public Coordinates DownArrow;

        public int Height;
        public int MaxHeight;
        public int Start;
        public int End;
        public int CurrentStep;

        public Scrollbar()
        {
            Position = null;
            UpArrow = null;
            DownArrow = null;
            Start = 0;
            End = 0;
            CurrentStep = 0;
            Height = 0;
            MaxHeight = 0;
        }

    }
}
