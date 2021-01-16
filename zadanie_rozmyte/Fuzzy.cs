﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace zadanie_rozmyte
{
    class Fuzzy
    {
        public static bool IsElementExist(string name)
        {
            string[] readText = File.ReadAllLines("fuzzy.txt");
            foreach (string f in readText) if (f.Split("|")[1] == name) return true;
            return false;
        }
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

        public static double[] TransformToDouble(string[] numbers)
        {
            List<double> m = new List<double>();
            foreach (string n in numbers) m.Add(Double.Parse(n));
            return m.ToArray();
        }
        public static bool IsFuzzyNumber(string number)
        {
            Regex rx = new Regex(@"^(\d+(.\d+)?;){3}\d+(.\d+)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            number = number.Replace("(", "").Replace(")", "");
            MatchCollection matchedNumber1 = rx.Matches(number);

            if (matchedNumber1.Count == 1)
            {
                return true;
            } else
            {
                return false;
            }
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