using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Park.Models
{
    public class Config : IDbModel
    {
        [Key]
        public int ID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public async static Task<string> GetAsync(ParkContext db, string key, string defaultValue)
        {
            string value = (await db.Configs.FirstOrDefaultAsync(p => p.Key == key))?.Value;
            return value ?? defaultValue;
        }
        public async static Task SetAsync(ParkContext db, string key, string value)
        {
            Config config = await db.Configs.FirstOrDefaultAsync(p => p.Key == key);
            if (config != null)
            {
                config.Value = value;
                db.Entry(config).State = EntityState.Modified;
            }
            else
            {
                config = new Config() { Key = key, Value = value };
                db.Configs.Add(config);
            }

            await db.SaveChangesAsync();

        }
    }
}
