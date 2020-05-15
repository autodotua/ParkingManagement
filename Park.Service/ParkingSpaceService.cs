using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Park.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park.Service
{
    public static class ParkingSpaceService
    {
        /// <summary>
        /// 设置停车位占用装态
        /// </summary>
        /// <param name="db"></param>
        /// <param name="parkingSpace"></param>
        /// <param name="hasCar"></param>
        /// <returns></returns>
        public static async Task SetParkingSpaceStatus(ParkContext db, ParkingSpace parkingSpace, bool hasCar)
        {
            parkingSpace.HasCar = hasCar;
            db.Entry(parkingSpace).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }
        /// <summary>
        /// 获取停车区地图
        /// </summary>
        /// <param name="db"></param>
        /// <param name="parkArea"></param>
        /// <returns></returns>
        public static Bitmap GetMap(ParkContext db, ParkArea parkArea)
        {
            double scale = 10;//设置倍率为10，即每0.5米用10个像素来表示
            Bitmap bitmap = new Bitmap((int)(parkArea.Length * scale), (int)(parkArea.Width * scale));
            using Graphics g = Graphics.FromImage(bitmap);//画板
            using Pen aislePan = new Pen(Brushes.Gray, (float)(2 * scale));//通道笔
            using Pen wallPan = new Pen(Brushes.Red, (float)(2 * scale));//墙的笔

            foreach (var ps in parkArea.ParkingSpaces)
            {
                //对停车位应用旋转变换
                using Matrix m = new Matrix();
                Rectangle r = new Rectangle((int)(scale * ps.X), (int)(scale * ps.Y),
                    (int)(scale * ps.Width), (int)(scale * ps.Height));
                m.RotateAt((float)ps.RotateAngle, new PointF(r.Left + (r.Width / 2),
                                          r.Top + (r.Height / 2)));
                g.Transform = m;
                //根据停车位是否有车的状态设置不同的颜色
                g.FillRectangle(ps.HasCar ? Brushes.Orange : Brushes.Green, r);
                g.ResetTransform();
            }
            //画通道
            foreach (var a in parkArea.Aisles)
            {
                g.DrawLine(aislePan, (float)(scale * a.X1), (float)(scale * a.Y1), (float)(scale * a.X2), (float)(scale * a.Y2));
            }
            //画墙
            foreach (var w in parkArea.Walls)
            {
                g.DrawLine(wallPan, (float)(scale * w.X1), (float)(scale * w.Y1), (float)(scale * w.X2), (float)(scale * w.Y2));
            }
            return bitmap;
        }

        public static async Task<List<ParkArea> > ImportFromJsonAsync(ParkContext db, string json)
        {
            List<ParkArea> parkAreas = JsonConvert.DeserializeObject<List<ParkArea>>(json);
            var existedParks = db.ParkAreas.Where(p => parkAreas.Select(q => q.Name).Contains(p.Name));
            (await existedParks.ToListAsync()).ForEach(p => db.ParkAreas.Remove(p));
            foreach (var parkArea in parkAreas)
            {
                foreach (var ps in parkArea.ParkingSpaces)
                {
                    ps.ParkArea = parkArea;
                    ps.Class ??= "";
                    ps.ID = 0;
                }
                foreach (var a in parkArea.Aisles)
                {
                    a.ParkArea = parkArea;
                    a.Class ??= "";
                    a.ID = 0;
                }
                foreach (var w in parkArea.Walls)
                {
                    w.ParkArea = parkArea;
                    w.Class ??= "";
                    w.ID = 0;
                }

                db.Add(parkArea);
            }
            await db.SaveChangesAsync();
            return parkAreas;
        }
    }
}
