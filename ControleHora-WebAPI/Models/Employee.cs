using System;
using System.Collections.Generic;
using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ControleHora_WebAPI.Models
{
    public class Employee
    {
        [BsonElement("_id")]
        [BsonId]
        public ObjectId ID { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("position")]
        public Position Position { get; set; }

        [BsonElement("role")]
        public Role Role { get; set; }

        //Defines a nullable date
        [BsonElement("date_joined")]
        public DateTime? DateJoined { get; set; }


        [BsonElement("week_hours")]
        public HoursAmount WeekHours { get; set; }

        [BsonElement("hour_bank")]
        public double HourBank { get; set; }
        [BsonElement("hours")]
        public List<HourEntry> Entries { get; set; }

        public Employee()
        {
            HourBank = 0;
        }
        public Employee(string name, Position pos, Role role, DateTime dateJoined, HoursAmount weekHours)
        {
            Name = name;
            Position = pos;
            Role = role;
            DateJoined = dateJoined;
            WeekHours = weekHours;
            HourBank = 0;
        }

        public void addHourEntry(DateTime date, int amount, string reason)
        {
            Entries.Add(new HourEntry(ID, date, amount, reason));
        }

        public override string ToString()
        {
            string str = $"ID: {ID}, Name: {Name}, Position: {Position}, Role: {Role}, "
                + $"DateJoined: {DateJoined}, WeekHours: {WeekHours}, HourBank: {HourBank}";
            return str;
        }
    }
}

//Sample Employee:
// {
// 	"_id" : ObjectId("5908cdd168c5d1e329219e04"),
// 	"name" : "Jow",
// 	"position" : 0,
// 	"role" : 0,
// 	"date_joined" : ISODate("2017-05-02T18:20:01.588Z"),
// 	"week_hours" : 40,
// 	"hour_bank" : 0,
// 	"hours" : [
// 		{
// 			"employee_id" : ObjectId("5908cdd168c5d1e329219e04"),
// 			"date" : ISODate("2017-05-02T18:24:34.548Z"),
// 			"reason" : "Overslept",
// 			"amount" : -4
// 		}
// 	]
// }
