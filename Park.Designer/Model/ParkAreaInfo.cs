using Park.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Park.Designer.Model
{
    public class ParkAreaInfo : INotifyPropertyChanged
    {
        private string name = "停车场";
        public string Name
        {
            get => name;
            set
            {
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        private int width = 35;
        public int Width
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

        public int Length
        {
            get => length;
            set
            {
                length = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Length)));
            }
        }
        public List<ParkingSpace> ParkingSpaces { get; set; } = new List<ParkingSpace>();
        public List<Aisle> Aisles { get; set; } = new List<Aisle>();
        public override string ToString()
        {
            return Name;
        }
    }
}
