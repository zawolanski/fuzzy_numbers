using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace zadanie_rozmyte
{
    public class Points
    {
        public ObservableCollection<Point> Data { get; set; }

        public Points()
        {
            Data = new ObservableCollection<Point>();
        }

        public void Add(double x, double y)
        {
            Data.Add(new Point { X = x, Y = y});
        }
    }
}
