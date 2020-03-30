//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Windows.Controls;

//namespace Park.Designer.UI
//{
//   public class GridCanvas:Canvas
//    {
//        private void DrawGrid()
//        {

//            for (double x = 0; x < parkArea.Length; x += 0.5)
//            {
//                for (double y = 0; y < parkArea.Width; y += 0.5)
//                {
//                    if (x + y == (int)(x + y))
//                    {
//                        Rectangle rect = new Rectangle()
//                        {
//                            Height = 0.5,
//                            Width = 0.5,
//                            Fill = Brushes.LightGray
//                        };
//                        SetLeft(rect, x);
//                        SetTop(rect, y);
//                        Children.Add(rect);
//                    }
//                }
//            }
//        }

//    }
//}
