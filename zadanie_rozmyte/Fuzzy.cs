﻿using System;
using System.Collections.Generic;
using System.Text;

namespace zadanie_rozmyte
{
    class Fuzzy
    {
        public static double[] FindElement(string name, List<Fuzzy> fuzzy_numbers)
        {
            List<double> fuzzyNumbers = new List<double>();

            foreach (Fuzzy f in fuzzy_numbers)
            {
                if (f.Name == name)
                {
                    string[] numbers = f.Number.Split(';');
                    foreach (string n in numbers) fuzzyNumbers.Add(Double.Parse(n));
                    return fuzzyNumbers.ToArray();
                };
            }
            return fuzzyNumbers.ToArray();
        }
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