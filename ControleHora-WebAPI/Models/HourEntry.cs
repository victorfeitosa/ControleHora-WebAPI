using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace ControleHora_WebAPI.Models
{
    public class HourEntry
    {
        /**
        Example JSON
        {
            "_id": string,
            "employee_id": string,
            "employee": string,
            "date": string,
            "reason": string,
            "amount": int
        }
         */
        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.String)]
        public ObjectId ID { get; set; }
        
        [BsonElement("employee_id")]
        [BsonRepresentation(BsonType.String)]
        public ObjectId? EmployeeId { get; set; }

        [BsonElement("employee")]
        public string EmployeeName;

        [BsonElement("date")]
        [BsonRepresentation(BsonType.String)]
        public DateTime? DateRegistered { get; set; }

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
            return $"ID: {ID} | Employee: {EmployeeName}:{EmployeeId} | Date: {DateRegistered.ToString()} | Amount: {Amount}" +
                $"\nReason: {Reason}";
        }
    }

}
