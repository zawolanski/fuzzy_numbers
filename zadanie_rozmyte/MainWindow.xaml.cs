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

    public partial class MainWindow : Window
    {
        private string CalculateNumber()
        {
            string number = number1.Text;
            string name = number_name.Text;
            Regex rx = new Regex(@"^(\d+(,\d+)?;){3}\d+(,\d+)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matchedNumber1 = rx.Matches(number);
            if (matchedNumber1.Count == 0) return "Błędny format liczby!";
            if (name.Length < 1) return "Nazwa nie może być pusta!";

            double[] arr = Fuzzy.TransformToDouble(number.Split(";"));

            if (!(arr[0] > arr[1] && arr[1] >= arr[2] && arr[2] > arr[3]) && !(arr[0] < arr[1] && arr[1] <= arr[2] && arr[2] < arr[3]))
            {
                return "Błędna liczba rozmyta!";
            }

            if (Fuzzy.IsElementExist(name))
            {
                return "Liczba o wpisanej nazwie już istnieje!";
            }

            using (StreamWriter sw = File.AppendText("fuzzy.txt"))
            {   
                sw.WriteLine($"{number}|{name}");
            }

            number1.Text = "";
            number_name.Text = "";
            return "Liczba została dodana!";
        }
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            errors.Text = CalculateNumber();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Liczby newWindow = new Liczby();
            newWindow.ShowDialog();
        }
    }
}
