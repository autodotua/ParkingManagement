using Microsoft.Win32;
using Newtonsoft.Json;
using Park.Designer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
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

namespace Park.Designer.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowBase, INotifyPropertyChanged
    {
        private ParkAreaInfo parkArea;
        public ParkAreaInfo ParkArea
        {
            get => parkArea;
            set
            {
                parkArea = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParkArea)));

                txtLength.Text = value.Length.ToString();
                txtWidth.Text = value.Width.ToString();
                cvs.ParkArea = value;
                cvs.Restore();
            }
        }
        private ObservableCollection<ParkAreaInfo> parkAreas;
        public ObservableCollection<ParkAreaInfo> ParkAreas
        {
            get => parkAreas;
            set
            {
                parkAreas = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParkAreas)));

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private MouseKeyboardHelper boardHelper;
        public MainWindow()
        {
            InitializeComponent();
            boardHelper = new MouseKeyboardHelper(bd).EnableDrag().EnableWheelScale();
            new MouseKeyboardHelper(txtLength).EnableAcceptNumberOnly();
            new MouseKeyboardHelper(txtWidth).EnableAcceptNumberOnly();
        }



        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            parkArea.Length = int.Parse(txtLength.Text);
            parkArea.Width = int.Parse(txtWidth.Text);
            cvs.Restore();
        }


        private void WindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            string path = "parks.json";
            if (File.Exists(path))
            {
                Import(path);
                ParkArea = ParkAreas[0];
            }
            //不用else，防止读取到的json为空
            if (ParkAreas == null || ParkAreas.Count == 0)
            {
                ParkAreas = new ObservableCollection<ParkAreaInfo>() { new ParkAreaInfo() };
                ParkArea = ParkAreas[0];
            }
            txtLength.Text = parkArea.Length.ToString();
            txtWidth.Text = parkArea.Width.ToString();
            var transform = bd.RenderTransform as MatrixTransform;
            var matrix = transform.Matrix;
            matrix.ScaleAtPrepend(8, 8, bd.ActualWidth / 2, bd.ActualHeight / 2);
            transform.Matrix = matrix;

        }

        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            boardHelper.DragEnable = false;
            ParkObjectBase obj = (sender as Button).Tag switch
            {
                "1" => new ParkingSpace() { Height = 2.5, Width = 4.5 },
                "2" => new Aisle(),
                _ => throw new NotImplementedException(),
            };
            MouseMode = 2;
            cvs.StartDrawing(obj);
            SetDrawingStatus(true);
        }

        private void SetDrawingStatus(bool draw)
        {
            foreach (var btn in grdDraw.Children.OfType<Button>().Where(p => p.Tag != null))
            {
                btn.IsEnabled = !draw;
            }
            grdRegion.IsEnabled = !draw;
        }

        private void StopDrawButton_Click(object sender, RoutedEventArgs e)
        {
            boardHelper.DragEnable = true;
            cvs.StopDrawing();
            SetDrawingStatus(false);
        }

        private int mouseMode = 1;
        public int MouseMode
        {
            get => mouseMode;
            set
            {
                var oldValue = mouseMode;
                mouseMode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MouseMode)));
                if (oldValue == 2)//不再绘制
                {
                    StopDrawButton_Click(null, null);
                }
                if (oldValue == 1)//不再浏览
                {
                    boardHelper.DragEnable = false;
                }
                if (value == 1)//开始浏览
                {
                    boardHelper.DragEnable = true;
                }
                if (oldValue == 0)//开始选取
                {
                    cvs.CanSelect = false;
                }
                if (value == 0)//不再选取
                {
                    cvs.CanSelect = true;
                }
                switch (value)
                {
                    case 0://选取
                        break;
                    case 1://浏览
                        break;
                    case 2://绘制
                        break;
                }
            }
        }

        private void cvs_ParkObjectPlaced(object sender, ParkObjectEventArgs e)
        {
            switch (e.ParkObject)
            {
                case ParkingSpace ps:
                    ps.Id = ParkArea.ParkingSpaces.Any() ? ParkArea.ParkingSpaces.Max(p => p.Id) + 1 : 0;
                    ParkArea.ParkingSpaces.Add(ps);
                    break;
                case Aisle a:
                    a.Id = ParkArea.Aisles.Any() ? ParkArea.Aisles.Max(p => p.Id) + 1 : 0;
                    ParkArea.Aisles.Add(a);
                    break;
            }
            props.Obj = e.ParkObject;

        }

        private void cvs_ParkObjectSelected(object sender, ParkObjectEventArgs e)
        {
            props.Obj = e.ParkObject;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            //string file = FileDialog.OpenFile(this);
            OpenFileDialog dialog = new OpenFileDialog() { Filter = "JSON配置文件|*.json" };
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                Import(path);
            }
        }

        private void Import(string path)
        {
            string json = File.ReadAllText(path);
            ParkAreas = JsonConvert.DeserializeObject<ObservableCollection<ParkAreaInfo>>(json);

        }
        private void Export(string path)
        {
            string json = JsonConvert.SerializeObject(ParkAreas);
            File.WriteAllText(path, json);
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog() { Filter = "JSON配置文件|*.json" };
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                Export(path);
            }
        }

        private void AddParkAreaButton_Click(object sender, RoutedEventArgs e)
        {
            ParkAreaInfo park = new ParkAreaInfo();
            ParkAreas.Add(park);
            ParkArea = park;
        }

        private void DeleteParkAreaButton_Click(object sender, RoutedEventArgs e)
        {
            ParkAreas.Remove(ParkArea);
            if (ParkAreas.Count == 0)
            {
                ParkAreas.Add(new ParkAreaInfo());
            }
            ParkArea = ParkAreas[0];
        }

        private void WindowBase_Closing(object sender, CancelEventArgs e)
        {
            Export("parks.json");
        }

        private void ParkObject_Delete(object sender, ParkObjectEventArgs e)
        {
            switch (e.ParkObject)
            {
                case ParkingSpace ps:
                    ParkArea.ParkingSpaces.Remove(ps);
                    break;
                case Aisle a:
                    ParkArea.Aisles.Remove(a);
                    break;
                default:
                    break;
            }
            cvs.Remove(e.ParkObject);
        }

    }
    public class IsNotNull2BoolConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
