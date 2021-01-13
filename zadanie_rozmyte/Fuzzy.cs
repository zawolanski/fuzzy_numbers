using System;
using System.Collections.Generic;
using System.Text;

namespace zadanie_rozmyte
{
    class Fuzzy
    {
        public string Name { get; set; }
        public string Number { get; set; }

        public Fuzzy()
        {
            Name = "";
            Number = "";
        }

        public Fuzzy(string name, string number)
        {
            Name = name;
            Number = number;
        }
    }
}