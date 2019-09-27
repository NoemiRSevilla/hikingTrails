using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hiking.Models
{

    public class Favorite
    {
        [Key]
        public int FavoriteId {get;set;}
        public int id {get;set;}
        public int UserId {get;set;}
        public Trail myTrails {get;set;}
        public User Owner {get;set;}
    }
}