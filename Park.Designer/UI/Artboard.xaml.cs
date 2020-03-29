using Park.Designer.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
    /// Artboard.xaml 的交互逻辑
    /// </summary>
    public partial class Artboard : Canvas, INotifyPropertyChanged
    {
        private ParkAreaInfo parkArea = new ParkAreaInfo();

        public event PropertyChangedEventHandler PropertyChanged;

        public ParkAreaInfo ParkArea
        {
            get => parkArea;
            set
            {
                parkArea = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParkArea)));
            }
        }
        private Shape currentShape;
        private DrawingMode drawingMode = DrawingMode.None;
        private Point aisleStartPoint;
        public ParkObjectBase Template { get; private set; }

        public void StartDrawing(ParkObjectBase template)
        {
            StartDraw(template, true);
        }
        private void StartDraw(ParkObjectBase template, bool cleanCurrent)
        {
            Template = template;

            if (cleanCurrent && currentShape != null)
            {
                Children.Remove(currentShape);
            }
            switch (template)
            {
                case ParkingSpace _:
                    drawingMode = DrawingMode.ParkingSpace;
                    currentShape = new Rectangle()
                    {
                        Height = template.Height,
                        Width = template.Width,
                        Fill = Brushes.Red,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    Children.Add(currentShape);
                    break;
                case Aisle _:
                    drawingMode = DrawingMode.Aisle1;
                    currentShape = new Rectangle()
                    {
                        Height = 2,
                        Width = 2,
                        Fill = Brushes.Green,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    Children.Add(currentShape);
                    break;
            }
        }
        public void StopDrawing()
        {
            if (currentShape != null)
            {
                Children.Remove(currentShape);
            }
            drawingMode = DrawingMode.None;
            Template = null;
        }
        public Artboard()
        {
            InitializeComponent();
        }

        public void Restore()
        {
            Children.Clear();
            DrawGrid();
            foreach (var obj in ParkArea.ParkingSpaces.Union<ParkObjectBase>(ParkArea.Aisles))
            {
                var rect = new Rectangle()
                {
                    Height = obj.Height,
                    Width = obj.Width,
                    RenderTransformOrigin = new Point(0.5, 0.5)
                };
                Brush color = obj switch
                {
                    ParkingSpace _ => ParkingSpace.Brush,
                    Aisle _ => Aisle.Brush,
                    _ => throw new NotImplementedException()
                };
                rect.Fill = color;
                SetLeft(rect, obj.X);
                SetTop(rect, obj.Y);
                Children.Add(rect);
            }
        }
        private void DrawGrid()
        {

            for (double x = 0; x < parkArea.Length; x += 0.5)
            {
                for (double y = 0; y < parkArea.Width; y += 0.5)
                {
                    if (x + y == (int)(x + y))
                    {
                        Rectangle rect = new Rectangle()
                        {
                            Height = 0.5,
                            Width = 0.5,
                            Fill = Brushes.LightGray
                        };
                        SetLeft(rect, x);
                        SetTop(rect, y);
                        Children.Add(rect);
                    }
                }
            }
        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (drawingMode == DrawingMode.None)
            {
                Cursor = Cursors.Arrow;
                return;
            }
            Cursor = Cursors.None;
            var pos = e.GetPosition(this);
            switch (drawingMode)
            {
                case DrawingMode.ParkingSpace:
                case DrawingMode.Aisle1:

                    double x = Math.Round(2 * pos.X - currentShape.Width) / 2;
                    double y = Math.Round(2 * pos.Y - currentShape.Height) / 2;
                    SetLeft(currentShape, x);
                    SetTop(currentShape, y);
                    break;
                case DrawingMode.Aisle2:
                    double width = Math.Round(2 * pos.X - 2 * aisleStartPoint.X + currentShape.Width) / 2;
                    double height = Math.Round(2 * pos.Y - 2 * aisleStartPoint.Y + currentShape.Height) / 2;
                    width = Math.Max(2, width);
                    height = Math.Max(2, height);
                    if (width > height)
                    {
                        currentShape.Width = width;
                        currentShape.Height = 2;
                    }
                    else
                    {
                        currentShape.Width = 2;
                        currentShape.Height = height;
                    }
                    break;
            }
        }

        public event EventHandler<ParkObjectEventArgs> ParkObjectPlaced;
        public event EventHandler<ParkObjectEventArgs> ParkObjectSelected;

        private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ParkObjectBase obj = null;
            switch (drawingMode)
            {
                case DrawingMode.None:
                    break;
                case DrawingMode.ParkingSpace:
                    obj = new ParkingSpace()
                    {
                        X = GetLeft(currentShape),
                        Y = GetTop(currentShape),
                        Width = Template.Width,
                        Height = Template.Height
                    };
                    ParkObjectPlaced?.Invoke(this, new ParkObjectEventArgs(obj));
                    break;
                case DrawingMode.Aisle1:
                    drawingMode = DrawingMode.Aisle2;
                    var x = GetLeft(currentShape) + currentShape.Width / 2;
                    var y = GetTop(currentShape) + currentShape.Height / 2;
                    aisleStartPoint = new Point(x, y);
                    break;
                case DrawingMode.Aisle2:
                    obj = new Aisle()
                    {
                        X = GetLeft(currentShape),
                        Y = GetTop(currentShape),
                        Width = Template.Width,
                        Height = Template.Height
                    };
                    ParkObjectPlaced?.Invoke(this, new ParkObjectEventArgs(obj));
                    break;
                default:
                    break;
            }
            if (obj != null)
            {
                currentShape.MouseEnter += (s, e) =>
                {
                    if (CanSelect)
                    {
                        var shape = s as Shape;
                        shape.RenderTransform = new ScaleTransform(1.2, 1.2, 0, 0);
                    }
                };
                currentShape.MouseLeave += (s, e) =>
                {
                    if (CanSelect)
                    {
                        var shape = s as Shape;
                        shape.RenderTransform = null;
                    }
                };
                currentShape.PreviewMouseLeftButtonDown += (s, e) =>
                {
                    if (CanSelect)
                    {
                        ParkObjectSelected?.Invoke(this, new ParkObjectEventArgs((s as Shape).Tag as ParkObjectBase));
                    }
                };
                currentShape.Tag = obj;
                StartDraw(Template, false);
                Canvas_PreviewMouseMove(sender, e);
            }
        }
        public bool CanSelect { get; set; }
    }
    public class ParkObjectEventArgs : EventArgs
    {
        public ParkObjectEventArgs(ParkObjectBase obj)
        {
            ParkObject = obj;
        }

        public ParkObjectBase ParkObject { get; }
    }

    public enum DrawingMode
    {
        None,
        ParkingSpace,
        Aisle1,
        Aisle2
    }
}
