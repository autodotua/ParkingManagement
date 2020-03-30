using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Text;

namespace Park.Designer.Model
{
    public class Aisle : ParkObjectBase
    {
        public static readonly Brush Brush = Brushes.Green;
        public static readonly double Width = 2;
        public double X1 { get; set; }
        public double Y1 { get; set; } 
        public double X2 { get; set; }
        public double Y2 { get; set; }
    }
}
