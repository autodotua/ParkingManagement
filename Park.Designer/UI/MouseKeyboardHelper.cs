using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Park.Designer.UI
{
    class MouseKeyboardHelper
    {
        public MouseKeyboardHelper(FrameworkElement ele)
        {
            Ele = ele;
        }
        public MouseKeyboardHelper EnableDrag()
        {
            Ele.MouseLeftButtonDown += MouseLeftButtonDown;
            Ele.MouseMove += MouseMove;
            Ele.MouseLeftButtonUp += MouseLeftButtonUp;
            return this;
        }

        public bool DragEnable { get; set; } = true;
        public MouseKeyboardHelper EnableWheelScale()
        {
            Ele.PreviewMouseWheel += PreviewMouseWheel;
            return this;
        }
        Point pos = new Point();

        public FrameworkElement Ele { get; }

        private void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!DragEnable)
            {
                return;
            }
            FrameworkElement tmp = (FrameworkElement)sender;
            tmp.ReleaseMouseCapture();
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (!DragEnable)
            {
                return;
            }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                FrameworkElement tmp = (FrameworkElement)sender;
                double dx = e.GetPosition(null).X - pos.X + tmp.Margin.Left;
                double dy = e.GetPosition(null).Y - pos.Y + tmp.Margin.Top;
                tmp.Margin = new Thickness(dx, dy, -dx, -dy);
                pos = e.GetPosition(null);
            }
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!DragEnable)
            {
                return;
            }

            FrameworkElement tmp = (FrameworkElement)sender;
            pos = e.GetPosition(null);
            tmp.CaptureMouse();
            //tmp.Cursor = Cursors.Hand;
        }
        private void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var element = sender as UIElement;
            var position = e.GetPosition(element);
            var transform = element.RenderTransform as MatrixTransform;
            var matrix = transform.Matrix;
            var scale = e.Delta >= 0 ? 1.1 : (1.0 / 1.1); // choose appropriate scaling factor

            matrix.ScaleAtPrepend(scale, scale, position.X, position.Y);
            transform.Matrix = matrix;
        }


        public MouseKeyboardHelper EnableAcceptNumberOnly()
        {
            Debug.Assert(Ele is TextBox);
            var txt = Ele as TextBox;
            txt.PreviewTextInput += Txt_PreviewTextInput;
            DataObject.AddPastingHandler(txt, TextBoxPasting);
            InputMethod.SetIsInputMethodEnabled(txt, false);
            return this;
        }

        private void Txt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                string text = (String)e.DataObject.GetData(typeof(String));
                if (Regex.IsMatch(text, "[^0-9]+"))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
