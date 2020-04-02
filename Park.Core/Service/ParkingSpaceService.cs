using Microsoft.EntityFrameworkCore;
using Park.Core.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Park.Core.Service
{
    public static class ParkingSpaceService
    {
        public static async Task SetParkingSpaceStatus(ParkContext db,ParkingSpace parkingSpace, bool hasCar)
        {
            parkingSpace.HasCar = hasCar;
            db.Entry(parkingSpace).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }

        public static Bitmap GetMap(ParkContext db,ParkArea parkArea)
        {
            Bitmap bitmap = new Bitmap((int)(parkArea.Length*10),(int)( parkArea.Width*10));
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                Pen pen = new Pen(Brushes.Green,20);
                g.DrawLine(pen, new Point(0, 0), new Point(100, 100));
            }
                return bitmap;
        }
    }
}
