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
                if (name == n1 || name == n2) j--;
                if (j == 0) return j;
            }

            return j;

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



            if (inputNumbers.Length > 2)
            {
                errors2.Text = "Możliwe jest tylko jedno działanie do wykonanie!";
                return;
            }

            if (inputNumbers.Length < 2 || inputNumbers[1] == "")
            {
                errors2.Text = "Brak działania!";
                return;
            }

            if (inputNumbers[0] == inputNumbers[1])
            {
                errors2.Text = "Liczby nie mogą być takie same!";
                return;
            }

            int typ = CheckNumbers(inputNumbers[0], inputNumbers[1]);

            if (typ == 1)
            {
                inputNumbers[0] = inputNumbers[0].Replace("(", "").Replace(")", "");
                MatchCollection matchedNumber1 = rx.Matches(inputNumbers[0]);
                if (matchedNumber1.Count == 0)
                {
                    errors2.Text = "Błędny format liczby!";
                    return;
                };
            }

            double[] numbers1 = typ == 1 ? Fuzzy.TransformToDouble(inputNumbers[0].Split(";")) : Fuzzy.FindElement(inputNumbers[0], fuzzy_numbers);
            double[] numbers2 = Fuzzy.FindElement(inputNumbers[1], fuzzy_numbers);

            //znajdowanie operatora
            int pos = typ == 1 ? inputNumbers[0].Length + 2 : inputNumbers[0].Length;
            char operat = operation[pos];

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
                    for (int i = 0; i < 4; i++)
                    {
                        result += (numbers1[i] * numbers2[i]).ToString();
                        if (i < 3) result += ";";
                    }
                    break;
                case '/':
                    for (int i = 0; i < 4; i++)
                    {
                        result += (numbers1[i] / numbers2[i]).ToString();
                        if (i < 3) result += ";";
                    }
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }
            result += ")";
            dzialanie1.Text = result;
        }
    }
}
