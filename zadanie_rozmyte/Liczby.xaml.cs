﻿using System;
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
        Regex rx = new Regex(@"^(\-?\d+(.\d+)?;){3,}\-?\d+(.\d+)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
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

        private string CalculateNumbers(string oper, double[] numbers1, double[] numbers2, double discretization)
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
            for (int i = 0; i < resultArrAdd.Length; i++)
            {
                resultArrAdd[i] = resultArrAdd[i].Replace("(", "");
                upsAdd.Add(Double.Parse(resultArrAdd[i]));
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
        public Liczby()
        {
            InitializeComponent();
            readText = File.ReadAllLines("fuzzy.txt");

            foreach (string n in readText)
            {
                fuzzy_numbers.Add(new Fuzzy(n.Split("|")[1], n.Split("|")[0], Double.Parse(n.Split("|")[2])));
            }

            Grid myGrid = new Grid();
            ColumnDefinition colDef1 = new ColumnDefinition();
            ColumnDefinition colDef2 = new ColumnDefinition();
            RowDefinition rowDef1 = new RowDefinition();

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

            titleNameTextBlock.FontSize = 32;
            titleNumberTextBlock.FontSize = 32;

            myGrid.Children.Add(titleNameTextBlock);
            myGrid.Children.Add(titleNumberTextBlock);

            for (int i = 0; i < fuzzy_numbers.Count; i++) {
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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string operationValue = operation.Text;
            string[] inputNumbers = operationValue.Split('+', '-', '/', '*');

            bool isValidNumbers = ValidateNumbers(inputNumbers);
            if (isValidNumbers == false) return;

            double[] numbers1;
            double[] numbers2;

            //znajdowanie operatora
            int pos = inputNumbers[0].Length;
            char operat = operationValue[pos];
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

            foreach(Fuzzy d in fuzzy_numbers) {
                if (d.Number == numberConcat) {
                    discretization = d.Discretization;
                    break;
                };
            }


            //obliczenia
            string result = "";
            errors2.Text = "";
            switch (operat)
            {
                case '-':
                    result = CalculateNumbers("-", numbers1, numbers2, discretization);
                    break;
                case '+':
                    result = CalculateNumbers("+", numbers1, numbers2, discretization);
                    break;
                case '*':
                    result = CalculateNumbers("*", numbers1, numbers2, discretization);

                    break;
                case '/':
                    result = CalculateNumbers("/", numbers1, numbers2, discretization);
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
