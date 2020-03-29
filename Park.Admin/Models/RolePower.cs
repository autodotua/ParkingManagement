using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Park.Admin.Models
{
    // https://docs.microsoft.com/en-us/ef/core/modeling/relationships
    // Many-to-many relationships without an entity class to represent the join table are not yet supported.

    public class RolePower : IKey2ID
    {
        public int RoleID { get; set; }
        public Role Role { get; set; }

        public int PowerID { get; set; }
        public Power Power { get; set; }


        [NotMapped]
        public int ID1
        {
            get
            {
                return RoleID;
            }
            set
            {
                RoleID = value;
            }
        }
        [NotMapped]
        public int ID2
        {
            get
            {
                return PowerID;
            }
            set
            {
                PowerID = value;
            }
        }

    }
}