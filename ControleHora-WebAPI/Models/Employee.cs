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
// id: {
// timestamp: 1493749201,
// machine: 6866385,
// pid: -7383,
// increment: 2203140,
// creationTime: "2017-05-02T18:20:01Z"
// },
// name: "Jow",
// position: 0,
// role: 0,
// dateJoined: "2017-05-02T18:20:01.588Z",
// weekHours: 40,
// hourBank: 0,
// entries: [
// {
// id: {
// timestamp: 0,
// machine: 0,
// pid: 0,
// increment: 0,
// creationTime: "1970-01-01T00:00:00Z"
// },
// employeeId: {
// timestamp: 1493749201,
// machine: 6866385,
// pid: -7383,
// increment: 2203140,
// creationTime: "2017-05-02T18:20:01Z"
// },
// dateRegistered: "2017-05-02T18:24:34.548Z",
// reason: "Overslept",
// amount: -4
// }
// ]
// }