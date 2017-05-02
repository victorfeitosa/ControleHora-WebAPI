using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace ControleHora_WebAPI.Models
{
    public enum Position
    {
        Junior = 0,
        Experienced = 1,
        Senior = 2
    }
    public enum Role
    {
        Developer = 0,
        Tester = 1,
        TechnicalLeader = 2,
        ProjectLeader = 3,
        ProjectManager = 4
    }

    public enum HoursAmount
    {
        FullTimeExtra = 44,
        FullTime = 40,
        PartTime = 30,
        PartTimeLesser = 20
    }

    public class HourEntry
    {
        [BsonElement("_id")]
        [BsonId]
        public ObjectId ID { get; set; }
        [BsonElementAttribute("employee_id")]
        public ObjectId EmployeeId { get; set; }
        [BsonElement("date")]
        public DateTime DateRegistered { get; set; }
        [BsonElement("reason")]
        public string Reason { get; set; }
        [BsonElement("amount")]
        public int Amount { get; set; }

        public HourEntry(ObjectId employee, DateTime date, int amount, string reason = "")
        {
            ID = new ObjectId();
            EmployeeId = employee;
            DateRegistered = date;
            Amount = amount;
            Reason = reason;
        }

        public override string ToString()
        {
            return $"Employee: {EmployeeId} | Date: {DateRegistered} | Amount: {Amount}" +
                "\nReason: {Reason}";
        }
    }
}