using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Park.Mgt.Models
{
    // https://docs.microsoft.com/en-us/ef/core/modeling/relationships
    // Many-to-many relationships without an entity class to represent the join table are not yet supported.

    public class TitleUser : IKey2ID
    {
        public int TitleID { get; set; }
        public Title Title { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }


        [NotMapped]
        public int ID1
        {
            get
            {
                return TitleID;
            }
            set
            {
                TitleID = value;
            }
        }
        [NotMapped]
        public int ID2
        {
            get
            {
                return UserID;
            }
            set
            {
                UserID = value;
            }
        }
    }
}