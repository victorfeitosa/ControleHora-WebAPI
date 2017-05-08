using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace ControleHora_WebAPI.Models
{
    public class HourEntry
    {
        // [BsonElement("_id")]
        // [BsonId]
        // public ObjectId ID { get; set; }
        [BsonElementAttribute("employee_id")]
        public ObjectId? EmployeeId { get; set; }

        [BsonElement("employee")]
        public string EmployeeName;

        [BsonElement("date")]
        public DateTime DateRegistered { get; set; }

        [BsonElement("reason")]
        public string Reason { get; set; }

        [BsonElement("amount")]
        public int Amount { get; set; }

        public HourEntry(ObjectId employee, DateTime date, int amount, string reason = "", string employeeName = "")
        {
            EmployeeId = employee;
            EmployeeName = employeeName;
            DateRegistered = date;
            Amount = amount;
            Reason = reason;
        }

        public override string ToString()
        {
            return $"Employee: {EmployeeId} | Date: {DateRegistered} | Amount: {Amount}" +
                $"\nReason: {Reason}";
        }
    }

}
