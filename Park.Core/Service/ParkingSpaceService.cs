using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Park.Core.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park.Core.Service
{
    public static class ParkingSpaceService
    {
        public static async Task SetParkingSpaceStatus(ParkContext db, ParkingSpace parkingSpace, bool hasCar)
        {
            parkingSpace.HasCar = hasCar;
            db.Entry(parkingSpace).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }

        public static Bitmap GetMap(ParkContext db, ParkArea parkArea)
        {
            double scale = 10;
            Bitmap bitmap = new Bitmap((int)(parkArea.Length * scale), (int)(parkArea.Width * scale));
            using Graphics g = Graphics.FromImage(bitmap);
            using Pen aislePan = new Pen(Brushes.Gray, (float)(2 * scale));
            using Pen wallPan = new Pen(Brushes.Red, (float)(2 * scale));

            foreach (var ps in parkArea.ParkingSpaces)
            {
                using Matrix m = new Matrix();
                Rectangle r = new Rectangle((int)(scale * ps.X), (int)(scale * ps.Y),
                    (int)(scale * ps.Width), (int)(scale * ps.Height));
                m.RotateAt((float)ps.RotateAngle, new PointF(r.Left + (r.Width / 2),
                                          r.Top + (r.Height / 2)));
                g.Transform = m;
                g.FillRectangle(ps.HasCar ? Brushes.Orange : Brushes.Green, r);
                g.ResetTransform();

            }

            foreach (var a in parkArea.Aisles)
            {
                g.DrawLine(aislePan, (float)(scale * a.X1), (float)(scale * a.Y1), (float)(scale * a.X2), (float)(scale * a.Y2));
            }
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
