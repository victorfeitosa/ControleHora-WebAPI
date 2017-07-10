using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ControleHora_WebAPI.Models
{
    [BsonIgnoreExtraElements]
    public class Employee
    {
        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.String)]
        public ObjectId ID { get; private set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [RegularExpression(@"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])")]
        [StringLength(10)]
        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("position")]
        public Position Position { get; set; }

        [BsonElement("role")]
        public Role Role { get; set; }

        //Defines a nullable date
        [BsonElement("date_joined")]
        [BsonRepresentation(BsonType.String)]
        public DateTime? DateJoined { get; private set; }


        [BsonElement("week_hours")]
        public HoursWeek WeekHours { get; set; }

        [BsonElement("hour_bank")]
        public double HourBank { get; set; }

        [BsonElement("hours")]
        public List<HourEntry> Entries { get; set; }

        public Employee()
        {
            HourBank = 0;
        }
        public Employee(string name, string email, Position pos, Role role, HoursWeek weekHours, DateTime dateJoined = new DateTime())
        {
            Name = name;
            Email = email;
            Position = pos;
            Role = role;
            DateJoined = dateJoined;
            WeekHours = weekHours;
            HourBank = 0;
            Entries = new List<HourEntry>();
        }

        public void addHourEntry(DateTime date, int amount, string reason)
        {
            Entries.Add(new HourEntry(ID, date, amount, reason));
        }

        public override string ToString()
        {
            string str = $"ID: {ID}, Name: {Name}, E-Mail: {Email}, Position: {Position}, Role: {Role}, "
                + $"DateJoined: {DateJoined}, WeekHours: {WeekHours}, HourBank: {HourBank}";
            return str;
        }
    }
}

