using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace zadanie_rozmyte
{
    class PassedNumber
    {
        public string Number { get; set; }
        
        public double[] CheckNumber(List<Fuzzy> fuzzy_numbers)
        {
            string currentNumber = Number.Replace("(", "").Replace(")", "");
            Regex rx = new Regex(@"^(\-?\d+(,\d+)?;){2,}\-?\d+(,\d+)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matchedNumber = rx.Matches(currentNumber);
            if (matchedNumber.Count == 1)
            {
                double[] arr = Fuzzy.TransformToDouble(currentNumber.Split(";"));
                if((arr[0] >= arr[1] && arr[1] >= arr[2] && arr[2] >= arr[3]) || (arr[0] <= arr[1] && arr[1] <= arr[2] && arr[2] <= arr[3]))
                {
                    return arr;
                }
                return new double[0];
            }
            else
            {
                for (int i = 0; i < fuzzy_numbers.Count; i++)
                {
                    string name = fuzzy_numbers[i].Name;
                    if (name == currentNumber) return Fuzzy.TransformToDouble(fuzzy_numbers[i].Number.Split(";"));
                }
               
            }

            return new double[0];
        }
        public double[] PrepareArray(string type)
        {
            string[] splitedNumner = Number.Split(';');
            double[] convertedNumber = new double[Number.Length];
            for (int i = 0; i < Number.Length; i++) convertedNumber[i] = Double.Parse(splitedNumner[i]);
            return convertedNumber;

        }
        public PassedNumber(string number)
        {
            Number = number;
        }
    }
}
