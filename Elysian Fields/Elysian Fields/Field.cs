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
