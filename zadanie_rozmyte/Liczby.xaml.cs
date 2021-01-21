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
using System.Diagnostics;

namespace zadanie_rozmyte
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class Liczby : Window
    {
        string[] readText;
        List<Fuzzy> fuzzy_numbers = new List<Fuzzy>();
        Regex rx = new Regex(@"^(\-?\d+(.\d+)?;){2,}\-?\d+(.\d+)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //jeśli zwróci 0 to obie liczby były zmiennymi, a jeśli 1 to tylko druga liczba była zmienną
        private int CheckNumbers(string n1, string n2)
        {
            int j = 2;
            for (int i = 0; i < fuzzy_numbers.Count; i++)
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

        private string CalculateNumbers(string oper, double[] numbers1, double[] numbers2, double discretization, string name)
        {
            string result = "(";
            List<double> y = new List<double>();

            double m = 1 / discretization;
            for (int d = 0; d < numbers1.Length; d++)
            {
                double k = Math.Round(m * d, 5);

                if (oper == "+") result += (Math.Round(numbers1[d] + numbers2[d], 2)).ToString();
                if (oper == "-") result += (Math.Round(numbers1[d] - numbers2[d], 2)).ToString();
                if (oper == "*") result += (Math.Round(numbers1[d] * numbers2[d], 2)).ToString();
                if (oper == "/") { 
                    if(numbers2[d] == 0) return "";
                    
                    result += (Math.Round(numbers1[d] / numbers2[d], 2)).ToString(); }

                if (d < numbers1.Length - 1) result += ";";
                if (k <= 1) y.Add(k);
            }

            List<double> Y = new List<double>();
            Y.AddRange(y);
            y.Reverse();
            Y.AddRange(y);

            List<double> upsAdd = new List<double>();
            string[] resultArrAdd = result.Split(";");
            string Yresult = "";
            for (int i = 0; i < resultArrAdd.Length; i++)
            {
                resultArrAdd[i] = resultArrAdd[i].Replace("(", "");
                upsAdd.Add(Double.Parse(resultArrAdd[i]));
                Yresult += Y[i];
                if (i < resultArrAdd.Length - 1) Yresult += ";";
            }


            if (name != "")
            {
                using (StreamWriter sw = File.AppendText("fuzzy.txt"))
                {
                    sw.WriteLine($"{result.Replace("(", "").Replace(")", "")}|{name}|{discretization}|{Yresult}");
                }
                Init();
            }

            SaveToExcel(upsAdd, Y);
            result += ")";
            return result;
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

            return true;
        }

        private void Init()
        {
            readText = File.ReadAllLines("fuzzy.txt");
            fuzzy_numbers.Clear();

            foreach (string n in readText)
            {
                fuzzy_numbers.Add(new Fuzzy(n.Split("|")[1], n.Split("|")[0], Double.Parse(n.Split("|")[2]), n.Split("|")[3]));
            }
            
            Grid myGrid = new Grid();
            ColumnDefinition colDef1 = new ColumnDefinition();
            ColumnDefinition colDef2 = new ColumnDefinition();
            RowDefinition rowDef1 = new RowDefinition();

            ScrollViewer viewer = new ScrollViewer();
            viewer.HorizontalAlignment = HorizontalAlignment.Center;
            viewer.Width = 771;
            viewer.Height = 900;
            viewer.Foreground = Brushes.White;

            Thickness margin = myGrid.Margin;
            margin.Top = 30;
            myGrid.Margin = margin;

            myGrid.Width = 730;
            myGrid.ShowGridLines = true;

            myGrid.HorizontalAlignment = HorizontalAlignment.Right;
            myGrid.VerticalAlignment = VerticalAlignment.Top;

            myGrid.ColumnDefinitions.Add(colDef1);
            myGrid.ColumnDefinitions.Add(colDef2);
            myGrid.RowDefinitions.Add(rowDef1);

            TextBlock titleNameTextBlock = new TextBlock();
            TextBlock titleNumberTextBlock = new TextBlock();

            titleNameTextBlock.Text = "Nazwa";
            titleNumberTextBlock.Text = "Liczba";

            Grid.SetColumn(titleNameTextBlock, 0);
            Grid.SetRow(titleNameTextBlock, 0);
            Grid.SetColumn(titleNumberTextBlock, 1);
            Grid.SetRow(titleNumberTextBlock, 0);

            titleNameTextBlock.FontSize = 28;
            titleNumberTextBlock.FontSize = 28;

            myGrid.Children.Add(titleNameTextBlock);
            myGrid.Children.Add(titleNumberTextBlock);

            for (int i = 0; i < fuzzy_numbers.Count; i++)
            {
                TextBlock nameTextBlock = new TextBlock();
                TextBlock numberTextBlock = new TextBlock();

                nameTextBlock.TextWrapping = TextWrapping.Wrap;
                numberTextBlock.TextWrapping = TextWrapping.Wrap;

                RowDefinition rowDef = new RowDefinition();
                myGrid.RowDefinitions.Add(rowDef);

                nameTextBlock.Text = $"{fuzzy_numbers[i].Name}";
                numberTextBlock.Text = $"({fuzzy_numbers[i].Number})";

                nameTextBlock.FontSize = 28;
                numberTextBlock.FontSize = 28;

                Grid.SetColumn(nameTextBlock, 0);
                Grid.SetRow(nameTextBlock, i + 1);
                Grid.SetColumn(numberTextBlock, 1);
                Grid.SetRow(numberTextBlock, i + 1);

                myGrid.Children.Add(nameTextBlock);
                myGrid.Children.Add(numberTextBlock);
            }

            viewer.Content = myGrid;

            viewerBox.Children.Clear();
            viewerBox.Children.Add(viewer);
        }
        public Liczby()
        {
            InitializeComponent();
            Init();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string operationValue = operation.Text;
            string nameValue = name.Text;

            if (Fuzzy.IsElementExist(nameValue))
            {
                errors2.Text = "Liczba o wpisanej nazwie już istnieje!";
                return;
            }
            int pos;

            Regex rx = new Regex(@"^\(?(\-?\d+(,\d+)?;){2,}\-?\d+(,\d+)?\)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            string[] separatingStrings = { ")-", "-(", ")*", "*(", ")/", "/(", ")+", "+(" };
            string[] inputNumbers = operationValue.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            if (inputNumbers.Length != 2)
            {
                inputNumbers = operationValue.Split('-', '+', '/', '*');
                if (inputNumbers.Length != 2)
                {
                    errors2.Text = "Błędna operacja!";
                    return;
                }

                pos = inputNumbers[0].Length;
            } else
            {
                pos = inputNumbers[0].Length + 1;
            }

            char operat = operationValue[pos];

            bool isValidNumbers = ValidateNumbers(inputNumbers);
            if (isValidNumbers == false) return;

            double[] numbers1;
            double[] numbers2;

            //znajdowanie operatora
            //(2;4;6;7)+(2;4;6;7)
            PassedNumber n1 = new PassedNumber(inputNumbers[0]);
            PassedNumber n2 = new PassedNumber(inputNumbers[1]);

            numbers1 = n1.CheckNumber(fuzzy_numbers);
            numbers2 = n2.CheckNumber(fuzzy_numbers);

            if (numbers1.Length != numbers2.Length || numbers1.Length == 0 || numbers2.Length == 0)
            {
                errors2.Text = "Podane wartości są nieprawidłowe!";
                return;
            }

            //dyskretyzacja
            double discretization = 1.0;
            string numberConcat = "";

            for(int i = 0; i < numbers1.Length; i++) {
                numberConcat += numbers1[i].ToString();
                if (i < numbers1.Length - 1) numberConcat += ";";
            }

            Fuzzy obj = fuzzy_numbers.Find(x => x.Number == numberConcat);

            if (obj != null) discretization = obj.Discretization;
            else { discretization = numberConcat.Split(";").Length / 2 - 1; }

            //obliczenia
            string result = "";
            errors2.Text = "";
            switch (operat)
            {
                case '-':
                    result = CalculateNumbers("-", numbers1, numbers2, discretization, nameValue);
                    break;
                case '+':
                    result = CalculateNumbers("+", numbers1, numbers2, discretization, nameValue);
                    break;
                case '*':
                    result = CalculateNumbers("*", numbers1, numbers2, discretization, nameValue);

                    break;
                case '/':
                    result = CalculateNumbers("/", numbers1, numbers2, discretization, nameValue);
                    if (result == "")
                    {
                        errors2.Text = "Nie można dzielić przez 0";
                        return;
                    }

                    break;
                default:
                    Console.WriteLine("Error");
                    break;
            }
            operation.Text = result;
            
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            string nameOfSetVar;
            double argumentVar;
            List<double> res = new List<double>();

            if (argument.Text.Length <= 0 || nameOfSet.Text.Length <= 0)
            {
                errors3.Text = "Pola nie mogą być puste!!";
                return;
            } else
            {
                argumentVar = Double.Parse(argument.Text.Trim());
                nameOfSetVar = nameOfSet.Text.Trim();
            }

            double[] n = Fuzzy.FindElement(nameOfSetVar, fuzzy_numbers);
            double[] Y = Fuzzy.FindY(nameOfSetVar, fuzzy_numbers);

            if(n.Length == 0 || Y.Length == 0)
            {
                errors3.Text = "Nieprawidłowe wartości!";
                return;
            }

            //double[] n = new double[] { 4, 2.74, 1.66, 0.74, 0, -0.57, -0.98, -1.22, -1.28, -1.17, -0.9, 24, 28.35, 33, 37.95, 43.2, 48.75, 54.6, 60.75, 67.2, 73.95, 81 };
            //double[] Y = new double[] { 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, 1, 0.9, 0.8, 0.7, 0.6, 0.5, 0.4, 0.3, 0.2, 0.1, 0 };

            for (int i = 1; i < n.Length - 1; i++)
            {
                if ((n[i] - n[i - 1]) != 0)
                {
                    if (argumentVar > n[i - 1] && argumentVar <= n[i] || argumentVar <= n[i - 1] && argumentVar > n[i])
                    {
                        double a1 = (Y[i - 1] - Y[i]);
                        double a2 = (n[i - 1] - n[i]);
                        double a = Math.Round(a1 / a2, 6);
                        double b = Y[i - 1] - (a * n[i - 1]);

                        //Debug.WriteLine($"a: {a}, b: {b}");

                        double y = a * argumentVar + b;
                        Debug.WriteLine($"{y}");

                        double result = (argumentVar - n[i - 1]) / (n[i] - n[i - 1]);
                        res.Add(result);
                    }
                }
            }
            if (res.Count == 0) res.Add(0);

            if(res.Count == 0) errors3.Text = $"Wystąpił błąd podczas obliczeń!";
            else
            {
                string text = "";
                foreach (double r in res)
                {
                    text += $"* {r}\n";
                    Debug.WriteLine(r);
                }
                errors3.Text = $"Wartości przynależności wynoszą:\n{text}";
            }
        }
    }
}
