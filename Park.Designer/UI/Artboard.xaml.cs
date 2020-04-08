using Park.Models;
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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Park.Designer.UI
{
    /// <summary>
    /// Artboard.xaml 的交互逻辑
    /// </summary>
    public partial class Artboard : Grid, INotifyPropertyChanged
    {
        public static readonly double LineWidth = 2;
        public static readonly Brush ParkingSpaceBrush = Brushes.Green;
        public static readonly Brush AisleBrush = Brushes.Gray;
        public static readonly Brush WallBrush = Brushes.Red;
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
        /// <summary>
        /// 当前的模板
        /// </summary>
        public IParkObject Template { get; private set; }
        /// <summary>
        /// 是否处于选择模式
        /// </summary>
        public bool CanSelect { get; set; }

        /// <summary>
        /// 新对象放置事件
        /// </summary>
        public event EventHandler<ParkObjectEventArgs> ParkObjectPlaced;
        /// <summary>
        /// 对象被选择
        /// </summary>
        public event EventHandler<ParkObjectEventArgs> ParkObjectSelected;
        /// <summary>
        /// 开始绘制
        /// </summary>
        /// <param name="template"></param>
        public void StartDrawing(IParkObject template)
        {
            StartDraw(template, true);
        }
        /// <summary>
        /// 开始绘制，内部方法
        /// </summary>
        /// <param name="template"></param>
        /// <param name="cleanCurrent"></param>
        private void StartDraw(IParkObject template, bool cleanCurrent)
        {
            Template = template;

            if (cleanCurrent && currentShape != null)
            {
                cvs.Children.Remove(currentShape);
            }
            switch (template)
            {
                case ParkingSpace ps:
                    drawingMode = DrawingMode.ParkingSpace;
                    currentShape = GetParkingSpacesRect(ps);
                    break;
                case Aisle _:
                    drawingMode = DrawingMode.Line1;
                    currentShape = new Rectangle()
                    {
                        Height = LineWidth,
                        Width = LineWidth,
                        Fill = AisleBrush,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };
                    break;        
                case Wall _:
                    drawingMode = DrawingMode.Line1;
                    currentShape = new Rectangle()
                    {
                        Height = LineWidth,
                        Width = LineWidth,
                        Fill = WallBrush,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };
                    break;
            }
            cvs.Children.Add(currentShape);
        }
        /// <summary>
        /// 停止绘制
        /// </summary>
        public void StopDrawing()
        {
            if (currentShape != null)
            {
                cvs.Children.Remove(currentShape);
            }
            drawingMode = DrawingMode.None;
            Template = null;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public Artboard()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 从停车位对象获取图形
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        private Rectangle GetParkingSpacesRect(ParkingSpace ps)
        {
            return new Rectangle()
            {
                Height = ps.Height,
                Width = ps.Width,
                RenderTransformOrigin = new Point(0.5, 0.5),
                Fill = ParkingSpaceBrush,
                RenderTransform = new RotateTransform(ps.RotateAngle),
                Stroke = Brushes.Gray,
                StrokeThickness = 0,
            };
        }
        /// <summary>
        /// 从通道对象获取图形
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private Line GetLine(LineParkObjectBase obj)
        {
            return new Line()
            {
                X1 = obj.X1,
                Y1 = obj.Y1,
                X2 = obj.X2,
                Y2 = obj.Y2,
                Stroke =obj is Aisle? AisleBrush:WallBrush,
                StrokeThickness = LineWidth,
                RenderTransformOrigin = new Point(0.5, 0.5)
            };
        }
        /// <summary>
        /// 从对象恢复图形
        /// </summary>
        public void Restore()
        {
            cvs.Children.Clear();
            DrawGrid();
            foreach (var obj in ParkArea.ParkingSpaces)
            {
                var rect = GetParkingSpacesRect(obj);
                rect.Tag = obj;
                Canvas.SetLeft(rect, obj.X);
                Canvas.SetTop(rect, obj.Y);
                cvs.Children.Add(rect);
                AddSelectEvents(rect);
            }
            foreach (var obj in ParkArea.Aisles.Union<LineParkObjectBase>(parkArea.Walls))
            {
                var line = GetLine(obj);
                line.Tag = obj;
                cvs.Children.Add(line);
                AddSelectEvents(line);
            }
        }
        /// <summary>
        /// 绘制底图的网格
        /// </summary>
        private void DrawGrid()
        {
            PathGeometry geometry = new PathGeometry();
            //直接使用许多个Rectangle（Shape）会导致性能低下完全不可用
            Path path = new Path();
            path.Fill = Brushes.LightGray;
            path.Data = geometry;
            for (double x = 0; x < parkArea.Length; x += 0.5)
            {
                for (double y = 0; y < parkArea.Width; y += 0.5)
                {
                    if (x + y == (int)(x + y))
                    {
                        RectangleGeometry rect = new RectangleGeometry()
                        {
                            Rect = new Rect(new Point(x, y), new Point(x + 0.5, y + 0.5)),

                            //Height = 0.5,
                            //Width = 0.5,
                            //Fill = Brushes.LightGray
                        };
                        geometry.AddGeometry(rect);

                        //Canvas.SetLeft(rect, x);
                        //Canvas.SetTop(rect, y);
                        //grid.Children.Add(rect);
                    }
                }
            }
            grid.Children.Add(path);
        }
        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Focus();
            if (drawingMode == DrawingMode.None)
            {
                Cursor = Cursors.Arrow;
                return;
            }
            Cursor = Cursors.Cross;
            var pos = e.GetPosition(this);
            switch (drawingMode)
            {
                case DrawingMode.ParkingSpace:
                case DrawingMode.Line1:
                    double x = Math.Round(2 * pos.X - currentShape.Width) / 2;
                    double y = Math.Round(2 * pos.Y - currentShape.Height) / 2;
                    Canvas.SetLeft(currentShape, x);
                    Canvas.SetTop(currentShape, y);
                    break;
                case DrawingMode.Line2:
                    Line line = currentShape as Line;
                    double width = Math.Round(2 * pos.X - 2 * line.X1) / 2;
                    double height = Math.Round(2 * pos.Y - 2 * line.Y1) / 2;
                    //width = Math.Max(2, width);
                    //height = Math.Max(2, height);
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                    {
                        if (Math.Abs(width) > Math.Abs(height))
                        {
                            line.X2 = Math.Round(2 * pos.X) / 2;
                            line.Y2 = line.Y1;
                            //currentShape.Width = width;
                            //currentShape.Height = 2;
                        }
                        else
                        {
                            line.X2 = line.X1;
                            line.Y2 = Math.Round(2 * pos.Y) / 2;
                            //currentShape.Width = 2;
                            //currentShape.Height = height;
                        }
                    }
                    else
                    {
                        line.X2 = Math.Round(2 * pos.X) / 2;
                        line.Y2 = Math.Round(2 * pos.Y) / 2;
                    }
                    break;
            }
        }

        internal void Remove(IParkObject parkObject)
        {
            cvs.Children.Remove(cvs.Children.OfType<Shape>().First(p => p.Tag == parkObject));
        }

        /// <summary>
        /// 左键按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IParkObject obj = null;
            switch (drawingMode)
            {
                case DrawingMode.None:
                    break;
                case DrawingMode.ParkingSpace:
                    double rotate = 0;
                    if (currentShape.RenderTransform is RotateTransform t)
                    {
                        rotate = t.Angle;
                    }
                    obj = new ParkingSpace()
                    {
                        X = Canvas.GetLeft(currentShape),
                        Y = Canvas.GetTop(currentShape),
                        Width = (Template as ParkingSpace).Width,
                        Height = (Template as ParkingSpace).Height,
                        RotateAngle = rotate,
                    };
                    ParkObjectPlaced?.Invoke(this, new ParkObjectEventArgs(obj));
                    break;
                case DrawingMode.Line1:
                    drawingMode = DrawingMode.Line2;
                    var x = Canvas.GetLeft(currentShape) + currentShape.Width / 2;
                    var y = Canvas.GetTop(currentShape) + currentShape.Height / 2;
                    //aisleStartPoint = new Point(x, y);
                    cvs.Children.Remove(currentShape);
                    currentShape = new Line()
                    {
                        StrokeThickness = LineWidth,
                        Stroke = Template is Aisle ? AisleBrush : WallBrush,
                        RenderTransformOrigin = new Point(0.5, 0.5),
                        X1 = x,
                        Y1 = y,
                        X2 = x,
                        Y2 = y
                    };
                    cvs.Children.Add(currentShape);
                    break;
                case DrawingMode.Line2:
                    Line line = currentShape as Line;
                    if (Template is Aisle)
                    {
                        obj = new Aisle()
                        {
                            X1 = line.X1,
                            Y1 = line.Y1,
                            X2 = line.X2,
                            Y2 = line.Y2
                        };
                    }
                    else if(Template is Wall)
                    {
                        obj = new Wall()
                        {
                            X1 = line.X1,
                            Y1 = line.Y1,
                            X2 = line.X2,
                            Y2 = line.Y2
                        };
                    }

                    ParkObjectPlaced?.Invoke(this, new ParkObjectEventArgs(obj));
                    break;
                default:
                    break;
            }
            if (obj != null)
            {
                AddSelectEvents(currentShape);
                currentShape.Tag = obj;
                StartDraw(obj, false);
                Canvas_PreviewMouseMove(sender, e);
            }
        }
        /// <summary>
        /// 为创建的对象添加选择对象相关事件
        /// </summary>
        /// <param name="shape"></param>
        private void AddSelectEvents(Shape shape)
        {
            shape.MouseEnter += (s, e) =>
            {
                if (CanSelect)
                {
                    var shape = s as Shape;
                    //shape.StrokeThickness += 0.5;
                    shape.Effect = new DropShadowEffect() { 
                        Color = Colors.Orange ,
                        ShadowDepth=0,
                        Opacity=1,
                        BlurRadius=4,
                    };

                }
            };
            shape.MouseLeave += (s, e) =>
            {
                if (CanSelect)
                {
                    var shape = s as Shape;
                    shape.Effect = null;
                    //shape.StrokeThickness -= 0.5;
                }
            };
            shape.PreviewMouseLeftButtonDown += (s, e) =>
            {
                if (CanSelect)
                {
                    ParkObjectSelected?.Invoke(this, new ParkObjectEventArgs((s as Shape).Tag as IParkObject));
                }
            };
        }
        /// <summary>
        /// 按键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Canvas_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            grid.Visibility = Visibility.Collapsed;
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {

            }
            if (currentShape is Rectangle rect && (e.Key == Key.Left || e.Key == Key.Right))
            {
                if (rect.RenderTransform.Equals(Transform.Identity))
                {
                    rect.RenderTransform = new RotateTransform(0);
                }
                if (e.Key == Key.Left)
                {
                    (rect.RenderTransform as RotateTransform).Angle -= 15;
                }
                else
                {

                    (rect.RenderTransform as RotateTransform).Angle += 15;
                }
            }


            grid.Visibility = Visibility.Visible;
        }
    }
    /// <summary>
    /// 停车场对象相关事件参数
    /// </summary>
    public class ParkObjectEventArgs : EventArgs
    {
        public ParkObjectEventArgs(IParkObject obj)
        {
            ParkObject = obj;
        }

        public IParkObject ParkObject { get; }
    }
    /// <summary>
    /// 绘制模式
    /// </summary>
    public enum DrawingMode
    {
        None,
        ParkingSpace,
        Line1,
        Line2
    }
}
