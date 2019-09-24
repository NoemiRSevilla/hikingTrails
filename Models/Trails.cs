using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hiking.Models
{

    public class Trails
    {
        [Key]
        public int id {get;set;}
        public string name {get;set;}
        public string type {get;set;}
        public string summary {get;set;}
        public string difficulty {get;set;}
        public float stars {get;set;}
        public int starVotes {get;set;}
        public string location {get;set;}
        public string url {get;set;}
        public int length {get;set;}
        public int ascent {get;set;}
        public int high {get;set;}
        public int low {get;set;}
        public float longitutde {get;set;}
        public float latitude {get;set;}
        public string conditionStatus {get;set;}
        public string conditionDetails {get;set;}
        public DateTime conditionDate {get;set;}
    }
}