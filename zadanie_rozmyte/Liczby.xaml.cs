using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using Spire.Xls;

namespace zadanie_rozmyte
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class Liczby : Window
    {
        string[] readText;
        List<Fuzzy> fuzzy_numbers = new List<Fuzzy>();
        Regex rx = new Regex(@"^(\d+(.\d+)?;){3}\d+(.\d+)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //jeśli zwróci 0 to obie liczby były zmiennymi, a jeśli 1 to tylko druga liczba była zmienną
        private int CheckNumbers(string n1, string n2)
        {
            int j = 2;
            for(int i = 0; i < fuzzy_numbers.Count; i++)
            {
                string name = fuzzy_numbers[i].Name;
                if (name == n1.Trim() && name == n2.Trim()) return 0;
                if (name == n1.Trim() || name == n2.Trim()) j--;
                if (j == 0) return j;
            }

            return j;

        }

        private void SaveToExcel(List<double> ups, List<double> Ypoints)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet = workbook.Worksheets[0];
            sheet.Name = "fuzzy numbers";
            sheet.Range["A1"].Text = "x";
            sheet.Range["B1"].Text = "y";

            for (int i = 0; i < ups.Count; i++)
            {
                sheet.Range["A" + (i + 2)].NumberValue = ups.ElementAt(i);
                sheet.Range["B" + (i + 2)].NumberValue = Ypoints.ElementAt(i);
            }

            workbook.SaveToFile("fuzzynumbers.xlsx", ExcelVersion.Version2016);
        }

        private bool ValidateNumbers(string[] inputNumbers)
        {
            if (inputNumbers.Length > 2)
            {
                errors2.Text = "Możliwe jest tylko jedno działanie do wykonanie!";
                return false;
            }

            if (inputNumbers.Length < 2 || inputNumbers[1] == "")
            {
                errors2.Text = "Brak działania!";
                return false;
            }

            /*if (inputNumbers[0] == inputNumbers[1]) //zapytać czy można dodawać te same (a+a) (1;1;1;1)+(1;1;1;1)
            {
                errors2.Text = "Liczby nie mogą być takie same!";
                return false;
            }*/
            return true;
        }
        public Liczby()
        {
            InitializeComponent();
            readText = File.ReadAllLines("fuzzy.txt");
            
            foreach (string n in readText) fuzzy_numbers.Add(new Fuzzy(n.Split("|")[1], n.Split("|")[0]));

            StackPanel myStackPanel = new StackPanel();
            myStackPanel.HorizontalAlignment = HorizontalAlignment.Left;
            myStackPanel.VerticalAlignment = VerticalAlignment.Top;
            
            foreach(Fuzzy n in fuzzy_numbers)
            {
                TextBlock myTextBlock = new TextBlock();
                myTextBlock.Text = $"{n.Name}:({n.Number})";
                myTextBlock.FontSize = 22;
                myStackPanel.Children.Add(myTextBlock);
            }

            viewer.Content = myStackPanel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string operation = dzialanie1.Text;
            string[] inputNumbers = operation.Split('+', '-', '/', '*');

            bool isValidNumbers = ValidateNumbers(inputNumbers);
            if(isValidNumbers == false) return;

            int typ = CheckNumbers(inputNumbers[0], inputNumbers[1]);

            double[] numbers1;
            double[] numbers2;
            //(2;4;6;7)+(2;4;6;7)

            int pos;

            if (typ == 0) {
                numbers1 = Fuzzy.FindElement(inputNumbers[0], fuzzy_numbers);
                numbers2 = Fuzzy.FindElement(inputNumbers[1], fuzzy_numbers);
                pos = inputNumbers[0].Length;
            } else if (typ == 1) {
                bool isFuzzyNumber = Fuzzy.IsFuzzyNumber(inputNumbers[0]);
                inputNumbers[0] = inputNumbers[0].Replace("(", "").Replace(")", "");
                if (isFuzzyNumber == false)
                {
                    errors2.Text = "Błędny format liczby!";
                    return;
                };
                numbers1 = Fuzzy.TransformToDouble(inputNumbers[0].Split(";"));
                numbers2 = Fuzzy.FindElement(inputNumbers[1], fuzzy_numbers);
                pos = inputNumbers[0].Length + 2;
            } else { 
                inputNumbers[0] = inputNumbers[0].Replace("(", "").Replace(")", "");
                inputNumbers[1] = inputNumbers[1].Replace("(", "").Replace(")", "");

                MatchCollection matchedNumber1 = rx.Matches(inputNumbers[1]);
                if (matchedNumber1.Count == 0)
                {
                    errors2.Text = "Błędny format liczby!";
                    return;
                };
                
                numbers1 = Fuzzy.TransformToDouble(inputNumbers[0].Split(";"));
                numbers2 = Fuzzy.TransformToDouble(inputNumbers[1].Split(";"));
                pos = inputNumbers[0].Length + 2;
            }

            //znajdowanie operatora
            char operat = operation[pos];
            errors3.Text = operat.ToString();
            double num;
            double discretizationValue = 1.0;
            if (Double.TryParse(discretization.Text, out num)) discretizationValue = num;

            
            errors2.Text = "";
            string result = "(";
            switch (operat)
            {
                case '+':
                    for (int i = 0; i < 4; i++)
                    {
                        result += (numbers1[i] + numbers2[i]).ToString();
                        if (i < 3) result += ";";
                    }
                    break;
                case '-':
                    for (int i = 0; i < 4; i++)
                    {
                        result += (numbers1[i] - numbers2[i]).ToString();
                        if (i < 3) result += ";";
                    }
                    break;
                case '*':
                    List<double> ups = new List<double>();
                    List<double> downs = new List<double>();
                    List<double> y = new List<double>();

                    double m = 1 / discretizationValue;
                    for (int d = 0; d <= discretizationValue; d++)
                    {
                        double k = Math.Round(m * d, 5);
                        double up = Math.Round((k * (numbers1[1] - numbers1[0]) + numbers1[0]) * (k * (numbers2[1] - numbers2[0]) + numbers2[0]), 2);
                        double down = Math.Round((k * (numbers1[3] - numbers1[2]) + numbers1[2]) * (k * (numbers2[3] - numbers2[2]) + numbers2[2]), 2);

                        ups.Add(up);
                        downs.Add(down);
                        y.Add(k);
                    }

                    ups.AddRange(downs);

                    List<double> Ypoints = new List<double>();
                    Ypoints.AddRange(y);
                    y.Reverse();
                    Ypoints.AddRange(y);

                    SaveToExcel(ups, Ypoints);

                    errors2.Text = "X: " + String.Join(" ", ups);
                    errors3.Text = "Y: " + String.Join(" ", Ypoints);

                    break;
                case '/':
                    List<double> ups2 = new List<double>();
                    List<double> downs2 = new List<double>();
                    List<double> y2 = new List<double>();

                    double m2 = 1 / discretizationValue;
                    for (int d = 0; d <= discretizationValue; d++)
                    {
                        double k = Math.Round(m2 * d, 5);
                        double up = Math.Round((k * (numbers1[1] - numbers1[0]) + numbers1[0]) / (k * (numbers2[1] - numbers2[0]) + numbers2[0]), 2);
                        double down = Math.Round((k * (numbers1[3] - numbers1[2]) + numbers1[2]) / (k * (numbers2[3] - numbers2[2]) + numbers2[2]), 2);

                        ups2.Add(up);
                        downs2.Add(down);
                        y2.Add(k);
                    }

                    ups2.AddRange(downs2);

                    List<double> Ypoints2 = new List<double>();
                    Ypoints2.AddRange(y2);
                    y2.Reverse();
                    Ypoints2.AddRange(y2);

                    SaveToExcel(ups2, Ypoints2);

                    errors2.Text = "X: " + String.Join(" ", ups2);
                    errors3.Text = "Y: " + String.Join(" ", Ypoints2);

                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }
            result += ")";
            dzialanie1.Text = result;
            
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            string nameOfSetVar = nameOfSet.Text.Trim();
            double argumentVar = Double.Parse(argument.Text.Trim());
            

            int typ = CheckNumbers(nameOfSetVar, nameOfSetVar);
            if (typ != 0)
            {
                errors3.Text = "Brak liczby o podanej nazwie!";
                return;
            }

            double[] number = Fuzzy.FindElement(nameOfSetVar, fuzzy_numbers);

            if(argumentVar <= number[0] || argumentVar > number[3]) errors3.Text = "Wartość przynależności wynosi wynosi: 0";
            else if(argumentVar > number[1] && argumentVar <= number[2]) errors3.Text = "Wartość przynależności wynosi wynosi: 1";
            else if (argumentVar > number[0] && argumentVar <= number[1])
            {
                double result = (argumentVar - number[0])/(number[1] - number[0]);
                errors3.Text = $"Wartość przynależności wynosi wynosi: {result}";
            }
            else if (argumentVar > number[2] && argumentVar <= number[3])
            {
                double result = (number[3] - argumentVar) / (number[3] - number[2]);
                errors3.Text = $"Wartość przynależności wynosi wynosi: {result}";
            }
        }
    }
}
