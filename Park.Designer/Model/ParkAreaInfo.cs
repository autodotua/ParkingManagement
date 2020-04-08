using Park.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Park.Designer.Model
{
    public class ParkAreaInfo : ParkArea
    {
        private string name = "停车场";
        public override string Name
        {
            get => name;
            set
            {
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        private int width = 35;
        public override int Width
        {
            get => width;
            set
            {
                width = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Width)));
            }
        }
        private int length = 55;

        public event PropertyChangedEventHandler PropertyChanged;

        public override int Length
        {
            get => length;
            set
            {
                length = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Length)));
            }
        }
        public override List<ParkingSpace> ParkingSpaces { get; set; } = new List<ParkingSpace>();
        public override List<Aisle> Aisles { get; set; } = new List<Aisle>();
        public override List<Wall> Walls { get; set; } = new List<Wall>();
        public override string ToString()
        {
            return Name;
        }
    }
}
