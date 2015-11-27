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
    /* Not yet finished and in use */
    class Field
    {
        private int _ID;
        private int _Position;
        private string _Name;
        private string _Value;
        public int ID { get { return _ID; } set { _ID = value; } }
        public int PositionInFile { get { return _Position; } set { _Position = value; } }
        public string Name { get { return _Name; } set { _Name = value; } }
        public string Value { get { return _Value; } set { _Value = value; } }

        public Field()
        {

        }

        public Field(string fieldName)
        {
            Name = fieldName;
        }

        public Field(string fieldName, string value)
        {
            Name = fieldName;
            Value = value;
        }
    }
}
