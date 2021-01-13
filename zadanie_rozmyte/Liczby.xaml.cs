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
        private bool CheckNumbers(string name1, string name2)
        {
            int i = 2;
            using (StreamReader sr = new StreamReader("fuzzy.txt"))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Split("|")[1] == name1 || line.Split("|")[1] == name2) i--;
                    if (i == 0) return true;
                }
            }

            if (i == 0) return true;
            else return false;

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
            string dzialanie = dzialanie1.Text;
            string[] numbers = dzialanie.Split('+', '-', '/', '*');

            /*if (numbers.Length > 2)
            {
                errors2.Text = "Możliwe jest tylko jedno działanie do wykonanie!";
                return;
            }
            if (numbers[0] == numbers[1])
            {
                errors2.Text = "Liczby nie mogą być takie same!";
                return;
            }*/
            
            /*string dupa = "";
            foreach (double n in numbers1) dupa += n;
            errors2.Text = dupa;*/

            if (dzialanie.Split('+').Length == 2)
            {
                if (CheckNumbers(numbers[0], numbers[1]))
                {
                    string result = "(";

                    double[] numbers1 = Fuzzy.FindElement(numbers[0], fuzzy_numbers);
                    double[] numbers2 = Fuzzy.FindElement(numbers[1], fuzzy_numbers);

                    for (int i = 0; i < 4; i++)
                    {
                        result += (numbers1[i] + numbers2[i]).ToString();
                        if (i < 3) result += ";";
                    }

                    result += ")";

                    dzialanie1.Text = result;
                }
                else errors2.Text = "Błędne działanie!";
            }


        }
    }
}
