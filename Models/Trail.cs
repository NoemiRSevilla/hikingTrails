using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hiking.Models
{

    public class Trail
    {
        [Key]
        public int id {get;set;}
        public List<Favorite> ownerOfFaves { get; set; }
    }
}