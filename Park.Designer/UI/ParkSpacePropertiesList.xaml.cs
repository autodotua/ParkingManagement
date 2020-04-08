using Park.Models;
using Park.Designer.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Park.Designer.UI
{
    /// <summary>
    /// ParkSpacePropertiesList.xaml 的交互逻辑
    /// </summary>
    public partial class ParkSpacePropertiesList : UserControl,INotifyPropertyChanged
    {
        private IParkObject obj;

        public event PropertyChangedEventHandler PropertyChanged;

        public IParkObject Obj
        {
            get => obj;
            set
            {
                obj = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Obj)));
            }
        }

        public ParkSpacePropertiesList()
        {
            DataContext = this;
            InitializeComponent();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Delete?.Invoke(this, new ParkObjectEventArgs(Obj));
            Obj = null;
        }
        public event EventHandler<ParkObjectEventArgs> Delete;
    }
}
