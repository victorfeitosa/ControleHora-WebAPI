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

    public enum HoursWeek
    {
        FullTimeExtra = 44,
        FullTime = 40,
        PartTime = 30,
        PartTimeLesser = 20
    }

    public enum AccessOperation
    {
        List, Add, Remove
    }

    public class AccessEntry
    {
        [BsonId]
        [BsonElement("employee_id")]
        [BsonRepresentation(BsonType.String)]
        public ObjectId ID { get; set; }

        [BsonElement("employee_id")]
        [BsonRepresentation(BsonType.String)]
        public ObjectId EmployeeID { get; set; }

        [BsonElement("operation")]
        public AccessOperation Operation { get; set; }

        [BsonElement("timestamp")]
        [BsonRepresentation(BsonType.String)]
        public DateTime Timestamp { get; set; }

        public bool Success { get; set; }

        public AccessEntry(ObjectId employeeId, AccessOperation operation, bool success)
        {
            EmployeeID = employeeId;
            Operation = operation;
            Timestamp = DateTime.Now;
            Success = success;
        }
    }
}