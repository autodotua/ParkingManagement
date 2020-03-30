using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Text;

namespace Park.Designer.Model
{
    public class ParkingSpace : ParkObjectBase
    {
        public static readonly Brush Brush = Brushes.Red;
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double RotateAngle { get; set; }
    }
}
