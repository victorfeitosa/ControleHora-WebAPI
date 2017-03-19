using System;
using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace ControleHora_WebAPI.Models
{
    [JsonObject]
    public class Employee
    {
        [BsonElement("_id")]
        [BsonId]
        public ObjectId ID { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("position")]
        public Position Position { get; set; }

        //Defines a nullable date
        [BsonElement("date_joined")]
        public DateTime? DateJoined { get; set; }


        [BsonElement("week_hours")]
        public HoursAmount WeekHours { get; set; }

        [BsonElement("hour_bank")]
        public double HourBank { get; set; }

        public Employee()
        {
            HourBank = 0;
        }
        public Employee(string name, Position pos, DateTime dateJoined, HoursAmount weekHours)
        {
            Name = name;
            Position = pos;
            DateJoined = dateJoined;
            WeekHours = weekHours;
            HourBank = 0;
        }

        public override string ToString()
        {
            string str = $"ID: {ID}, Name: {Name}, Position: {Position},"
                + $"DateJoined: {DateJoined}, WeekHours: {WeekHours}, HourBank: {HourBank}";
            return str;
        }
    }
}