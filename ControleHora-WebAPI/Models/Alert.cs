using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace ControleHora_WebAPI.Models
{
    public class Alert
    {
        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.String)]
        public ObjectId ID { get; private set; }

        [BsonElement("from_id")]
        [BsonRepresentation(BsonType.String)]
        public ObjectId FromID { get; private set; }

        public bool Active { get; set; }

        [BsonElement("date")]
        [BsonRepresentation(BsonType.String)]
        public DateTime Date { get; set; }

        [BsonElement("message")]
        public string Message { get; set; }

        public Alert(ObjectId from, DateTime date,
            string message = "Please, pay attention to your shcedule and avoid missing out on work.")
        {
            FromID = from;
            Date = date;
            Message = message;
            Active = true;
        }
    }
}